using System.Diagnostics.CodeAnalysis;
using DragoonScript.Core;
using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;

namespace DragoonScript.Runtime;

abstract record Callable
{
    public abstract HMClosureType Type { get; }
    public abstract int MaxArgsCount { get; }

    public abstract string Format();
}

abstract record ImmediateCallable : Callable
{
    public abstract object Call(Interpreter interpreter, object[] args);
}