using System.Buffers;
using DragoonScript.Syntax.Lexing;
using DragoonScript.Syntax.Source;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Syntax;

internal class Lexer(SourceDocument inputString) : TokenStream
{
    private int _pos = 0;
    private int _start = -1;
    private int _line = 1;
    private int _column = 1;
    public override SourceDocument Document { get; } = inputString;
    private static readonly SearchValues<char> OperatorChars = SearchValues.Create(@"!#$%&*+./<=>?@^|-~");
    private readonly Queue<Token> _buffer = [];

    public override Token Peek() => Peek(0);
    public override Token Peek(int lookahed)
    {
        while (_buffer.Count <= lookahed)
        {
            var next = NextInternal();
            _buffer.Enqueue(next);
        }

        return _buffer.ElementAt(lookahed);
    }

    private Token NextInternal()
    {
        var input = Document.Contents.AsSpan();
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
                    return Emit(TokenKind.LParen);
                }
                _pos++;
                return Emit(TokenKind.Unit);
            case ')':
                _pos++;
                return Emit(TokenKind.RParen);
                // case '\\':
                //     _pos++;
                //     throw new NotImplementedException();
        }

        var first = Slice(input, _pos..(_pos + 1));
        if (first.Contains('"'))
        {
            while (true)
            {
                _pos++;
                first = Slice(input, _pos..(_pos + 1));
                if (first.Contains('"'))
                {
                    _pos++;
                    return Emit(TokenKind.String);
                }
                if (first.Length < 1 || first.Contains('\n'))
                {
                    return Emit(TokenKind.Error);
                }
                if (first.Contains('\\'))
                {
                    _pos++;
                    first = Slice(input, _pos..(_pos + 1));
                    if (first.Contains('"'))
                    {
                        continue;
                    }
                }
            }
        }

        if (first.ContainsAnyInRange('0', '9'))
        {
            bool isFloat = false;
        numberPart:
            _pos++;
            first = Slice(input, _pos..(_pos + 1));
            if (first.Length < 1)
            {
                return Emit(isFloat ? TokenKind.Float : TokenKind.Integer);
            }

            if (first[0] == '.')
            {
                isFloat = true;
                goto numberPart;
            }

            if (first.ContainsAnyExceptInRange('0', '9'))
            {
                return Emit(isFloat ? TokenKind.Float : TokenKind.Integer);
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
                    "=" => EmitIs(),
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
                    "return" => Emit(TokenKind.Return),
                    "if" => Emit(TokenKind.If),
                    "then" => Emit(TokenKind.Then),
                    "else" => Emit(TokenKind.Else),
                    "in" => Emit(TokenKind.In),

                    _ => Emit(TokenKind.Identifier)
                };
            }
            goto identifierPart;
        }
        _pos++;
        return first.Length == 0 ? EmitEof() : Emit(TokenKind.Error);
    }

    public override Token Next()
    {
        if (_buffer.Count > 0)
        {
            return _buffer.Dequeue();
        }

        return NextInternal();
    }

    private Token EmitIs() => Emit(TokenKind.Is);

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

    private Token Emit(TokenKind type)
    {
        var token = new Token(type, new SourceSpan(Document, _start, _pos - _start, _line, _column));
        _column += token.View.Length;

        // next line
        if (type == TokenKind.NewLine)
        {
            _line++;
            _column = 1;
        }

        return token;
    }

    private Token EmitEof() => Emit(TokenKind.EoF);//new(TokenKind.EoF, new SourceSpan(Document, Document.Length - 1, 0, _line, _column));
}
