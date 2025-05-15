using System.Diagnostics;
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
        var function = Visit(binding.Function).ValueCast<IClosure>();
        var expression = binding.Expression.Unwrap();
        var args = binding.Arguments.Select(ExtractValue).ToArray();

        // if (expression is Variable variable && variable.Name == result.Name)
        // {

        // }

        var callResult = function.Call(this, args);
        Current.DefineUniqueOrFork(result.Name, callResult, out _current);
        return Visit(expression);
    }
    public override object VisitVariable(Variable variable)
        => Current.Get(variable.Name).TryUnwrap(out var v)
        ? v
        : throw new InterpreterException($"Variable not in scope: {variable.Name}.", None);

    public override object VisitFunctionVariable(FunctionVariable variable)
        => Current.Get(variable.Function.Name).TryUnwrap(out var f)
        ? f
        : throw new InterpreterException($"Function not in scope: {variable.Function.Name}.", None);

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
        Variable v
            => Current.Get(v.Name).TryUnwrap(out var x)
            ? x
            : throw new InterpreterException($"Variable not in scope: {v.Name}.", None),
        Literal l => ExtractLiteral(l),
        FunctionVariable f => Current.Get(f.Function.Name).Unwrap(),
        Abstraction lambda => Closure.FromLambda(lambda, Current),

        _ => throw new InterpreterException($"Not in scope: {value}.", None)
    };
    public override object VisitLiteral(Literal literal) => ExtractLiteral(literal);
    private static object ExtractLiteral(Literal l)
    {
        if (int.TryParse(l.Value, out var i))
        {
            return i;
        }
        if (double.TryParse(l.Value, out var d))
        {
            return d;
        }
        if (l.Value == "()")
        {
            return Prelude.Unit;
        }

        return ExtractString(l);
    }
    private static string ExtractString(Literal l)
    {
        var value = l.Value.AsSpan()[1..^1];
        var buffer = value.Length > 50 ? new char[value.Length] : stackalloc char[value.Length];
        int offset = 0;

        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] != '\\')
            {
                buffer[i - offset] = value[i];
                continue;
            }

            if (i + 1 >= value.Length)
            {
                throw new InterpreterException("Invalid string literal.", None);
            }

            char next = value[i + 1] switch
            {
                'n' => '\n',
                't' => '\t',
                '\\' => '\\',
                '"' => '"',
                _ => throw new InterpreterException("Invalid escape sequence.", None)
            };
            buffer[i - offset] = next;
            offset++;
            i++;
        }

        return buffer.ToString();
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
