using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record DelegateCallable(Func<Interpreter, object[], object> Delegate, HMClosureType Type, Option<string> Name = default) : Callable
{
    public Func<Interpreter, object[], object> Delegate { get; } = Delegate;
    public override int MaxArgsCount { get; } = Type.Parameters.Length;

    public override HMClosureType Type => Type;

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

    public override string Format() => Name.TryUnwrap(out var name) ? name : "<builtin>";
}
