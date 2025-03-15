using System.Diagnostics;
using Compiler.Syntax.Lexing;
using Compiler.Syntax.Utils;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax;

class TypeExpressionParser(TokenStream lexer) : PrattParser(lexer)
{
    protected override ParseTree ConstructTree(Token token, ParseTree rhs) => throw new NotSupportedException();

    protected override ParseTree ConstructTree(ParseTree lhs, Token token, ParseTree rhs)
    {
        Debug.Assert(token.Kind == TokenKind.SignatureArrow);
        return new ParseTree(TreeKind.TypeArrowExpr).AddChild(lhs).AddChild(new TokenTree(token)).AddChild(rhs);
    }

    protected override Option<(int lbp, int rbp)> InfixBindingPower(Token token) => token.Kind switch
    {
        TokenKind.SignatureArrow => Some((2, 1)),
        _ => None
    };

    protected override Option<int> PrefixBindingPower(Token current) => None;
}