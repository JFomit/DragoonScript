using System.Runtime.CompilerServices;
using DragoonScript.Utils;

namespace DragoonScript.Core.Ast;

abstract record AstNode
{
    public abstract IEnumerable<AstNode> Children { get; }
    public abstract TResult Accept<TResult>(AstNodeVisitor<TResult> visitor);
}
