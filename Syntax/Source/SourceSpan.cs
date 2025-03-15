namespace Compiler.Syntax.Source;

readonly record struct SourceSpan(SourceDocument Source, int Pos, int Length)
{
    public SourceSpan(SourceDocument source, Range range)
        : this(source, range.Start.Value, range.End.Value - range.Start.Value)
    { }

    public ReadOnlySpan<char> AsSpan() => Source.Contents.AsSpan().Slice(Pos, Length);
}