using DragoonScript.Core;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class CurriedClosure(IClosure inner, object[] bound) : IClosure
{
    public IClosure Inner { get; } = inner;
    public object[] Bound { get; } = bound;

    public int MaxArgsCount => Inner.MaxArgsCount - Bound.Length;

    public HMClosureType Type => new(Enumerable.Range(1, MaxArgsCount).Select(_ => new Any()).ToArray());

    public object Call(Interpreter interpreter, object[] args)
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

        return new CurriedClosure(Inner, [.. Bound, .. args]); // partial application
    }

    public string Format()
    {
        if (MaxArgsCount == Inner.MaxArgsCount)
        {
            return Inner.Format();
        }

        return $"<partial: {Type.Format()}>";
    }
}
