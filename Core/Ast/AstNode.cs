using System.Runtime.CompilerServices;

namespace Compiler.Core.Ast;

abstract record AstNode
{
    public abstract IEnumerable<AstNode> Children { get; }
}
