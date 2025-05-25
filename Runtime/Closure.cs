using System.Diagnostics;
using DragoonScript.Core;
using DragoonScript.Core.Ast;
using DragoonScript.Debugging;
using JetBrains.Annotations;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

static class Closure
{
    public static Callable Curry(this Callable inner)
    {
        var max = inner.MaxArgsCount;
        if (max == 1)
        {
            return inner; // already curried
        }

        return new CurriedCallable(inner, []);
    }

    public static Callable FromDelegate(Func<Unit> func)
    {
        return new DelegateCallable((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InterpreterException("Extra arguments.", None);
            }
            return func();
        }, new(new CLRType(typeof(Unit))));
    }

    public static Callable FromDelegate<TResult>(Func<TResult> func)
    {
        return new DelegateCallable((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InterpreterException("Extra arguments.", None);
            }
            return func()!;
        }, new(new CLRType(typeof(Unit))));
    }
    public static Callable FromDelegate<T1, TResult>(Func<T1, TResult> func)
    {
        return new DelegateCallable((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InterpreterException("Extra arguments.", None);
            }
            return func(args[0].ValueCast<T1>())!;
        }, new(new CLRType(typeof(Unit))));
    }
    public static Callable FromDelegate<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return new DelegateCallable((_, args) =>
        {
            return func(args[0].ValueCast<T1>(), args[1].ValueCast<T2>())!;
        }, new(new CLRType(typeof(T1)), new CLRType(typeof(T2))));
    }
    public static Callable FromDelegate<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
    {
        return new DelegateCallable((_, args) =>
        {
            return func(args[0].ValueCast<T1>(), args[1].ValueCast<T2>(), args[2].ValueCast<T3>())!;
        }, new(new CLRType(typeof(T1)), new CLRType(typeof(T2)), new CLRType(typeof(T3))));
    }

    public static Callable FromDelegate<T1, T2, T3, TResult>(Func<Interpreter, T1, T2, T3, TResult> func)
    {
        return new DelegateCallable((interpreter, args) =>
        {
            return func(interpreter, args[0].ValueCast<T1>(), args[1].ValueCast<T2>(), args[2].ValueCast<T3>())!;
        }, new(new CLRType(typeof(T1)), new CLRType(typeof(T2)), new CLRType(typeof(T3))));
    }

    public static Callable FromDeclaration(FunctionDeclaration declaration) => new FunctionCallable(declaration).Curry();
    public static Callable FromLambda(Abstraction abstraction, FunctionScope scope) => new LambdaClosure(abstraction, scope).Curry();

    public static Callable Overloaded(int argsCount, params Callable[] closures)
    {
        var builder = OverloadedCallable.CreateBuilder(argsCount);
        var result = builder.AddRange(closures).Build();

        var result2 = result.Select((string message) => new InterpreterException(message, None));
        return ResultExtensions.Cast<OverloadedCallable, InterpreterException, Exception>(result2).UnwrapOrThrow();
    }
}
