using JFomit.Functional;

namespace DragoonScript.Runtime;

class Closure(Func<object[], object> function)
{
    public Func<object[], object> Function { get; } = function;

    public object Call(object[] args) => Function(args);

    public static Closure FromDelegate(Func<Unit> func)
    {
        return new(args =>
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
        return new(args =>
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
        return new(args =>
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
        return new(args =>
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
}