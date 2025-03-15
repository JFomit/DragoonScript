using System.Diagnostics;
using Compiler.Diagnostics;
using Compiler.Syntax.Lexing;
using Compiler.Syntax.Utils;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax;

class TypeExpressionParser(TokenStream lexer, List<Diagnostic> diagnostics) : PrattParser(lexer, diagnostics)
{
    protected override ParseTree ConstructTree(Token token, ParseTree rhs) => throw new NotSupportedException();

    protected override ParseTree ConstructTree(ParseTree lhs, Token token, ParseTree rhs)
    {
        Debug.Assert(token.Kind == TokenKind.SignatureArrow);
        return new ParseTree(TreeKind.TypeArrowExpr).PushBack(lhs).PushBack(new TokenTree(token)).PushBack(rhs);
    }

    protected override ParseTree ConstructTree(ParseTree inner)
    {
        return new ParseTree(TreeKind.TypeExpr).PushBack(inner);
    }

    // TypeConstructor ::= type=ID args=TypeExpr*
    protected override ParseTree Identifier(Token current)
    {
        var tree = new ParseTree(TreeKind.TypeConstructor);
        tree.PushBack(new TokenTree(current));
        while (!At(TokenKind.SignatureArrow) && !At(TokenKind.EoF))
        {
            if (At(TokenKind.LParen))
            {
                var lp = Next();
                var inner = ParseExpression(0).UnwrapOr(new ParseTree());
                var rp = At(TokenKind.RParen) ? new TokenTree(Next()) : UnmatchedParenthesis(lp, Peek());
                tree.PushBack(ConstructTree(inner).PushFront(new TokenTree(lp)).PushBack(rp));
                continue;
            }
            if (!At(TokenKind.Identifier))
            {
                break;
            }
            tree.PushBack(new TokenTree(Next()));
        }
        return tree;
    }
    private bool At(TokenKind kind) => Peek().Kind == kind;

    protected override Option<(int lbp, int rbp)> InfixBindingPower(Token token) => token.Kind switch
    {
        TokenKind.SignatureArrow => Some((4, 3)),
        _ => None
    };

    protected override Option<int> PrefixBindingPower(Token current) => current.Kind switch
    {
        _ => None
    };

    protected override bool IsValidExpressionStart(Token peeked)
    {
        return peeked.Kind is TokenKind.Identifier or TokenKind.LParen;
    }
}