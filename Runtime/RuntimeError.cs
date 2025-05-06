namespace DragoonScript.Runtime;

class RuntimeError
{
    public RuntimeError(string message, string function)
    {
        Message = message;
        Function = function;
    }

    public string Message { get; }
    public string Function { get; }
}