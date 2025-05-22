using DragoonScript.Core;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class CurriedCallable(Callable inner, object[] bound) : Callable
{
    public Callable Inner { get; } = inner;
    public object[] Bound { get; } = bound;

    public override int MaxArgsCount => Inner.MaxArgsCount - Bound.Length;

    public override HMClosureType Type => new(Enumerable.Range(1, MaxArgsCount).Select(_ => new Any()).ToArray());

    public override object Call(Interpreter interpreter, object[] args)
    {
        if (args.Length == MaxArgsCount) // perfect forwarding
        {
            return Inner.Call(interpreter, [.. Bound, .. args]);
        }

        if (args.Length > MaxArgsCount)
        {
            throw new InterpreterException("Extra arguments.", Some(Format()));
        }
        if (args.Length < 1)
        {
            throw new InterpreterException("Not enough arguments provided.", Some(Format()));
        }

        return new CurriedCallable(Inner, [.. Bound, .. args]); // partial application
    }

    public override string Format()
    {
        if (MaxArgsCount == Inner.MaxArgsCount)
        {
            return Inner.Format();
        }

        return $"<partial: {Type.Format()}>";
    }
}
