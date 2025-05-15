using DragoonScript.Core;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class DelegateClosure(Func<Interpreter, object[], object> @delegate, HMClosureType type, string? format = null) : IClosure
{
    public Option<string> Name { get; } = format.ToOption();

    public Func<Interpreter, object[], object> Delegate { get; } = @delegate;
    public int MaxArgsCount { get; } = type.Parameters.Length;

    public HMClosureType Type { get; } = type;

    public object Call(Interpreter interpreter, object[] args)
    {
        if (args.Length > MaxArgsCount)
        {
            throw new InterpreterException("Extra arguments.", Some(Format()));
        }
        if (args.Length < MaxArgsCount)
        {
            throw new InterpreterException("Too few arguments provided.", Some(Format()));
        }
        return Delegate(interpreter, args);
    }

    public string Format() => Name.TryUnwrap(out var name) ? name : "<builtin>";
}
