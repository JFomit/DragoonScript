using JFomit.Functional.Monads;

namespace DragoonScript.Runtime;

[Serializable]
class InterpreterException(string message, Option<string> function) : Exception(message)
{
    public Option<string> Function { get; set; } = function;

    public RuntimeError ToError() => new(Message, Function);
}