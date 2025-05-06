namespace DragoonScript.Runtime;

class DelegateClosure(Func<Interpreter, object[], object> @delegate, int argsCount) : IClosure
{
    public Func<Interpreter, object[], object> Delegate { get; } = @delegate;
    public int MaxArgsCount { get; } = argsCount;

    public object Call(Interpreter interpreter, object[] args)
    {
        if (args.Length > MaxArgsCount)
        {
            throw new InvalidOperationException("Extra arguments.");
        }
        if (args.Length < MaxArgsCount)
        {
            throw new InvalidOperationException("Too few arguments provided.");
        }
        return Delegate(interpreter, args);
    }
}
