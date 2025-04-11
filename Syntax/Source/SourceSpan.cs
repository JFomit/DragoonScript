using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax.Source;

readonly record struct SourceSpan(SourceDocument Source, int Pos, int Length, int Line, int Column)
{
    public SourceSpan(SourceDocument source, Range range, int line, int column)
        : this(source, range.Start.Value, range.End.Value - range.Start.Value, line, column)
    { }

    public ReadOnlySpan<char> AsSpan() => Source.Contents.AsSpan().Slice(Pos, Length);
}