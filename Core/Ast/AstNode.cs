using System.Runtime.CompilerServices;

namespace DragoonScript.Core.Ast;

abstract record AstNode
{
    public abstract IEnumerable<AstNode> Children { get; }

    public virtual string Stringify() => $"({Children.Select(c => c.Stringify()).Aggregate((p, n) => $"{p} {n}")})";
}
