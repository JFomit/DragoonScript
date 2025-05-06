using System.Diagnostics;
using DragoonScript.Core.Ast;
using DragoonScript.Debugging;
using JetBrains.Annotations;
using JFomit.Functional;

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
                throw new InvalidOperationException("Extra arguments.");
            }
            return func();
        }, 1);
    }

    public static IClosure FromDelegate<TResult>(Func<TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func()!;
        }, 1);
    }
    public static IClosure FromDelegate<T1, TResult>(Func<T1, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func((T1)args[0])!;
        }, 1);
    }
    public static IClosure FromDelegate<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            return func((T1)args[0], (T2)args[1])!;
        }, 2).Curry();
    }
    public static IClosure FromDelegate<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            return func((T1)args[0], (T2)args[1], (T3)args[2])!;
        }, 3).Curry();
    }

    public static IClosure FromDelegate<T1, T2, T3, TResult>(Func<Interpreter, T1, T2, T3, TResult> func)
    {
        return new DelegateClosure((interpreter, args) =>
        {
            return func(interpreter, (T1)args[0], (T2)args[1], (T3)args[2])!;
        }, 3).Curry();
    }

    public static IClosure Loop()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var count = (int)(double)args[0];
            var func = (IClosure)args[1];

            for (int i = 0; i < count; i++)
            {
                func.Call(interpreter, [(double)i]);
            }

            return Unit.Value;
        }, 2).Curry();
    }
    public static IClosure Repeat()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var count = (int)(double)args[0];
            var func = (IClosure)args[1];

            for (int i = 0; i < count; i++)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Unit.Value]);
            }

            return Unit.Value;
        }, 2).Curry();
    }
    public static IClosure InfiniteLoop()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var func = (IClosure)args[0];

            while (true)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Unit.Value]);
            }
        }, 1);
    }
    public static IClosure While()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var cond = (IClosure)args[0];
            var func = (IClosure)args[1];

            while (cond.Call(interpreter, [Unit.Value]) is true)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Unit.Value]);
            }

            return Prelude.Unit;
        }, 2).Curry();
    }

    public static IClosure FromDeclaration(FunctionDeclaration declaration) => new FunctionClosure(declaration).Curry();
    public static IClosure FromLambda(Abstraction abstraction, FunctionScope scope) => new LambdaClosure(abstraction, scope).Curry();
}
