using Compiler.Syntax;
using Compiler.Syntax.Utils;

namespace Compiler.Utils;

abstract class ParseTreeVisitor
{
    public virtual void Visit(IParseTreeItem item)
    {
        item.Accept(this);
    }

    public abstract void Visit(ParseTree tree);
    public abstract void Visit(TokenTree tokenTree);
}