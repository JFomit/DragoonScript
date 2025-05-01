using DragoonScript.Core.Ast;
using JFomit.Functional;

namespace DragoonScript.Runtime;

class Closure(Func<Interpreter, object[], object> function)
{
    public Func<Interpreter, object[], object> Function { get; } = function;

    public object Call(Interpreter interpreter, object[] args) => Function(interpreter, args);

    public static Closure FromDelegate(Func<Unit> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 0)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func();
        });
    }

    public static Closure FromDelegate<TResult>(Func<TResult> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 0)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func()!;
        });
    }
    public static Closure FromDelegate<T1, TResult>(Func<T1, TResult> func)
    {
        return new((_, args) =>
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
        });
    }
    public static Closure FromDelegate<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 2)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            if (args.Length == 0)
            {
                throw new InvalidOperationException("Too few arguments provided.");
            }
            if (args.Length == 1)
            {
                return FromDelegate<T2, TResult>(next =>
                {
                    return func((T1)args[0], next);
                });
            }
            return func((T1)args[0], (T2)args[1])!;
        });
    }

    public static Closure FromDeclaration(FunctionDeclaration declaration)
    {
        return new((interpreter, args) =>
        {
            var i = new Interpreter(interpreter.Global.Current);
            return i.Visit(declaration);
        });
    }
}