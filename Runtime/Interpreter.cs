using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class Interpreter(FunctionScope builtInFunctions) : AstNodeVisitor<object>
{
    public FunctionScope Global { get; private set; } = builtInFunctions;
    public FunctionScope Scope { get; private set; } = builtInFunctions;

    public override object VisitAbstraction(Abstraction abstraction)
    {
        foreach (var item in abstraction.Variables)
        {
            Scope.UpdateWithShadow(item.Name, 0.0d);
        }
        return Visit(abstraction.Expression);
    }

    public override object VisitHalt(Value value) => ExtractValue(value);
    public override object VisitApplicationBinding(ApplicationBinding binding)
    {
        var resultName = binding.Variable.Name;
        var function = (Closure)ExtractValue(binding.Function);
        var args = binding.Arguments.Select(ExtractValue).ToArray();
        Scope.UpdateWithShadow(resultName, function.Call(this, args));
        return Visit(binding.Expression.Unwrap());
    }

    public override object VisitVariable(Variable variable) => ExtractValue(variable);
    public override object VisitLiteral(Literal literal) => ExtractValue(literal);
    public override object VisitIfExpressionBinding(IfExpressionBinding binding)
    {
        var resultName = binding.Variable.Name;
        var condition = Visit(binding.Condition);

        PushScope();
        Scope.UpdateOrAddValue(resultName, condition switch
        {
            true => Visit(binding.Then),
            false => Visit(binding.Else),
            _ => throw new InvalidOperationException("Value is not a boolean.")
        });
        PopScope();

        return Visit(binding.Expression.Unwrap());
    }

    public override object VisitValueBinding(ValueBinding binding)
    {
        var variableName = binding.Variable.Name;
        var value = ExtractValue(binding.Value);
        Scope.UpdateWithShadow(variableName, value);
        return Visit(binding.Expression.Unwrap());
    }

    private object ExtractValue(Value value) => value switch
    {
        Literal l => ExtractLiteral(l),
        Variable v => Scope
            .GetValue(v.Name)
            .Expect($"Variable not is scope: {v.Name}."),
        FunctionVariable f => Scope
            .GetValue(f.Function.Name)
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

    public void PushScope()
    {
        Scope = Scope.Fork();
    }
    public void PopScope()
    {
        Scope = Scope.Parent.Unwrap();
    }
}
