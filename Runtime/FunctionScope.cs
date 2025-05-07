using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class FunctionScope(Dictionary<string, object> values)
{
    public FunctionScope(Dictionary<string, object> values, FunctionScope parent) : this(values)
    {
        Parent = Some(parent);
    }

    public Option<FunctionScope> Parent { get; }
    public Dictionary<string, object> Values { get; } = values;

    public Option<object> Get(string name)
    {
        if (Values.TryGetValue(name, out var obj))
        {
            return Some(obj);
        }

        return Parent.SelectMany(name, static (p, name) => p.Get(name));
    }

    public bool DefineUniqueOrFork(string name, object value, out FunctionScope nextScope)
    {
        if (Values.ContainsKey(name))
        {
            nextScope = Fork();
            var ok = nextScope.DefineUniqueOrFork(name, value, out _);
            Debug.Assert(ok);
            return false;
        }
        Values[name] = value;
        nextScope = this;
        return true;
    }

    public Option<Unit> Update(string name, object value)
    {
        if (Values.ContainsKey(name))
        {
            Values[name] = value;
            return Some();
        }

        return Parent.SelectMany((name, value), static (p, ctx) => p.Update(ctx.name, ctx.value));
    }

    public FunctionScope Fork() => new([], this);
    public FunctionScope Fork(string name, object value) => new(new()
    {
        [name] = value
    }, this);
    public FunctionScope Fork(KeyValuePair<string, object> x) => new(new()
    {
        [x.Key] = x.Value
    }, this);
    public FunctionScope Fork(KeyValuePair<string, object> x, KeyValuePair<string, object> y) => new(new()
    {
        [x.Key] = x.Value,
        [y.Key] = y.Value
    }, this);
    public FunctionScope Fork(params KeyValuePair<string, object>[] additional) => new(new(additional), this);

    public void Dump()
    {
        foreach (var (key, value) in Values)
        {
            Console.WriteLine($"{key} = {value}");
        }
    }
    public void DumpAll()
    {
        if (Parent.TryUnwrap(out var p))
        {
            Console.WriteLine("Parent:");
            p.DumpAll();
        }

        foreach (var (key, value) in Values)
        {
            Console.WriteLine($"{key} = {value}");
        }
    }
}