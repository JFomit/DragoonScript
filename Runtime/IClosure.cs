using DragoonScript.Core;

namespace DragoonScript.Runtime;

interface IClosure
{
    HMClosureType Type { get; }

    object Call(Interpreter interpreter, object[] args);
    int MaxArgsCount { get; }

    public string Format();
}
