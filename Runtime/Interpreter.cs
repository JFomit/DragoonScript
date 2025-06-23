using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using DragoonScript.Core.Ast;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class Interpreter(FunctionScope globals)
{
    public FunctionScope Global { get; } = globals;
    public FunctionScope Current
    {
        get => _current;
        private set => _current = value;
    }
    private FunctionScope _current = globals.Fork();

    public Stack<CallFrame> CallStack { get; } = [];

    public object CallMain(FunctionDeclaration main)
    {
        Enter(new FunctionCallable(main), null!);
        if (main.Parameters.Length > 1)
        {
            HaltAndCatchFire("Main function must not take named parameters, only ().");
        }

        PushScope();
        var callResult = Run(main.Body);
        PopScope();
        var next = Leave();
        Debug.Assert(next is null);

        return callResult;
    }

    private object Run(LambdaTerm expression)
    {
    next:
        switch (expression)
        {
            case ValueBinding binding:
                {
                    Current.DefineUniqueOrFork(binding.Variable.Name, ExtractValue(binding.Value), out _current);
                    expression = binding.Expression.Unwrap();
                    goto next;
                }
            case ApplicationBinding applicationBinding:
                {
                    var function = ExtractValue(applicationBinding.Function).ValueCast<Callable>();
                    var result = applicationBinding.Variable;
                    var args = applicationBinding.Arguments.Select(ExtractValue).ToArray();
                call:
                    switch (function)
                    {
                        case OverloadedCallable overloaded:
                            {
                                if (args.Length > overloaded.MaxArgsCount)
                                {
                                    HaltAndCatchFire("Extra arguments.");
                                }
                                if (args.Length < overloaded.MaxArgsCount)
                                {
                                    HaltAndCatchFire("Not enough arguments provided.");
                                }

                                foreach (var item in overloaded.Closures)
                                {
                                    if (item.Type.IsCallableWith(args))
                                    {
                                        function = item;
                                        goto call;
                                    }
                                }

                                return HaltAndCatchFire($"No function in {overloaded.Format()} is callable with provided arguments: {args.Skip(1).Aggregate(args[0].GetType().Format(), (p, n) => $"{p} -> {n.GetType().Format()}")}");
                            }
                        case CurriedCallable curried:
                            {
                                if (args.Length == curried.Inner.MaxArgsCount) // perfect forwarding
                                {
                                    function = curried.Inner;
                                    args = [.. curried.Bound, .. args];
                                    goto call;
                                }

                                Enter(curried, applicationBinding.Expression.Unwrap());
                                if (args.Length > curried.MaxArgsCount)
                                {
                                    HaltAndCatchFire("Extra arguments.");
                                }
                                if (args.Length < 1)
                                {
                                    HaltAndCatchFire("Not enough arguments provided.");
                                }

                                var callResult = new CurriedCallable(curried.Inner, [.. curried.Bound, .. args]); // partial application
                                expression = Leave();
                                Current.DefineUniqueOrFork(result.Name, callResult, out _current);
                                goto next;
                            }
                        case ImmediateCallable immediate:
                            {
                                Enter(immediate, applicationBinding.Expression.Unwrap());
                                var callResult = immediate.Call(this, args);
                                expression = Leave();
                                Current.DefineUniqueOrFork(result.Name, callResult, out _current);
                                goto next;
                            }
                        case FunctionCallable functionCallable:
                            {
                                var functionDefinition = functionCallable.Function;
                                if (functionDefinition.Parameters.Length < args.Length)
                                {
                                    HaltAndCatchFire("Extra arguments.");
                                }

                                if (applicationBinding.IsTailcall)
                                {
                                    // previous function
                                    PopScope();
                                    var returnTarget = Leave();
                                    // current function
                                    Enter(functionCallable, returnTarget);
                                    PushScope();

                                    var scope = Current;
                                    for (int i = 0; i < args.Length; i++)
                                    {
                                        var ok = scope.DefineUniqueOrFork(functionDefinition.Parameters[i].Name, args[i], out _);
                                        Debug.Assert(ok);
                                    }
                                    expression = functionDefinition.Body;
                                }
                                else
                                {
                                    Enter(functionCallable, applicationBinding.Expression.Unwrap());
                                    PushScope();
                                    var scope = Current;
                                    for (int i = 0; i < args.Length; i++)
                                    {
                                        var ok = scope.DefineUniqueOrFork(functionDefinition.Parameters[i].Name, args[i], out _);
                                        Debug.Assert(ok);
                                    }

                                    var callResult = Run(functionDefinition.Body);
                                    PopScope();

                                    Current.DefineUniqueOrFork(result.Name, callResult, out _current);
                                    expression = Leave();
                                }
                                goto next;
                            }
                        case LambdaClosure lambda:
                            {
                                var abstraction = lambda.Lambda;
                                if (abstraction.Variables.Length < args.Length)
                                {
                                    HaltAndCatchFire("Extra arguments.");
                                }

                                if (applicationBinding.IsTailcall)
                                {
                                    // previous function
                                    PopScope();
                                    var returnTarget = Leave();
                                    // current function
                                    Enter(lambda, returnTarget);
                                    PushScope();

                                    var scope = Current;
                                    for (int i = 0; i < args.Length; i++)
                                    {
                                        var ok = scope.DefineUniqueOrFork(abstraction.Variables[i].Name, args[i], out _);
                                        Debug.Assert(ok);
                                    }
                                    expression = abstraction.Body;
                                }
                                else
                                {
                                    Enter(lambda, applicationBinding.Expression.Unwrap());
                                    var old = PushScope(lambda.Closure);
                                    var scope = Current;
                                    for (int i = 0; i < args.Length; i++)
                                    {
                                        var ok = scope.DefineUniqueOrFork(abstraction.Variables[i].Name, args[i], out _);
                                        Debug.Assert(ok);
                                    }
                                    // no tco by default
                                    var callResult = Run(abstraction.Body);
                                    PopScope(old);

                                    Current.DefineUniqueOrFork(result.Name, callResult, out _current);
                                    expression = Leave();
                                }
                                goto next;
                            }
                        default:
                            return HaltAndCatchFire($"Interpreter discovered an invalid program.");
                    }
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
                    : HaltAndCatchFire($"Variable not in scope: {variable.Name}.");
                }
            case FunctionVariable variable:
                {
                    return Current
                            .Get(variable.Function.Name)
                            .TryUnwrap(out var f)
                        ? f
                        : HaltAndCatchFire($"Function not in scope: {variable.Function.Name}.");
                }
            case Abstraction abstraction:
                {
                    var lambda = Closure.FromLambda(abstraction, Current);
                    return lambda;
                }
            case Literal literal:
                return ExtractLiteral(literal);
            case IfExpressionBinding ifExpression:
                {
                    var result = ifExpression.Variable;
                    var condition = (bool)ExtractValue(ifExpression.Condition);
                    PushScope();
                    expression = condition ? ifExpression.Then : ifExpression.Else;
                    goto next;
                }
        }

        return HaltAndCatchFire("Interpreter discovered an invalid program.");
    }

    private object ExtractValue(Value value) => value switch
    {
        Variable v
            => Current.Get(v.Name).TryUnwrap(out var x)
            ? x
            : (Variable)(object)HaltAndCatchFire($"Variable not in scope: {v.Name}."),
        Literal l => ExtractLiteral(l),
        FunctionVariable f => Current.Get(f.Function.Name).Unwrap(),
        Abstraction lambda => Closure.FromLambda(lambda, Current),

        _ => HaltAndCatchFire($"Not in scope: {value}.")
    };
    private object ExtractLiteral(Literal l)
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
    private string ExtractString(Literal l)
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
                HaltAndCatchFire("Invalid string literal.");
            }

            char next = value[i + 1] switch
            {
                'n' => '\n',
                't' => '\t',
                '\\' => '\\',
                '"' => '"',
                _ => (char)(object)HaltAndCatchFire("Invalid escape sequence.")
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

    [DoesNotReturn]
    public Absurd HaltAndCatchFire(string message) => throw new InterpreterException(message, CallStack);
}
