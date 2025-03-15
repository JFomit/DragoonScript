using System.Diagnostics;
using Compiler.Diagnostics;
using Compiler.Syntax.Lexing;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax.Utils;

abstract class PrattParser(TokenStream lexer, List<Diagnostic> diagnostics)
{
    protected TokenStream Lexer { get; } = lexer;
    private List<Diagnostic> Diagnostics { get; } = diagnostics;

    protected void PushDiagnostic(Diagnostic diagnostic) => Diagnostics.Add(diagnostic);

    private Token Next(bool skipWs = true)
    {
        while (true)
        {
            var next = Lexer.Next();
            if (skipWs && (next.Kind is TokenKind.NewLine or TokenKind.WhiteSpace))
            {
                continue;
            }

            return next;
        }
    }
    private Token Peek(bool skipWs = true)
    {
        while (true)
        {
            var peeked = Lexer.Peek();
            if (skipWs && (peeked.Kind is TokenKind.NewLine or TokenKind.WhiteSpace))
            {
                Lexer.Next();
                continue;
            }

            return peeked;
        }
    }

    public ParseTree ParseExpression(int rbp = 0)
    {
        var current = Next();
        var lhs = current.Kind switch
        {
            TokenKind.Identifier => new ParseTree(TreeKind.SimpleTypeExpr).PushBack(new TokenTree(current)),
            TokenKind.Operator or TokenKind.Pipe or TokenKind.SignatureArrow => PrefixOperator(current),
            TokenKind.LParen => Parenthesis(current),
            TokenKind.RParen => UnexpectedRightParenthesis(current),
            _ => UnexpectedToken(current)
        };

        while (true)
        {
            var op = Peek();
            var opt = InfixBindingPower(op);
            if (opt.TryUnwrap(out var operation))
            {
                if (operation.lbp < rbp)
                {
                    break;
                }
                Next();
                var rhs = ParseExpression(operation.rbp);
                lhs = ConstructTree(lhs, op, rhs);
                continue;
            }

            break;
        }

        return lhs;
    }

    private ParseTree PrefixOperator(Token current) => PrefixBindingPower(current).Match(
        (self: this, current),
        some: static (rbp, tuple) => tuple.self.ConstructTree(tuple.current, tuple.self.ParseExpression(rbp)),
        none: static tuple => tuple.self.UnexpectedToken(tuple.current)
    );
    private ParseTree UnexpectedToken(Token current)
    {
        var tree = new ParseTree(TreeKind.Error).PushBack(new TokenTree(current));
        PushDiagnostic(Diagnostic.Create(DiagnosticLabel.Create(current))
            .WithSeverity(DiagnosticSeverity.Error)
            .WhitMessage("Unexpected token.")
            .Build()
        );
        return tree;
    }
    private ParseTree UnexpectedRightParenthesis(Token rparen)
    {
        var tree = new ParseTree(TreeKind.Error);
        tree.PushBack(new TokenTree(rparen));
        var diagnostic = Diagnostic.Create(DiagnosticLabel.Create(rparen))
            .WithSeverity(DiagnosticSeverity.Error)
            .WhitMessage("Unexpected ')'.")
            .Build();
        PushDiagnostic(diagnostic);
        return tree;
    }
    private ParseTree Parenthesis(Token lparen)
    {
        var tree = ConstructTree(ParseExpression(0));
        tree.PushFront(new TokenTree(lparen));
        if (Peek().Kind != TokenKind.RParen)
        {
            tree.PushBack(new ParseTree(TreeKind.Error));
            var diagnostic = Diagnostic.Create(DiagnosticLabel.Create(Peek()))
                .WithSeverity(DiagnosticSeverity.Error)
                .WhitMessage("Unmatched parenthesis.")
                .WithLabel(DiagnosticLabel.Create(Peek()).WithMessage("Expected a ')'"))
                .WithLabel(DiagnosticLabel.Create(lparen).WithMessage("To match this '('"))
                .Build();
            PushDiagnostic(diagnostic);
        }
        else
        {
            tree.PushBack(new TokenTree(Next()));
        }

        return tree;
    }

    protected abstract ParseTree ConstructTree(ParseTree inner);
    protected abstract ParseTree ConstructTree(Token token, ParseTree rhs);
    protected abstract ParseTree ConstructTree(ParseTree lhs, Token token, ParseTree rhs);

    protected abstract Option<int> PrefixBindingPower(Token current);
    protected abstract Option<(int lbp, int rbp)> InfixBindingPower(Token token);
}