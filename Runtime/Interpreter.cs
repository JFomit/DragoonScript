using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record FunctionScope(Dictionary<string, object> Variables, Option<FunctionScope> Parent = default)
{
    public Option<object> GetVariable(string name) => Variables.GetValue(name).Flatten();
    public Option<Unit> UpdateVariable(string name, object value)
    {
        if (Variables.ContainsKey(name))
        {
            Variables[name] = value;
            return Some();
        }

        return None;
    }
    public void CreateOrUpdate(string name, object value) => Variables[name] = value;
}
class FunctionScopeStack(FunctionScope Root)
{
    private FunctionScope Current = Root;

    public Option<object> GetVariable(string name)
    {
        var current = Current.GetVariable(name);

        return current.TryUnwrap(out var variable)
            ? Some(variable)
            : Current.Parent.SelectMany(name, (scope, name) => scope.GetVariable(name));
    }
    public Option<Unit> Update(string name, object value, bool addIfNotPresent = false)
    {
        var result = Current.UpdateVariable(name, value);
        if (result.TryUnwrap(out _))
        {
            return Some();
        }
        var parentResult = Current.Parent.SelectMany((name, value), (p, tuple) => p.UpdateVariable(tuple.name, tuple.value));
        if (parentResult.IsSome)
        {
            return Some();
        }

        if (addIfNotPresent)
        {
            Current.CreateOrUpdate(name, value);
            return Some();
        }

        return None;
    }
}

class Interpreter(FunctionScope builtInFunctions) : AstNodeVisitor<object>
{
    private readonly FunctionScopeStack Scopes = new(builtInFunctions);

    public override object VisitAbstraction(Abstraction abstraction)
    {
        foreach (var item in abstraction.Variables)
        {
            Scopes.Update(item.Name, 0.0d, true);
        }
        return Visit(abstraction.Expression);
    }

    public override object VisitHalt(Value value) => ExtractValue(value);
    public override object VisitApplicationBinding(ApplicationBinding binding)
    {
        var resultName = binding.Variable.Name;
        var function = (Closure)ExtractValue(binding.Function);
        var args = binding.Arguments.Select(ExtractValue).ToArray();
        Scopes.Update(resultName, function.Call(args), true).Unwrap();
        return Visit(binding.Expression.Unwrap());
    }
    public override object VisitVariable(Variable variable) => ExtractValue(variable);
    public override object VisitLiteral(Literal literal) => ExtractValue(literal);
    public override object VisitIfExpressionBinding(IfExpressionBinding binding)
    {
        var resultName = binding.Variable.Name;
        var condition = Visit(binding.Condition);

        Scopes.Update(resultName, condition switch
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
        Scopes.Update(variableName, value, true);
        return Visit(binding.Expression.Unwrap());
    }

    private object ExtractValue(Value value) => value switch
    {
        Literal l => ExtractLiteral(l),
        Variable v => Scopes
            .GetVariable(v.Name)
            .Expect($"Variable not is scope: {v.Name}."),
        FunctionVariable f => Scopes
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
