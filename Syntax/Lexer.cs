using System.Buffers;
using System.Diagnostics;
using DragoonScript.Syntax.Lexing;
using DragoonScript.Syntax.Source;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Syntax;

internal record struct AlignedBlock(TokenKind Kind, int Offset);

internal class Lexer(SourceDocument inputString) : TokenStream
{
    private int _pos = 0;
    private int _start = -1;
    private int _line = 1;
    private int _column = 1;
    public override SourceDocument Document { get; } = inputString;
    private static readonly SearchValues<char> OperatorChars = SearchValues.Create(@"!#$%&*+./<=>?@^|-~\\");
    private readonly Queue<Token> _buffer = [];
    private readonly Stack<int> _lineIndents = InitStack();
    private static Stack<int> InitStack()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        return stack;
    }

    private Token _last;

    public override Token Peek() => Peek(0);
    public override Token Peek(int lookahed)
    {
        while (_buffer.Count <= lookahed)
        {
            NextInternal();
        }

        return _buffer.ElementAt(lookahed);
    }

    private bool IsOnNewLine()
    {
        var input = Document.Contents.AsSpan();
        var span = Slice(input, _pos..);
        if (span.Length == 0)
        {
            return false;
        }

        return span[0] == '\n';
    }

    private void NextInternal()
    {
        var input = Document.Contents.AsSpan();
        var span = Slice(input, _pos..);
        _start = _pos;
        if (span.Length == 0)
        {
            EmitEof();
            return;
        }

        switch (span[0])
        {
            // case ';':
            //     _pos++;
            //     return Emit(TokenKind.Semi);
            case '\n':
                _pos++;
                Emit(TokenKind.NewLine);
                return;
            case '\t':
            case ' ':
            state_ws:
                _pos++;
                span = Slice(input, _pos..);
                if (span.Length < 1 || (span[0] != ' ' && span[0] != '\t'))
                {
                    Emit(TokenKind.WhiteSpace);
                    return;
                }
                goto state_ws;
            case ':':
                _pos++;
                Emit(TokenKind.Colon);
                return;
            case '(':
                _pos++;
                span = Slice(input, _pos..);
                if (span.Length < 1 || span[0] != ')')
                {
                    Emit(TokenKind.LParen);
                    return;
                }
                _pos++;
                Emit(TokenKind.Unit);
                return;
            case ')':
                _pos++;
                Emit(TokenKind.RParen);
                return;
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
                    Emit(TokenKind.String);
                    return;
                }
                if (first.Length < 1 || first.Contains('\n'))
                {
                    Emit(TokenKind.Error);
                    return;
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
                Emit(isFloat ? TokenKind.Float : TokenKind.Integer);
                return;
            }

            if (first[0] == '.')
            {
                isFloat = true;
                goto numberPart;
            }

            if (first.ContainsAnyExceptInRange('0', '9'))
            {
                Emit(isFloat ? TokenKind.Float : TokenKind.Integer);
                return;
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
                switch (result)
                {
                    case "->":
                        Emit(TokenKind.Arrow);
                        return;
                    case "=":
                        Emit(TokenKind.Is);
                        return;
                    case "|":
                        Emit(TokenKind.Pipe);
                        return;
                    case "\\":
                        Emit(TokenKind.Lambda);
                        return;
                    default:
                        Emit(TokenKind.Operator);
                        return;
                }
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

                switch (result)
                {
                    case "λ":
                        Emit(TokenKind.Lambda);
                        return;
                    case "fn":
                        Emit(TokenKind.Fn);
                        return;
                    case "let":
                        Emit(TokenKind.Let);
                        return;
                    case "type":
                        Emit(TokenKind.Type);
                        return;
                    case "if":
                        Emit(TokenKind.If);
                        return;
                    case "then":
                        Emit(TokenKind.Then);
                        return;
                    case "else":
                        Emit(TokenKind.Else);
                        return;
                    case "in":
                        Emit(TokenKind.In);
                        return;
                    case "match":
                        Emit(TokenKind.Match);
                        return;
                    case "with":
                        Emit(TokenKind.With);
                        return;
                    default:
                        Emit(TokenKind.Identifier);
                        return;
                }
            }
            goto identifierPart;
        }
        _pos++;
        if (first.Length == 0)
        {
            EmitEof();
        }
        else
        {
            Emit(TokenKind.Error);
        }
    }

    public override Token Next()
    {
        if (_buffer.Count == 0)
        {
            NextInternal();
        }

        _last = _buffer.Dequeue();
        return _last;
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

    private void Emit(TokenKind type)
    {
        var token = new Token(type, new SourceSpan(Document, _start, _pos - _start, _line, _column));
        _column += token.View.Length;

        // next line
        if (type == TokenKind.NewLine)
        {
            _line++;
            _column = 1;
        }

        if (_last.Kind == TokenKind.NewLine && ((token.Kind == TokenKind.WhiteSpace && IsOnNewLine()) || token.Kind == TokenKind.NewLine))
        {
            _buffer.Enqueue(token);
            return;
        }

        if (_last.Kind == TokenKind.NewLine)
        {
            var offside = 1;
            if (token.Kind == TokenKind.WhiteSpace)
            {
                offside += GetWhitespaceLength(token);
            }

            var current = _lineIndents.Peek();
            if (current < offside)
            {
                _buffer.Enqueue(new(TokenKind.Indent, token.View));
                _buffer.Enqueue(token);
                _lineIndents.Push(offside);
                return;
            }
            else if (current > offside)
            {
                while (current > offside)
                {
                    _buffer.Enqueue(new(TokenKind.Dedent, token.View));
                    _lineIndents.Pop();
                    current = _lineIndents.Peek();
                }
                _buffer.Enqueue(token);
                return;
            }
        }

        if (token.Kind == TokenKind.EoF)
        {
            if (_lineIndents.Count == 1)
            {
                _buffer.Enqueue(token);
                return;
            }
            // pop everything
            while (_lineIndents.Count > 1)
            {
                _lineIndents.Pop();
                _buffer.Enqueue(new(TokenKind.Dedent, token.View));
            }
            return;
        }

        _buffer.Enqueue(token);
    }
    private static int GetWhitespaceLength(in Token token)
    {
        Debug.Assert(token.Kind == TokenKind.WhiteSpace);
        var span = token.View.AsSpan();
        // Tab is eight spaces. This is by design and what Haskell is doing
        return 8 * span.Count('\t') + span.Count(' ');
    }

    private void EmitEof() => Emit(TokenKind.EoF);//new(TokenKind.EoF, new SourceSpan(Document, Document.Length - 1, 0, _line, _column));
}
