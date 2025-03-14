using System.Buffers;
using JFomit.Functional.Monads;
using Pixie.Code;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax;

internal class Lexer(StringDocument inputString) : TokenStream
{
    private int _pos = 0;
    private int _start = -1;
    private readonly StringDocument _document = inputString;

    private static readonly SearchValues<char> OperatorChars = SearchValues.Create(@"!#$%&*+./<=>?@^|-~");

    private Option<Token> buffer_ = None;

    public override Token Peek()
    {
        if (buffer_.TryUnwrap(out var peek))
        {
            return peek;
        }

        var next = Next();
        buffer_ = Some(next);
        return next;
    }

    public override Token Next()
    {
        if (buffer_.TryUnwrap(out var token))
        {
            buffer_ = None;
            return token;
        }

        var input = _document.Contents.AsSpan();
        var span = Slice(input, _pos..);
        _start = _pos;
        if (span.Length == 0)
        {
            return EmitEof();
        }

        switch (span[0])
        {
            case '\n':
                _pos++;
                return Emit(TokenKind.NewLine);
            case '\t':
            case ' ':
            state_ws:
                _pos++;
                span = Slice(input, _pos..);
                if (span.Length < 1 || (span[0] != ' ' && span[0] != '\t'))
                {
                    return Emit(TokenKind.WhiteSpace);
                }
                goto state_ws;
            case ':':
                _pos++;
                return Emit(TokenKind.Colon);
            case '(':
                _pos++;
                span = Slice(input, _pos..);
                if (span.Length < 1 || span[0] != ')')
                {
                    return Emit(TokenKind.LeftParenthesis);
                }
                _pos++;
                return Emit(TokenKind.Unit);
            case ')':
                _pos++;
                return Emit(TokenKind.RightParenthesis);
            case '\\':
                _pos++;
                throw new NotImplementedException();
        }

        var first = Slice(input, _pos..(_pos + 1));

        if (first.ContainsAnyInRange('0', '9'))
        {
        numberPart:
            _pos++;
            first = Slice(input, _pos..(_pos + 1));
            if (first.Length < 1 || first.ContainsAnyExceptInRange('0', '9'))
            {
                return Emit(TokenKind.Integer);
            }
            goto numberPart;
        }

        if (first.ContainsAny(OperatorChars))
        {
        operatorPart:
            _pos++;
            first = Slice(input, _pos..(_pos + 1));
            if (first.Length < 1 || first.ContainsAnyExcept(OperatorChars))
            {
                var result = input[_start.._pos];

                return result switch
                {
                    "->" => Emit(TokenKind.SignatureArrow),
                    "=" => Emit(TokenKind.Is),
                    "|" => Emit(TokenKind.Pipe),

                    _ => Emit(TokenKind.Operator)
                };
            }
            goto operatorPart;
        }

        if (first.Length >= 1 && (char.IsLetter(first[0]) || first[0] == '_'))
        {
        identifierPart:
            _pos++;
            first = Slice(input, _pos..(_pos + 1));
            if (first.Length < 1 || (!char.IsLetterOrDigit(first[0]) && first[0] != '_'))
            {
                var result = input[_start.._pos];

                return result switch
                {
                    "fn" => Emit(TokenKind.Fn),
                    "let" => Emit(TokenKind.Let),
                    "type" => Emit(TokenKind.Type),

                    _ => Emit(TokenKind.Identifier)
                };
            }
            goto identifierPart;
        }
        _pos++;
        return first.Length == 0 ? EmitEof() : Emit(TokenKind.Error);
    }

    private static ReadOnlySpan<char> Slice(ReadOnlySpan<char> span, Range range)
    {
        if (range.End.Value > span.Length)
        {
            return new ReadOnlySpan<char>();
        }
        else
        {
            return span[range];
        }
    }
    private Token Emit(TokenKind type) => new(type, new SourceSpan(_document.Contents, _start, _pos - _start));
    private Token EmitEof() => new(TokenKind.EoF, new SourceSpan(_document.Contents, _document.Length - 1, 0));
}
