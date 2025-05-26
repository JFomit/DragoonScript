using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record DelegateCallable(Func<Interpreter, object[], object> Delegate, HMClosureType Type, Option<string> Name = default) : ImmediateCallable
{
    public Func<Interpreter, object[], object> Delegate { get; } = Delegate;
    public override int MaxArgsCount { get; } = Type.Parameters.Length;

    public override HMClosureType Type { get; } = Type;

    public override object Call(Interpreter interpreter, object[] args)
    {
        if (args.Length > MaxArgsCount)
        {
            interpreter.ErrorAndThrow("Extra arguments.");
        }
        if (args.Length < MaxArgsCount)
        {
            interpreter.ErrorAndThrow("Too few arguments provided.");
        }
        return Delegate(interpreter, args);
    }

    public override string Format() => Name.TryUnwrap(out var name) ? name : "<builtin>";
}
