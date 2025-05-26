using JFomit.Functional;
using JFomit.Functional.Extensions;

namespace DragoonScript.Runtime;

internal static class Utils
{
    internal static string Format(this Type t)
    {
        if (t == typeof(Callable))
        {
            return "<fn>";
        }
        else if (t == typeof(Unit))
        {
            return "()";
        }
        else
        {
            return t.Name;
        }
    }
    internal static T ValueCast<T>(this object obj, Stack<CallFrame>? callFrames = null)
        => obj is T t
        ? t
        : throw new InterpreterException($"Invalid type: expected {typeof(T).Format()}, got {obj.GetType().Format()}.", callFrames ?? []);
}
