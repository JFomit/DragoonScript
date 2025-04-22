
using DragoonScript.Utils;

namespace DragoonScript.Core.Ast;

enum FunctionKind
{
    CallableFunction = 0,
    InfixOperator,
    PrefixOperator
}

record FunctionDeclaration(string Name, LambdaTerm Body) : AstNode
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Body;
        }
    }
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitFunctionDeclaration(this);
}
