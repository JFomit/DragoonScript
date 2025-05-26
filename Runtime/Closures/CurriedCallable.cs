using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record CurriedCallable(Callable Inner, object[] Bound) : Callable
{
    public override int MaxArgsCount => Inner.MaxArgsCount - Bound.Length;

    public override HMClosureType Type => new(Enumerable.Range(1, MaxArgsCount).Select(_ => new Any()).ToArray());

    public override string Format()
    {
        if (MaxArgsCount == Inner.MaxArgsCount)
        {
            return Inner.Format();
        }

        return $"<partial: {Type.Format()}>";
    }
}
