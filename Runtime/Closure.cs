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
    public static IClosure Curry(this IClosure inner)
    {
        var max = inner.MaxArgsCount;
        if (max == 1)
        {
            return inner; // already curried
        }

        return new CurriedClosure(inner, []);
    }

    public static IClosure FromDelegate(Func<Unit> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InterpreterException("Extra arguments.", None);
            }
            return func();
        }, new(new CLRType(typeof(Unit))));
    }

    public static IClosure FromDelegate<TResult>(Func<TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InterpreterException("Extra arguments.", None);
            }
            return func()!;
        }, new(new CLRType(typeof(Unit))));
    }
    public static IClosure FromDelegate<T1, TResult>(Func<T1, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InterpreterException("Extra arguments.", None);
            }
            return func(args[0].ValueCast<T1>())!;
        }, new(new CLRType(typeof(Unit))));
    }
    public static IClosure FromDelegate<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            return func(args[0].ValueCast<T1>(), args[1].ValueCast<T2>())!;
        }, new(new CLRType(typeof(T1)), new CLRType(typeof(T2))));
    }
    public static IClosure FromDelegate<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            return func(args[0].ValueCast<T1>(), args[1].ValueCast<T2>(), args[2].ValueCast<T3>())!;
        }, new(new CLRType(typeof(T1)), new CLRType(typeof(T2)), new CLRType(typeof(T3))));
    }

    public static IClosure FromDelegate<T1, T2, T3, TResult>(Func<Interpreter, T1, T2, T3, TResult> func)
    {
        return new DelegateClosure((interpreter, args) =>
        {
            return func(interpreter, args[0].ValueCast<T1>(), args[1].ValueCast<T2>(), args[2].ValueCast<T3>())!;
        }, new(new CLRType(typeof(T1)), new CLRType(typeof(T2)), new CLRType(typeof(T3))));
    }

    public static IClosure Loop()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var func = args[1].ValueCast<IClosure>();
            var count = args[0].ValueCast<int>();

            for (int i = 0; i < count; i++)
            {
                func.Call(interpreter, [i.ValueCast<int>()]);
            }

            return Prelude.Unit;
        }, new(new CLRType(typeof(int)), new CLRType(typeof(IClosure)))).Curry();
    }
    public static IClosure Repeat()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var func = args[1].ValueCast<IClosure>();
            var count = args[0].ValueCast<int>();

            for (int i = 0; i < count; i++)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Prelude.Unit]);
            }

            return Prelude.Unit;
        }, new(new CLRType(typeof(int)), new CLRType(typeof(IClosure)))).Curry();
    }
    public static IClosure InfiniteLoop()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var func = args[0].ValueCast<IClosure>();

            while (true)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Prelude.Unit]);
            }
        }, new(new CLRType(typeof(IClosure))));
    }
    public static IClosure While()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var cond = args[0].ValueCast<IClosure>();
            var func = args[1].ValueCast<IClosure>();

            while (cond.Call(interpreter, [Prelude.Unit]) is true)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Prelude.Unit]);
            }

            return Prelude.Unit;
        }, new(new CLRType(typeof(IClosure)), new CLRType(typeof(IClosure)))).Curry();
    }

    public static IClosure FromDeclaration(FunctionDeclaration declaration) => new FunctionClosure(declaration).Curry();
    public static IClosure FromLambda(Abstraction abstraction, FunctionScope scope) => new LambdaClosure(abstraction, scope).Curry();

    public static IClosure Overloaded(int argsCount, params IClosure[] closures)
    {
        var builder = OverloadedClosure.CreateBuilder(argsCount);
        var result = builder.AddRange(closures).Build();

        var result2 = result.Select((string message) => new InterpreterException(message, None));
        return ResultExtensions.Cast<OverloadedClosure, InterpreterException, Exception>(result2).UnwrapOrThrow();
    }
}
