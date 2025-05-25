using System.Diagnostics;
using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record FunctionCallable(FunctionDeclaration Function) : Callable
{
    public override int MaxArgsCount => Function.Parameters.Length;

    public override HMClosureType Type { get; } = new(Function.Parameters.Select(_ => new Any()).ToArray());

    public OneOf<object, LambdaTerm> Call(Interpreter interpreter, object[] args)
    {
        var scope = interpreter.Current;
        if (Function.Parameters.Length < args.Length)
        {
            throw new InterpreterException("Extra arguments.", Some(Format()));
        }
        for (int i = 0; i < args.Length; i++)
        {
            var ok = scope.DefineUniqueOrFork(Function.Parameters[i].Name, args[i], out _);
            Debug.Assert(ok);
        }

        return Variant(Function.Body);
    }

    public override string Format() => $"<{Function.Name}: {Type.Format()}>";
}
