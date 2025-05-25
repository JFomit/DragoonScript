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

    public Stack<CallFrame> CallStack { get; } = [];

    public object Run(LambdaTerm start)
    {
        var expression = start;

    next:
        switch (expression)
        {
            case ValueBinding binding:
                {
                    var result = binding.Variable;
                    Current.DefineUniqueOrFork(result.Name, ExtractValue(binding.Value), out _current);
                    expression = binding.Expression.Unwrap();
                    goto next;
                }
            case IfExpressionBinding ifExpression:
                {
                    var result = ifExpression.Variable;
                    var condition = (bool)ExtractValue(ifExpression.Condition);
                    PushScope();
                    expression = condition ? ifExpression.Then : ifExpression.Else;
                    goto next;
                }
            case Join join:
                {
                    var result = ExtractValue(join.Value);
                    PopScope();
                    Current.DefineUniqueOrFork(join.Variable.Name, result, out _current);
                    expression = join.JoinTarget.Unwrap();
                    goto next;
                }
            case Variable variable:
                {
                    return Current
                        .Get(variable.Name)
                        .TryUnwrap(out var v)
                    ? v
                    : throw new InterpreterException($"Variable not in scope: {variable.Name}.", None);
                }
            case FunctionVariable variable:
                {
                    return Current
                            .Get(variable.Function.Name)
                            .TryUnwrap(out var f)
                        ? f
                        : throw new InterpreterException($"Function not in scope: {variable.Function.Name}.", None);
                }
            case Abstraction abstraction:
                {
                    var lambda = Closure.FromLambda(abstraction, Current);
                    return lambda;
                }
            case Literal literal:
                return ExtractLiteral(literal);
            case ApplicationBinding application:
                {
                    var result = application.Variable;
                    var function = ExtractValue(application.Function).ValueCast<Callable>();
                    var args = application.Arguments.Select(ExtractValue).ToArray();

                call:
                    switch (function)
                    {
                        case DelegateCallable delegateCallable:
                            {
                                Enter(delegateCallable, application.Expression.Unwrap());
                                var callResult = delegateCallable.Call(this, args);
                                expression = Leave();
                                Current.DefineUniqueOrFork(result.Name, callResult, out _current);

                                goto next;
                            }
                        case LambdaClosure lambdaClosure:
                            {
                                var old = PushScope(lambdaClosure.Closure);
                                var scope = Current;

                                for (int i = 0; i < args.Length; i++)
                                {
                                    var ok = scope.DefineUniqueOrFork(lambdaClosure.Lambda.Variables[i].Name, args[i], out _);
                                    Debug.Assert(ok);
                                }
                                Enter(lambdaClosure, application.Expression.Unwrap());
                                expression = lambdaClosure.Lambda.Body;
                                PopScope(old);

                                goto next;
                            }
                        case CurriedCallable curriedCallable:
                            {
                                if (args.Length == curriedCallable.MaxArgsCount) // perfect forwarding
                                {
                                    args = [.. curriedCallable.Bound, .. args];
                                    function = curriedCallable.Inner;
                                    goto call;
                                }

                                if (args.Length > curriedCallable.MaxArgsCount)
                                {
                                    throw new InterpreterException("Extra arguments.", Some(curriedCallable.Format()));
                                }
                                if (args.Length < 1)
                                {
                                    throw new InterpreterException("Not enough arguments provided.", Some(curriedCallable.Format()));
                                }

                                // partial application
                                var callResult = new CurriedCallable(curriedCallable.Inner, [.. curriedCallable.Bound, .. args]);
                                Current.DefineUniqueOrFork(result.Name, callResult, out _current);
                                goto next;
                            }
                        default:
                            throw new UnreachableException();
                    }
                }
            default:
                throw new InterpreterException("Interpreter discovered an invalid program.", None);
        }
    }

    public override object VisitValueBinding(ValueBinding binding) => Run(binding);
    public override object VisitApplicationBinding(ApplicationBinding binding) => Run(binding);
    public override object VisitVariable(Variable variable) => Run(variable);

    public override object VisitFunctionVariable(FunctionVariable variable) => Run(variable);

    public override object VisitIfExpressionBinding(IfExpressionBinding binding) => Run(binding);
    public override object VisitAbstraction(Abstraction abstraction) => Run(abstraction);

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
    public override object VisitLiteral(Literal literal) => Run(literal);
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

        // char
        if (l.Value.StartsWith('\''))
        {
            if (l.Value.AsSpan()[1..^1] == "\\'")
            {
                return '\'';
            }
            else
            {
                return l.Value[1];
            }
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


    private void Enter(Callable callable, LambdaTerm returnTarget) => CallStack.Push(new CallFrame(returnTarget, callable));
    private LambdaTerm Leave() => CallStack.Pop().ReturnTarget;
}
