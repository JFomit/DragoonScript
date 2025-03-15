namespace Compiler.Syntax.Source;

public readonly record struct SourceSpan(string Source, int Pos, int Length)
{
    public SourceSpan(string source, Range range)
        : this(source, range.Start.Value, range.End.Value - range.Start.Value)
    { }

    public ReadOnlySpan<char> AsSpan() => Source.AsSpan().Slice(Pos, Length);
}