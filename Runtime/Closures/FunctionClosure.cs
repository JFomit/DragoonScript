using System.Diagnostics;
using DragoonScript.Core.Ast;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class FunctionClosure(FunctionDeclaration function) : IClosure
{
    public FunctionDeclaration Function { get; } = function;
    public int MaxArgsCount => Function.Parameters.Length;

    public object Call(Interpreter interpreter, object[] args)
    {
        interpreter.PushScope();
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
        interpreter.PopScope();

        return result;
    }

    public string Format() => $"{Function.Name}: {Function.Parameters.Aggregate("obj", (p, n) => $"{p} -> obj")}";
}
