namespace DragoonScript.Runtime;

class CurriedClosure(IClosure inner, object[] bound) : IClosure
{
    public IClosure Inner { get; } = inner;
    public object[] Bound { get; } = bound;

    public int MaxArgsCount => Inner.MaxArgsCount - Bound.Length;

    public object Call(Interpreter interpreter, object[] args)
    {
        if (args.Length == MaxArgsCount) // perfect forwarding
        {
            return Inner.Call(interpreter, [.. Bound, .. args]);
        }

        if (args.Length > MaxArgsCount)
        {
            throw new InvalidOperationException("Extra arguments.");
        }
        if (args.Length < 1)
        {
            throw new InvalidOperationException("Not enough arguments provided.");
        }

        return new CurriedClosure(Inner, [.. Bound, .. args]); // partial application
    }
}
