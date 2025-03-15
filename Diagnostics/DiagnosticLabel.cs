using Compiler.Syntax;
using Compiler.Syntax.Source;

namespace Compiler.Diagnostics;

record struct DiagnosticLabel(SourceDocument Document, int Pos, int Length, ColorIndex Color = ColorIndex.Default)
{
    public static DiagnosticLabel Create(SourceDocument source, int pos, int length, ColorIndex color = ColorIndex.Default)
        => new(source, pos, length, color);
    public static DiagnosticLabel Create(Token token, ColorIndex color = ColorIndex.Default)
        => new(token.Source, token.View.Pos, token.View.Length, color);
}
