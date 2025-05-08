using JFomit.Functional.Monads;

namespace DragoonScript.Runtime;

class RuntimeError(string message, Option<string> function)
{
    public string Message { get; } = message;
    public Option<string> Function { get; } = function;
}