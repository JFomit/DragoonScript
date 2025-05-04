namespace DragoonScript.Runtime;

interface IClosure
{
    object Call(Interpreter interpreter, object[] args);
    int MaxArgsCount { get; }
}
