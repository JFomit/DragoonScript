using DragoonScript.Diagnostics;
using DragoonScript.Syntax.Lexing;
using JFomit.Functional.Monads;

using static JFomit.Functional.Prelude;

namespace DragoonScript.Semantic;

enum Associativity
{
    None,
    Left,
    Right
}

readonly record struct InfixOperator(Token Token, int Precedence, Associativity Associativity = Associativity.None)
{
    public static Result<InfixOperator, Diagnostic> CreateFromToken(Token token)
    {
        if (token.Kind == TokenKind.WhiteSpace)
        {
            return Ok(new InfixOperator(token, 10, Associativity.Left));
        }

        if (token.Kind != TokenKind.Operator && token.Kind != TokenKind.Pipe)
        {
            var diagnostic = Diagnostic
                .Create(DiagnosticLabel.Create(token))
                .WithSeverity(DiagnosticSeverity.Fatal)
                .WhitMessage("Passed token is not neither Operator nor Pipe token. Bug in the parser.")
                .Build();
            return Error(diagnostic);
        }
        var span = token.View.AsSpan();
        if (span.Length == 0)
        {
            var diagnostic = Diagnostic
                .Create(DiagnosticLabel.Create(token))
                .WithSeverity(DiagnosticSeverity.Fatal)
                .WhitMessage("Passed token is zero length. Bug in the lexer.")
                .Build();
            return Error(diagnostic);
        }

        if (span == "$")
        {
            return Ok(new InfixOperator(token, 0, Associativity.Right)); // right side application group
        }

        ReadOnlySpan<char> decider = span[^1..];
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] != '.')
            {
                decider = span[i..];
                break;
            }
        }

        // For some reason the formatter chokes on this and refuses to format patterns consistently
        // and instead puts some first few patterns with the same indentation, as the brace, but all the rest
        // are indented. It just looks wrong tbh
        #pragma warning disable format
        return decider switch
        {
            ['.', ..] => Ok(new InfixOperator(token, 9, Associativity.Right)), // composition group

            ['^', ..] => Ok(new InfixOperator(token, 8, Associativity.Right)), // power group
            ['*' or '/' or '%', ..] => Ok(new InfixOperator(token, 7, Associativity.Left)), // multiplicative group
            ['+' or '-', ..] => Ok(new InfixOperator(token, 6, Associativity.Left)), // additive group

            ">" or "<" or ">=" or "==" or "!=" => Ok(new InfixOperator(token, 5, Associativity.Left)), // relational group

            "&" or "&&" => Ok(new InfixOperator(token, 4, Associativity.Left)), // `and' group
            "|" or "||" => Ok(new InfixOperator(token, 3, Associativity.Left)), // `or' group
            ['|' or '&', ..] => Ok(new InfixOperator(token, 3, Associativity.Left)), // custom `logic` group

            ['>', '>', ..] or ['<', '<', ..] => Ok(new InfixOperator(token, 2, Associativity.Left)), // bit-shift group

            _ => Ok(new InfixOperator(token, 1, Associativity.None)) // wildcard group with almost no powers
        };
        #pragma warning restore format
    }

    public void Deconstruct(out int precedence, out Associativity associativity)
    {
        precedence = Precedence;
        associativity = Associativity;
    }
}