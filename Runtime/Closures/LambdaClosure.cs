using System.Diagnostics;
using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record LambdaClosure(Abstraction Lambda, FunctionScope Closure) : Callable
{
    public override int MaxArgsCount => Lambda.Variables.Length;

    public override HMClosureType Type { get; } = new(Lambda.Variables.Select(_ => new Any()).ToArray());

    public override string Format() => $"<lambda: {Type.Format()}>";
}