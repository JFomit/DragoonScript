using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;

namespace DragoonScript.Runtime.Interpreter;

class ExecutionVisitor(VM vm) : AstNodeVisitor<VMValue>
{
    public VM Vm { get; } = vm;

    public override VMValue VisitApplicationBinding(ApplicationBinding binding)
    {
        var args = binding.Arguments.Select(Visit).ToArray();
        var function = Vm.GetFunction(binding.Function).Unwrap();//.UnwrapOr(Visit(binding.Function).GetFunction());
        var result = function.Body.Invoke(args);

        Vm.Variables.Add(binding.Variable.Name, result);
        return Visit(binding.Expression.Unwrap());
    }

    public override VMValue VisitLiteral(Literal literal)
    {
        if (double.TryParse(literal.Value, out var value))
        {
            return new VMValue(Kind.Number, value);
        }

        return new VMValue(Kind.Function, Vm.GetFunction(literal.Value).Unwrap());
    }
}