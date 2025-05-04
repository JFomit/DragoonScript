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
            if (args.Length > 0)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func();
        }, 0);
    }

    public static IClosure FromDelegate<TResult>(Func<TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 0)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func()!;
        }, 0);
    }
    public static IClosure FromDelegate<T1, TResult>(Func<T1, TResult> func)
    {
        return new DelegateClosure((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            if (args.Length == 0)
            {
                throw new InvalidOperationException("Too few arguments provided.");
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

    public static IClosure FromDeclaration(FunctionDeclaration declaration) => new FunctionClosure(declaration).Curry();
    public static IClosure FromLambda(Abstraction abstraction, FunctionScope scope) => new LambdaClosure(abstraction, scope).Curry();
}
