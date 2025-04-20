namespace Compiler.Core.Ast;

record Application(AstNode Callable, params AstNode[] Arguments) : AstNode
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Callable;
            foreach (AstNode arg in Arguments)
            {
                yield return arg;
            }
        }
    }
}
