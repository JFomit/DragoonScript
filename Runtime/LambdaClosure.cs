using DragoonScript.Core.Ast;

namespace DragoonScript.Runtime;

class LambdaClosure(Abstraction lambda, FunctionScope currentScope) : IClosure
{
    public Abstraction Lambda { get; } = lambda;
    FunctionScope Closure { get; } = currentScope;

    public int MaxArgsCount => Lambda.Variables.Length;

    public object Call(Interpreter interpreter, object[] args)
    {
        var old = interpreter.PushScope(Closure);
        var scope = interpreter.Current;
        for (int i = 0; i < args.Length; i++)
        {
            scope.Define(Lambda.Variables[i].Name, args[i]);
        }
        var result = interpreter.Visit(Lambda.Body);
        interpreter.PopScope(old);

        return result;
    }
}