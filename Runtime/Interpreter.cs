using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class Interpreter(FunctionScope builtInFunctions) : AstNodeVisitor<object>
{
    public FunctionScope Global { get; } = builtInFunctions;

    public override object VisitAbstraction(Abstraction abstraction)
    {
        foreach (var item in abstraction.Variables)
        {
            Global.Update(item.Name, 0.0d, true);
        }
        return Visit(abstraction.Expression);
    }

    public override object VisitHalt(Value value) => ExtractValue(value);
    public override object VisitApplicationBinding(ApplicationBinding binding)
    {
        var resultName = binding.Variable.Name;
        var function = (Closure)ExtractValue(binding.Function);
        var args = binding.Arguments.Select(ExtractValue).ToArray();
        Global.Update(resultName, function.Call(this, args), true).Unwrap();
        return Visit(binding.Expression.Unwrap());
    }

    public override object VisitVariable(Variable variable) => ExtractValue(variable);
    public override object VisitLiteral(Literal literal) => ExtractValue(literal);
    public override object VisitIfExpressionBinding(IfExpressionBinding binding)
    {
        var resultName = binding.Variable.Name;
        var condition = Visit(binding.Condition);

        Global.Update(resultName, condition switch
        {
            true => Visit(binding.Then),
            false => Visit(binding.Else),

            _ => throw new InvalidOperationException("Value is not a boolean.")
        }, true);

        return Visit(binding.Expression.Unwrap());
    }

    public override object VisitValueBinding(ValueBinding binding)
    {
        var variableName = binding.Variable.Name;
        var value = ExtractValue(binding.Value);
        Global.Update(variableName, value, true);
        return Visit(binding.Expression.Unwrap());
    }

    private object ExtractValue(Value value) => value switch
    {
        Literal l => ExtractLiteral(l),
        Variable v => Global
            .GetVariable(v.Name)
            .Expect($"Variable not is scope: {v.Name}."),
        FunctionVariable f => Global
            .GetVariable(f.Function.Name)
            .Expect($"Function not is scope: {f.Function.Name}."),

        _ => throw new InvalidOperationException("Invalid value.")
    };
    private static object ExtractLiteral(Literal l)
    {
        if (double.TryParse(l.Value, out var d))
        {
            return d;
        }
        if (l.Value == "()")
        {
            return Prelude.Unit;
        }

        return l.Value.Replace("\\\"", "\"")[1..^1];
    }
}
