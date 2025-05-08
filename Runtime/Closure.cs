using System.Diagnostics;
using DragoonScript.Core.Ast;
using DragoonScript.Debugging;
using JetBrains.Annotations;
using JFomit.Functional;
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
        }, 1);
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
        }, 1);
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
        }, 1);
    }
    public static IClosure FromDelegate<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            return func(args[0].ValueCast<T1>(), args[1].ValueCast<T2>())!;
        }, 2).Curry();
    }
    public static IClosure FromDelegate<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            return func(args[0].ValueCast<T1>(), args[1].ValueCast<T2>(), args[2].ValueCast<T3>())!;
        }, 3).Curry();
    }

    public static IClosure FromDelegate<T1, T2, T3, TResult>(Func<Interpreter, T1, T2, T3, TResult> func)
    {
        return new DelegateClosure((interpreter, args) =>
        {
            return func(interpreter, args[0].ValueCast<T1>(), args[1].ValueCast<T2>(), args[2].ValueCast<T3>())!;
        }, 3).Curry();
    }

    public static IClosure Loop()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var func = args[1].ValueCast<IClosure>();
            var count = (int)args[0].ValueCast<double>();

            for (int i = 0; i < count; i++)
            {
                func.Call(interpreter, [i.ValueCast<double>()]);
            }

            return Prelude.Unit;
        }, 2).Curry();
    }
    public static IClosure Repeat()
    {
        return new DelegateClosure((interpreter, args) =>
        {
            var func = args[1].ValueCast<IClosure>();
            var count = (int)args[0].ValueCast<double>();

            for (int i = 0; i < count; i++)
            {
                Debug.Assert(func.MaxArgsCount > 0);
                func.Call(interpreter, [Prelude.Unit]);
            }

            return Prelude.Unit;
        }, 2).Curry();
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
        }, 1);
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
        }, 2).Curry();
    }

    public static IClosure FromDeclaration(FunctionDeclaration declaration) => new FunctionClosure(declaration).Curry();
    public static IClosure FromLambda(Abstraction abstraction, FunctionScope scope) => new LambdaClosure(abstraction, scope).Curry();
}
