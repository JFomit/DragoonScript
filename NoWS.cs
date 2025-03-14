using Compiler.Syntax;

namespace Compiler;

class NoWS(TokenStream inner) : TokenStream
{
    public override Token Next()
    {
        while (inner.Peek().Kind == TokenKind.WhiteSpace || inner.Peek().Kind == TokenKind.NewLine)
        {
            inner.Next();
        }

        return inner.Next();
    }
    public override Token Peek()
    {
        while (inner.Peek().Kind == TokenKind.WhiteSpace || inner.Peek().Kind == TokenKind.NewLine)
        {
            inner.Next();
        }

        return inner.Peek();
    }
}