using DragoonScript.Utils;

namespace DragoonScript.Runtime;

class VM
{
    Dictionary<string, VMValue> Variables { get; } = [];
}

enum Kind
{
    Integer,
    Boolean,
}

record VMValue(Kind Kind);