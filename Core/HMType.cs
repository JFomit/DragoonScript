using System.Diagnostics;
using DragoonScript.Runtime;

namespace DragoonScript.Core;

abstract class HMType
{
    public abstract bool IsOfType(object obj);
    public abstract bool IsEqualTo(HMType other);
    public virtual bool IsConvertibelTo(HMType other) => IsEqualTo(other);

    public abstract string Format();
}

class Any : HMType
{
    public override string Format() => "any";
    public override bool IsEqualTo(HMType other) => other is Any;
    public override bool IsOfType(object obj) => true;
}

class CLRType(Type type) : HMType
{
    public Type Type { get; } = type;

    public override string Format() => Type.Name;
    public override bool IsEqualTo(HMType other) => other is CLRType clr && clr.Type == Type;
    public override bool IsOfType(object obj) => obj.GetType() == Type;
}

[DebuggerDisplay("Format(),nq")]
class HMClosureType(params HMType[] @params) : HMType
{
    public HMType[] Parameters { get; } = @params;

    public override bool IsEqualTo(HMType other) => other is HMClosureType closureType && closureType.Parameters.SequenceEqual(Parameters);
    public override bool IsOfType(object obj) => obj is IClosure closure && IsEqualTo(closure.Type);

    public bool IsCallableWith(object[] args)
    {
        if (args.Length != Parameters.Length)
        {
            return false;
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (!Parameters[i].IsOfType(args[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override string Format() => Parameters switch
    {
    [] => throw new InvalidOperationException("Void accepting function."),
    [var single] => single.Format(),
    [var first, var second] => $"{first.Format()} -> {second.Format()}",
    [var first, .. var rest] => rest.Aggregate(first.Format(), (p, n) => $"{p} -> {n.Format()}")
    } + " -> any";
}