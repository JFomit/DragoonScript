using System.Diagnostics;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax.Utils;

class PrattParser(TokenStream lexer)
{
    TokenStream Lexer { get; } = lexer;

    public ParseTree TypeExpression(int rbp = 0)
    {
        var current = Lexer.Next();
        var lhs = current.Kind switch
        {
            TokenKind.Identifier => new ParseTree(TreeKind.SimpleTypeExpr).AddChild(new TokenTree(current)),
            _ => new ParseTree(TreeKind.Error).AddChild(new TokenTree(current))
        };

        while (true)
        {
            var op = Lexer.Peek();
            var (opLbp, opRbp, shouldGo) = InfixBindingPower(op);
            if (!shouldGo)
            {
                break;
            }
            if (opLbp < rbp)
            {
                break;
            }
            Lexer.Next();
            var rhs = TypeExpression(opRbp);

            lhs = new ParseTree(TreeKind.TypeArrowExpr).AddChild(lhs).AddChild(new TokenTree(op)).AddChild(rhs);
        }

        return lhs;
    }

    private static (int, int, bool) InfixBindingPower(Token token)
    {
        if (token.Kind == TokenKind.SignatureArrow)
        {
            return (2, 1, true);
        }

        return (0, 0, false);
    }
}