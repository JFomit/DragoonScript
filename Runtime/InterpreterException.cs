namespace DragoonScript.Runtime;

[Serializable]
class InterpreterException(string message, string function) : Exception(message)
{
    public string Function { get; set; } = function;

    public RuntimeError ToError() => new(Message, Function);
}