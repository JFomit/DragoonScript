namespace Compiler.Runtime;

abstract record Value;
record Unit : Value;

record Closure : Value
{
    Value[]? Captures { get; } = null;
}
