using JFomit.Functional.Monads;

namespace DragoonScript.Runtime;

class RuntimeError(string message, Stack<CallFrame> callFrames)
{
    public string Message { get; } = message;
    public Stack<CallFrame> CallFrames { get; } = callFrames;
}