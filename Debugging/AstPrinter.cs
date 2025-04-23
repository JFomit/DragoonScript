using System.Text;
using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Debugging;

class AstPrinter : AstNodeVisitor<string>
{
    public override string VisitApplicationBinding(ApplicationBinding binding)
        => $"(let {Visit(binding.Variable)} = ({Visit(binding.Function)} {string.Join(' ', binding.Arguments.Select(Visit))}) in\n{Visit(binding.Expression.Unwrap())})";
    public override string VisitValueBinding(ValueBinding binding)
        => $"(let {Visit(binding.Variable)} = {Visit(binding.Value)} in\n{Visit(binding.Expression.Unwrap())})";
    public override string VisitVariable(Variable variable) => variable.Name;
    public override string VisitLiteral(Literal literal) => literal.Value;
    public override string VisitFunctionVariable(FunctionVariable functionVariable) => functionVariable.Function.Name;
    public override string VisitIfExpressionBinding(IfExpressionBinding binding)
        => $"(let {Visit(binding.Variable)} = if ({Visit(binding.Condition)}) then ({Visit(binding.Then)}) else ({Visit(binding.Else)}) in\n{Visit(binding.Expression.Unwrap())})";
}
