using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;

namespace DragoonScript.Runtime;

class VM
{
    public Dictionary<string, VMValue> Variables { get; } = [];

    public Dictionary<string, VMFunction> Functions { get; } = new()
    {
        ["+"] = new("+", FunctionKind.InfixOperator, static (Memory<VMValue> argsStruct) =>
        {
            var args = argsStruct.Span;
            if (args.Length != 2)
            {
                throw new InvalidOperationException();
            }

            if (args[0].TryGetInteger(out var a) && args[1].TryGetInteger(out var b))
            {
                return new VMValue(Kind.Number, a + b);
            }

            throw new InvalidOperationException();
        }),
        ["print"] = new("print", FunctionKind.CallableFunction, static (Memory<VMValue> argsStruct) =>
        {
            var args = argsStruct.Span;
            if (args.Length != 1)
            {
                throw new InvalidOperationException();
            }

            if (args[0].TryGetInteger(out var a))
            {
                Console.WriteLine(a);
                return new VMValue(Kind.Unit, Prelude.Unit);
            }

            throw new InvalidOperationException();
        })
    };

    public Option<VMFunction> GetFunction(string name) => Functions.GetValue(name).Flatten();
    public Option<VMFunction> GetFunction(Value value)
    {
        return value switch
        {
            Variable v => GetFunction(v.Name),
            Literal l => GetFunction(l.Value),
            FunctionVariable f => GetFunction(f.Function.Name),

            _ => throw new InvalidOperationException()
        };
    }
}

record VMFunction(string Name, FunctionKind Kind, Func<Memory<VMValue>, VMValue> Body);

enum Kind
{
    Number,
    Boolean,
    Unit,
    Function,
}

record VMValue(Kind Kind, object Value)
{
    public bool TryGetInteger(out double value) => TryGet(Kind.Number, out value);
    public bool TryGetBoolean(out bool value) => TryGet(Kind.Boolean, out value);
    public bool TryGetUnit(out Unit value) => TryGet(Kind.Unit, out value);
    public bool TryGetUnit(out VMFunction value) => TryGet(Kind.Function, out value);

    public VMFunction GetFunction() => Get<VMFunction>();

    private bool TryGet<T>(Kind kind, out T value)
    {
        if (Kind == kind)
        {
            value = (T)Value;
            return true;
        }
        value = default!;
        return false;
    }
    private T Get<T>() => (T)Value;
}