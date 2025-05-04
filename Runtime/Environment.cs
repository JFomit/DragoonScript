namespace DragoonScript.Runtime;

class Environment(Environment? parent = null)
{
    public Environment? Parent { get; } = parent;

    readonly Dictionary<string, object> _values = [];
    public void Define(string name, object value)
    {
        _values[name] = value;
    }
    public object Get(string name)
    {
        if (_values.TryGetValue(name, out var value))
        {
            return value;
        }

        return Parent?.Get(name) ?? throw new InvalidOperationException($"Undefined reference: {name}.");
    }
}