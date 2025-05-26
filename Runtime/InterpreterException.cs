using JFomit.Functional.Monads;

namespace DragoonScript.Runtime;

[Serializable]
class InterpreterException(string message, Stack<CallFrame> callFrames) : Exception(message)
{
    public Stack<CallFrame> CallFrames { get; } = callFrames;

    public RuntimeError ToError() => new(Message, CallFrames);
}