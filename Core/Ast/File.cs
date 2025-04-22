using DragoonScript.Utils;

namespace DragoonScript.Core.Ast;

record File(List<FunctionDeclaration> Functions) : AstNode
{
    public override IEnumerable<AstNode> Children => Functions;
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitFile(this);
}