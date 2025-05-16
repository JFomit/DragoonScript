using JFomit.Functional;

namespace DragoonScript.Runtime;

static class Globals
{
    public static FunctionScope GetInterpreterDefaults() => new(new()
    {
        ["~-"] = Closure.FromDelegate((int x) => -x),
        ["~+"] = Closure.FromDelegate((int x) => +x),

        ["+"] = Closure.FromDelegate((int a, int b) => a + b),
        ["-"] = Closure.FromDelegate((int a, int b) => a - b),
        ["*"] = Closure.FromDelegate((int a, int b) => a * b),
        ["/"] = Closure.FromDelegate((int a, int b) => a / b),

        [">"] = Closure.FromDelegate((int a, int b) => a > b),
        ["<"] = Closure.FromDelegate((int a, int b) => a < b),
        [">="] = Closure.FromDelegate((int a, int b) => a >= b),
        ["<="] = Closure.FromDelegate((int a, int b) => a <= b),
        ["=="] = Closure.Overloaded(2, Closure.FromDelegate((int a, int b) => a == b), Closure.FromDelegate((char a, char b) => a == b)),
        ["!="] = Closure.Overloaded(2, Closure.FromDelegate((int a, int b) => a != b), Closure.FromDelegate((char a, char b) => a != b)),

        ["++"] = Closure.FromDelegate((object x, object y) => $"{x}{y}"),

        ["print"] = Closure.FromDelegate((object x) =>
        {
            if (x is Unit u)
            {
                Console.WriteLine("()");
            }
            else if (x is IClosure cl)
            {
                Console.WriteLine(cl.Format());
            }
            else
            {
                Console.WriteLine(x);
            }
            return Prelude.Unit;
        }),
        ["read"] = Closure.FromDelegate((object x) =>
        {
            var str = Console.ReadLine()!;
            return str;
        }),
        ["random"] = Closure.FromDelegate((int a, int b) =>
        {
            return Random.Shared.Next(a, b);
        }).Curry(),
        ["loop"] = Closure.Loop(),
        ["repeat"] = Closure.Repeat(),
        ["forever"] = Closure.InfiniteLoop(),
        ["while"] = Closure.While(),

        ["shell"] = ShellClosure.PrepareCommand(),
        ["||>"] = ShellClosure.Pipe(),
        ["run"] = ShellClosure.Run(),

        ["!!"] = Closure.FromDelegate((object array, int index) =>
        {
            if (array is Array arr)
            {
                return arr.GetValue(index);
            }
            else if (array is string str)
            {
                return str[index];
            }

            throw new InterpreterException($"Invalid type: expected string or array, got {array.GetType().Format()}.", Prelude.None);
        }),

        ["numarray"] = Closure.FromDelegate((int size) => new int[size]),
        ["bytearray"] = Closure.FromDelegate((int size) => new byte[size]),
        ["get"] = Closure.FromDelegate((int index, object array) =>
        {
            if (array is Array arr)
            {
                return arr.GetValue(index);
            }
            else if (array is string str)
            {
                return str[index];
            }
            throw new InterpreterException($"Invalid type: expected string or array, got {array.GetType().Format()}.", Prelude.None);
        }).Curry(),
        ["set"] = Closure.FromDelegate((int index, object value, object array) =>
        {
            if (array is Array arr)
            {
                arr.SetValue(value, index);
                return Prelude.Unit;
            }
            throw new InterpreterException($"Invalid type: expected array, got {array.GetType().Format()}.", Prelude.None);
        }).Curry()
    });
}