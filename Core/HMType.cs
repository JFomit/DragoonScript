using DragoonScript.Runtime;

namespace DragoonScript.Core;

abstract class HMType
{
    public abstract bool IsOfType(object obj);
    public abstract bool IsEqualTo(HMType other);
    public virtual bool IsConvertibelTo(HMType other) => IsEqualTo(other);

    public abstract string Format();
}

class HMClosureType : HMType
{
    public override bool IsEqualTo(HMType other)
    {
        throw new NotImplementedException();
    }

    public override bool IsOfType(object obj)
    {
        if (obj is IClosure closure)
        {
            return closure.Type
        }
    }

    public override string Format()
    {
        throw new NotImplementedException();
    }
}