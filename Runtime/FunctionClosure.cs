using DragoonScript.Core.Ast;

namespace DragoonScript.Runtime;

class FunctionClosure(FunctionDeclaration function) : IClosure
{
    public FunctionDeclaration Function { get; } = function;
    public int MaxArgsCount => Function.Parameters.Length;

    public object Call(Interpreter interpreter, object[] args)
    {
        interpreter.PushScope();
        var scope = interpreter.Current;
        for (int i = 0; i < args.Length; i++)
        {
            scope.Define(Function.Parameters[i].Name, args[i]);
        }
        var result = interpreter.Visit(Function);
        interpreter.PopScope();

        return result;
    }
}