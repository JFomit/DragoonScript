using System.Buffers;

namespace Compiler.Syntax;

internal class Lexer
{
    private int _pos = 0;
    private int _start = -1;

    private static readonly SearchValues<char> OperatorChars = SearchValues.Create(@"!#$%&*+./<=>?@^|-~");

    public Token NextToken(ReadOnlySpan<char> input)
    {
        var span = input[_pos..];
        _start = _pos;
        if (span.Length == 0)
        {
            return Emit(TokenType.EoF);
        }

        switch (span[0])
        {
            case '\n':
                _pos++;
                return Emit(TokenType.NewLine);
            case '\t':
            case ' ':
            state_ws:
                _pos++;
                span = input[_pos..];
                if (span.Length < 1 || (span[0] != ' ' && span[0] != '\t'))
                {
                    return Emit(TokenType.WhiteSpace);
                }
                goto state_ws;
            case ':':
                _pos++;
                return Emit(TokenType.Colon);
            case '(':
                _pos++;
                span = input[_pos..];
                if (span.Length < 1 || span[0] != ')')
                {
                    return Emit(TokenType.LeftParenthesis);
                }
                _pos++;
                return Emit(TokenType.Unit);
            case ')':
                _pos++;
                return Emit(TokenType.RightParenthesis);
            case '\\':
                _pos++;
                throw new NotImplementedException();
        }

        var first = input[_pos..(_pos + 1)];

        if (first.ContainsAnyInRange('0', '9'))
        {
        numberPart:
            _pos++;
            first = input[_pos..(_pos + 1)];
            if (first.Length < 1 || first.ContainsAnyExceptInRange('0', '9'))
            {
                return Emit(TokenType.Integer);
            }
            goto numberPart;
        }

        if (first.ContainsAny(OperatorChars))
        {
        operatorPart:
            _pos++;
            first = input[_pos..(_pos + 1)];
            if (first.Length < 1 || first.ContainsAnyExcept(OperatorChars))
            {
                var result = input[_start.._pos];

                return result switch
                {
                    "->" => Emit(TokenType.SignatureArrow),
                    "=" => Emit(TokenType.Is),
                    "|" => Emit(TokenType.Pipe),

                    _ => Emit(TokenType.Operator)
                };
            }
            goto operatorPart;
        }

        if (char.IsLetter(first[0]) || first[0] == '_')
        {
        identifierPart:
            _pos++;
            first = input[_pos..(_pos + 1)];
            if (first.Length < 1 || (!char.IsLetterOrDigit(first[0]) && first[0] != '_'))
            {
                var result = input[_start.._pos];

                return result switch
                {
                    "fn" => Emit(TokenType.Fn),
                    "let" => Emit(TokenType.Let),
                    "type" => Emit(TokenType.Type),

                    _ => Emit(TokenType.Identifier)
                };
            }
            goto identifierPart;
        }

        _pos++;
        return Emit(TokenType.Error);
    }

    private Token Emit(TokenType type, int? p = null) => Token.Create(type, _start, (p ?? _pos) - _start);
}
