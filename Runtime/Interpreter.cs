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
    public FunctionScope Current
    {
        get => _current;
        private set => _current = value;
    }
    private FunctionScope _current = globals.Fork();

    public override object VisitValueBinding(ValueBinding binding)
    {
        var result = binding.Variable;
        Current.DefineUniqueOrFork(result.Name, ExtractValue(binding.Value), out _current);
        return Visit(binding.Expression.Unwrap());
    }
    public override object VisitApplicationBinding(ApplicationBinding binding)
    {
        var result = binding.Variable;
        var function = (IClosure)Visit(binding.Function);
        var expression = binding.Expression.Unwrap();
        var args = binding.Arguments.Select(ExtractValue).ToArray();

        // if (expression is Variable variable && variable.Name == result.Name)
        // {

        // }

        var callResult = function.Call(this, args);
        Current.DefineUniqueOrFork(result.Name, callResult, out _current);
        return Visit(expression);
    }
    public override object VisitVariable(Variable variable) => Current.Get(variable.Name).Expect($"Variable not in scope: {variable.Name}.");
    public override object VisitFunctionVariable(FunctionVariable variable) => Current.Get(variable.Function.Name).Expect($"Function not in scope: {variable.Function.Name}.");

    public override object VisitIfExpressionBinding(IfExpressionBinding binding)
    {
        var result = binding.Variable;
        var condition = (bool)ExtractValue(binding.Condition);
        if (condition)
        {
            PushScope();
            var then = Visit(binding.Then);
            PopScope();
            Current.DefineUniqueOrFork(result.Name, then, out _current);
        }
        else
        {
            PushScope();
            var @else = Visit(binding.Else);
            PopScope();
            Current.DefineUniqueOrFork(result.Name, @else, out _current);
        }

        return Visit(binding.Expression.Unwrap());
    }
    public override object VisitAbstraction(Abstraction abstraction)
    {
        var lambda = Closure.FromLambda(abstraction, Current);
        return lambda;
    }

    private object ExtractValue(Value value) => value switch
    {
        Variable v => Current.Get(v.Name).Expect($"Variable not in scope: {v.Name}."),
        Literal l => ExtractLiteral(l),
        FunctionVariable f => Current.Get(f.Function.Name).Unwrap(),
        Abstraction lambda => Closure.FromLambda(lambda, Current),

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

    public FunctionScope PushScope(FunctionScope scope)
    {
        var old = Current;
        Current = scope.Fork();
        return old;
    }
    public void PopScope(FunctionScope old)
    {
        Current = old;
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
