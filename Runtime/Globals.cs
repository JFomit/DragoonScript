using JFomit.Functional;

namespace DragoonScript.Runtime;

static class Globals
{
    public static FunctionScope GetInterpreterDefaults() => new(new()
    {
        ["~-"] = Closure.FromDelegate((double x) => -x),
        ["~+"] = Closure.FromDelegate((double x) => +x),

        ["+"] = Closure.FromDelegate((double a, double b) => a + b),
        ["-"] = Closure.FromDelegate((double a, double b) => a - b),
        ["*"] = Closure.FromDelegate((double a, double b) => a * b),
        ["/"] = Closure.FromDelegate((double a, double b) => a / b),

        [">"] = Closure.FromDelegate((double a, double b) => a > b),
        ["<"] = Closure.FromDelegate((double a, double b) => a < b),
        [">="] = Closure.FromDelegate((double a, double b) => a >= b),
        ["<="] = Closure.FromDelegate((double a, double b) => a <= b),
        ["=="] = Closure.FromDelegate((double a, double b) => a == b),
        ["!="] = Closure.FromDelegate((double a, double b) => a != b),

        ["++"] = Closure.FromDelegate((object x, object y) => $"{x}{y}"),

        ["print"] = Closure.FromDelegate((object x) =>
        {
            Console.WriteLine(x);
            return Prelude.Unit;
        }),
        ["read"] = Closure.FromDelegate((object x) =>
        {
            var str = Console.ReadLine()!;
            return str;
        }),
        ["random"] = Closure.FromDelegate((double a, double b) =>
        {
            return (double)Random.Shared.Next((int)a, (int)b);
        }),
        ["loop"] = Closure.Loop(),
        ["repeat"] = Closure.Repeat(),
        ["forever"] = Closure.InfiniteLoop(),
        ["while"] = Closure.While(),
    });
}