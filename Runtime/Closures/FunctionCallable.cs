using System.Diagnostics;
using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record FunctionCallable(FunctionDeclaration Function) : Callable
{
    public override int MaxArgsCount => Function.Parameters.Length;

    public override HMClosureType Type { get; } = new(Function.Parameters.Select(_ => new Any()).ToArray());

    public override string Format() => $"<{Function.Name}: {Type.Format()}>";
}
