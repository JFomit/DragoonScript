namespace Compiler.Core.Ast;

record File(List<FunctionDeclaration> Functions) : AstNode
{
    public override IEnumerable<AstNode> Children => Functions;
}