using DragoonScript.Core.Ast;

namespace DragoonScript.Utils;

abstract class AstNodeVisitor<TResult>
{
    protected virtual TResult Default() => default!;
    protected virtual TResult Aggregate(TResult previous, TResult next) => next;
    protected virtual TResult VisitChildren(AstNode node)
    {
        var result = Default();
        foreach (var item in node.Children)
        {
            var next = item.Accept(this);
            result = Aggregate(result, next);
        }

        return result;
    }

    public virtual TResult Visit(AstNode item) => item.Accept<TResult>(this);

    public virtual TResult VisitFile(Core.Ast.File file) => VisitChildren(file);
    public virtual TResult VisitFunctionDeclaration(FunctionDeclaration function) => VisitChildren(function);
    public virtual TResult VisitAbstraction(Abstraction abstraction) => VisitChildren(abstraction);
    public virtual TResult VisitLiteral(Literal literal) => VisitChildren(literal);
    public virtual TResult VisitVariable(Variable variable) => VisitChildren(variable);
    public virtual TResult VisitApplicationBinding(ApplicationBinding binding) => VisitChildren(binding);
    public virtual TResult VisitIfExpressionBinding(IfExpressionBinding binding) => VisitChildren(binding);
    public virtual TResult VisitValueBinding(ValueBinding binding) => VisitChildren(binding);
    public virtual TResult VisitFunctionVariable(FunctionVariable functionVariable) => VisitChildren(functionVariable);
    public virtual TResult VisitHalt(Value value) => VisitChildren(value);
    public virtual TResult VisitJoin(Join join) => Visit(join.Value);
}
