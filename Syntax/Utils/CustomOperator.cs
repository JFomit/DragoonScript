using Compiler.Diagnostics;
using Compiler.Syntax.Lexing;
using JFomit.Functional.Monads;

using static JFomit.Functional.Prelude;

namespace Compiler.Syntax.Utils;

enum Associativity
{
    None,
    Left,
    Right
}

readonly record struct CustomInfixOperator(Token Token, int Precedence, Associativity Associativity = Associativity.None)
{
    public static Result<CustomInfixOperator, Diagnostic> CreateFromToken(Token token)
    {
        if (token.Kind == TokenKind.WhiteSpace)
        {
            return Ok(new CustomInfixOperator(token, 10, Associativity.Left));
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
            return Ok(new CustomInfixOperator(token, 0, Associativity.Right)); // right side application group
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

        return decider switch
        {
        ['.', ..] => Ok(new CustomInfixOperator(token, 9, Associativity.Right)), // composition group

        ['^', ..] => Ok(new CustomInfixOperator(token, 8, Associativity.Right)), // power group
        ['*' or '/' or '%', ..] => Ok(new CustomInfixOperator(token, 7, Associativity.Left)), // multiplicative group
        ['+' or '-', ..] => Ok(new CustomInfixOperator(token, 6, Associativity.Left)), // additive group

            ">" or "<" or ">=" or "==" or "!=" => Ok(new CustomInfixOperator(token, 5, Associativity.Left)), // relational group

            "&" or "&&" => Ok(new CustomInfixOperator(token, 4, Associativity.Right)), // `and' group
            "|" or "||" => Ok(new CustomInfixOperator(token, 3, Associativity.Right)), // `or' group

            ['>', '>', ..] or ['<', '<', ..] => Ok(new CustomInfixOperator(token, 2, Associativity.Left)), // bit-shift group

            _ => Ok(new CustomInfixOperator(token, 1, Associativity.None)) // wildcard group with almost no powers
        };
    }

    public void Deconstruct(out int precedence, out Associativity associativity)
    {
        precedence = Precedence;
        associativity = Associativity;
    }
}