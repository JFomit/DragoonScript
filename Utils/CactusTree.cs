namespace DragoonScript.Utils;

static class CactusStack
{
    public static ICactusStack<T> CreateRoot<T>() => new CactusStackNodeSentinel<T>();
}

class CactusStackNodeSentinel<T>() : ICactusStack<T>
{
    public bool IsRoot => true;

    public T Pop(out ICactusStack<T>? parent)
    {
        parent = null;
        return default!;
    }

    public ICactusStack<T> PushSide(T value, out ICactusStack<T> newNode)
    {
        newNode = new CactusStackNode<T>(value)
        {
            Parent = this
        };

        return this;
    }

    public ICactusStack<T> PushUp(T value)
    {
        var node = new CactusStackNode<T>(value)
        {
            Parent = this
        };
        return node;
    }
}

interface ICactusStack<T>
{
    /// <summary>
    /// Creates a node and pushes it at the top of the current stem.
    /// </summary>
    /// <returns>The new node.</returns>
    ICactusStack<T> PushUp(T value);
    /// <summary>
    /// Creates a node and pushes it to the side of the current stem, creating a new one.
    /// </summary>
    /// <param name="newNode">The resulting child node in the new stem.</param>
    /// <returns>The old node - root of the new stem.</returns>
    ICactusStack<T> PushSide(T value, out ICactusStack<T> newNode);

    T Pop(out ICactusStack<T>? parent);

    bool IsRoot { get; }
}

class CactusStackNode<T>(T value) : ICactusStack<T>
{
    public bool IsRoot => Parent is null;

    public required ICactusStack<T> Parent { get; init; }
    public virtual T Value { get; set; } = value;

    public ICactusStack<T> PushUp(T value)
    {
        var node = new CactusStackNode<T>(value)
        {
            Parent = this
        };
        return node;
    }

    public ICactusStack<T> PushSide(T value, out ICactusStack<T> newNode)
    {
        newNode = new CactusStackNode<T>(value)
        {
            Parent = this
        };

        return this;
    }

    public T Pop(out ICactusStack<T>? parent)
    {
        parent = Parent;
        return Value;
    }
}