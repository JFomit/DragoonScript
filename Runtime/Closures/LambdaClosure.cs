using System.Diagnostics;
using DragoonScript.Core;
using DragoonScript.Core.Ast;

namespace DragoonScript.Runtime;

class LambdaClosure(Abstraction lambda, FunctionScope currentScope) : IClosure
{
    public Abstraction Lambda { get; } = lambda;
    FunctionScope Closure { get; } = currentScope;

    public int MaxArgsCount => Lambda.Variables.Length;

    public HMClosureType Type { get; } = new(lambda.Variables.Select(_ => new Any()).ToArray());

    public object Call(Interpreter interpreter, object[] args)
    {
        var old = interpreter.PushScope(Closure);
        var scope = interpreter.Current;

        for (int i = 0; i < args.Length; i++)
        {
            var ok = scope.DefineUniqueOrFork(Lambda.Variables[i].Name, args[i], out _);
            Debug.Assert(ok);
        }
        var result = interpreter.Visit(Lambda.Body);
        interpreter.PopScope(old);

        return result;
    }

    public string Format() => $"<lambda: {Type.Format()}>";
}