using System.Diagnostics;
using Compiler.Syntax.Lexing;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax.Utils;

abstract class PrattParser(TokenStream lexer)
{
    TokenStream Lexer { get; } = lexer;

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
            TokenKind.Identifier => new ParseTree(TreeKind.SimpleTypeExpr).AddChild(new TokenTree(current)),
            TokenKind.Operator or TokenKind.Pipe or TokenKind.SignatureArrow => PrefixOperator(current),
            _ => new ParseTree(TreeKind.Error).AddChild(new TokenTree(current))
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
            }

            break;
        }

        return lhs;
    }

    private ParseTree PrefixOperator(Token current)
    {
        var text = current.View.AsSpan();
        var c = text[0];

        return PrefixBindingPower(current).Match(
            (self: this, current),
            some: static (rbp, tuple) => tuple.self.ConstructTree(tuple.current, tuple.self.ParseExpression(rbp)),
            none: static tuple => new ParseTree(TreeKind.Error).AddChild(new TokenTree(tuple.current))
        );
    }

    protected abstract ParseTree ConstructTree(Token token, ParseTree rhs);
    protected abstract ParseTree ConstructTree(ParseTree lhs, Token token, ParseTree rhs);

    protected abstract Option<int> PrefixBindingPower(Token current);
    protected abstract Option<(int lbp, int rbp)> InfixBindingPower(Token token);
}