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

    public Option<object> GetValue(string name)
    {
        if (Values.TryGetValue(name, out var obj))
        {
            return Some(obj);
        }

        return Parent.SelectMany(name, (p, name) => p.GetValue(name));
    }
}