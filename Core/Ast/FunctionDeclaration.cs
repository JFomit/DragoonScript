
namespace Compiler.Core.Ast;

enum FunctionKind
{
    CallableFunction = 0,
    InfixOperator,
    PrefixOperator
}

record FunctionDeclaration(string Name, AstNode Body) : AstNode
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Body;
        }
    }
}
