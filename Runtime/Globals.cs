using JFomit.Functional;

namespace DragoonScript.Runtime;

static class Globals
{
    public static FunctionScope GetInterpreterDefaults() => new(new()
    {
        ["~-"] = Closure.FromDelegate((int x) => -x),
        ["~+"] = Closure.FromDelegate((int x) => +x),

        ["+"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a + b),
            Closure.FromDelegate((byte a, byte b) => (byte)(a + b))),
        ["-"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a - b),
            Closure.FromDelegate((byte a, byte b) => (byte)(a - b))),
        ["*"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a * b),
            Closure.FromDelegate((byte a, byte b) => (byte)(a * b))),
        ["/"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a / b),
            Closure.FromDelegate((byte a, byte b) => (byte)(a / b))),
        ["%"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a % b),
            Closure.FromDelegate((byte a, byte b) => (byte)(a % b))),

        [">"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a > b),
            Closure.FromDelegate((byte a, byte b) => a > b)),
        ["<"] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a < b),
            Closure.FromDelegate((byte a, byte b) => a < b)),
        [">="] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a >= b),
            Closure.FromDelegate((byte a, byte b) => a >= b)),
        ["<="] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a <= b),
            Closure.FromDelegate((byte a, byte b) => a <= b)),
        ["=="] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a == b),
            Closure.FromDelegate((char a, char b) => a == b),
            Closure.FromDelegate((byte a, byte b) => a == b)),
        ["!="] = Closure.Overloaded(2,
            Closure.FromDelegate((int a, int b) => a != b),
            Closure.FromDelegate((char a, char b) => a != b),
            Closure.FromDelegate((byte a, byte b) => a != b)),

        ["++"] = Closure.FromDelegate((object x, object y) => $"{x}{y}"),

        ["printline"] = Closure.FromDelegate((object x) =>
        {
            if (x is Unit u)
            {
                Console.WriteLine();
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
        ["print"] = Closure.FromDelegate((object x) =>
        {
            if (x is Unit u)
            {
                Console.Write("()");
            }
            else if (x is IClosure cl)
            {
                Console.Write(cl.Format());
            }
            else
            {
                Console.Write(x);
            }
            return Prelude.Unit;
        }),
        ["read"] = Closure.FromDelegate((object x) =>
        {
            var str = Console.ReadLine()!;
            return str;
        }),
        ["read_char"] = Closure.FromDelegate((object x) =>
        {
            var str = (byte)Console.ReadKey().KeyChar;
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
        }).Curry(),
        ["length_of"] = Closure.FromDelegate((object array) =>
        {
            if (array is Array arr)
            {
                return arr.Length;
            }
            else if (array is string str)
            {
                return str.Length;
            }
            throw new InterpreterException($"Invalid type: expected string or array, got {array.GetType().Format()}.", Prelude.None);
        }),
        ["to_byte"] = Closure.FromDelegate((object value) =>
        {
            if (value is int i)
            {
                return (byte)i;
            }
            else if (value is byte b)
            {
                return b;
            }
            else if (value is char c)
            {
                return (byte)c;
            }
            else
            {
                return value.ValueCast<byte>();
            }
        }),
        ["to_char"] = Closure.FromDelegate((object value) =>
        {
            if (value is int i)
            {
                return (char)i;
            }
            else if (value is byte b)
            {
                return (char)b;
            }
            else if (value is char c)
            {
                return c;
            }
            else
            {
                return value.ValueCast<char>();
            }
        }),
        ["failwith"] = Closure.FromDelegate<string, Absurd>((string msg) => throw new InterpreterException(msg, Prelude.None))
    });
}
