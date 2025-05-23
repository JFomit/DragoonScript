using DragoonScript.Core;
using DragoonScript.Core.Ast;

namespace DragoonScript.Runtime;

abstract class Callable
{
    public abstract HMClosureType Type { get; }

    public abstract object Call(Interpreter interpreter, object[] args);
    public abstract int MaxArgsCount { get; }

    public abstract string Format();

    public virtual bool IsImmediate(int count) => false;
}
