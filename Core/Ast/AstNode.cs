using System.Runtime.CompilerServices;

namespace Compiler.Core.Ast;

abstract record AstNode
{
    public abstract IEnumerable<AstNode> Children { get; }
}

record File : AstNode
{
    protected List<FunctionDeclaration> Functions { get; } = [];
    public override IEnumerable<AstNode> Children => Functions;
}