using DragoonScript.Syntax;
using DragoonScript.Syntax.Utils;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Utils;

abstract class ParseTreeVisitor
{
    public virtual void VisitTree(IParseTreeItem item)
    {
        item.Accept(this);
    }

    public abstract void Visit(ParseTree tree);
    public abstract void Visit(TokenTree tokenTree);
}

abstract class ParseTreeVisitor<TResult>
{
    public virtual TResult VisitTree(IParseTreeItem item)
    {
        return item.Accept<TResult>(this);
    }

    public abstract TResult Visit(ParseTree tree);
    public abstract TResult Visit(TokenTree tokenTree);
}