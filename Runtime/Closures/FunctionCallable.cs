using System.Diagnostics;
using DragoonScript.Core;
using DragoonScript.Core.Ast;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class FunctionCallable(FunctionDeclaration function) : Callable
{
    public FunctionDeclaration Function { get; } = function;
    public override int MaxArgsCount => Function.Parameters.Length;

    public override HMClosureType Type { get; } = new(function.Parameters.Select(_ => new Any()).ToArray());

    public override object Call(Interpreter interpreter, object[] args)
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
        var result = interpreter.Visit(Function);

        return result;
    }

    public override string Format() => $"<{Function.Name}: {Type.Format()}>";
}
