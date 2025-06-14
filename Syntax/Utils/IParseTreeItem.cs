using DragoonScript.Utils;

namespace DragoonScript.Syntax.Utils;

interface IParseTreeItem
{
    TreeKind Kind { get; }

    void Accept(ParseTreeVisitor visitor);
    TResult Accept<TResult>(ParseTreeVisitor<TResult> visitor);
}
