namespace DragoonScript.Utils;

class CactusTreeNode<T>(T value)
{
    public CactusTreeNode<T>? Parent { get; private set; } = null;
    public T Value { get; set; } = value;

    /// <summary>
    /// Creates a node and pushes it at the top of the current stem.
    /// </summary>
    /// <returns>The new node.</returns>
    public CactusTreeNode<T> PushUp(T value)
    {
        var node = new CactusTreeNode<T>(value);
        node.Parent = this;
        return node;
    }

    /// <summary>
    /// Creates a node and pushes it to the side of the current stem, creating a new one.
    /// </summary>
    /// <param name="newNode">The resulting child node in the new stem.</param>
    /// <returns>The old node - root of the new stem.</returns>
    public CactusTreeNode<T> PushSide(T value, out CactusTreeNode<T> newNode)
    {
        newNode = new CactusTreeNode<T>(value);
        newNode.Parent = this;

        return this;
    }
}