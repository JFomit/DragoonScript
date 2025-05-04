using System.Runtime.InteropServices;
using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class Interpreter(FunctionScope globals) : AstNodeVisitor<object>
{
    public FunctionScope Global { get; } = globals;
    public FunctionScope Current { get; private set; } = globals.Fork();

    public override object VisitValueBinding(ValueBinding binding)
    {
        var result = binding.Variable;
        Current.Define(result.Name, ExtractValue(binding.Value));
        return Visit(binding.Expression.Unwrap());
    }
    public override object VisitApplicationBinding(ApplicationBinding binding)
    {
        var result = binding.Variable;
        var function = (IClosure)Visit(binding.Function);
        var callResult = function.Call(this, binding.Arguments.Select(ExtractValue).ToArray());
        Current.Define(result.Name, callResult);
        return Visit(binding.Expression.Unwrap());
    }
    public override object VisitVariable(Variable variable) => Current.Get(variable.Name).Expect($"Variable not in scope: {variable.Name}.");
    public override object VisitFunctionVariable(FunctionVariable variable) => Current.Get(variable.Function.Name).Expect($"Function not in scope: {variable.Function.Name}.");

    private object ExtractValue(Value value) => value switch
    {
        Variable v => Current.Get(v.Name).Unwrap(),
        Literal l => ExtractLiteral(l),
        FunctionVariable f => Current.Get(f.Function.Name).Unwrap(),

        _ => throw new InvalidOperationException($"Not in scope: {value}.")
    };
    public override object VisitLiteral(Literal literal) => ExtractLiteral(literal);
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
        Current = Current.Fork();
    }
    public void PopScope()
    {
        Current = Current.Parent.Unwrap();
    }
}
