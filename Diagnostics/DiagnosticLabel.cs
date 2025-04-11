using Compiler.Syntax.Lexing;
using Compiler.Syntax.Source;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Diagnostics;

record struct DiagnosticLabel(SourceDocument Document, int Pos, int Length, int Line, int Column, ColorIndex Color = ColorIndex.Default)
{
    public Option<string> Message { get; set; } = None;

    public static DiagnosticLabel Create(SourceDocument source, int pos, int length, int line, int column, ColorIndex color = ColorIndex.Default)
        => new(source, pos, length, line, column, color);
    public static DiagnosticLabel Create(Token token, ColorIndex color = ColorIndex.Default)
        => new(token.Source, token.View.Pos, token.View.Length, token.View.Line, token.View.Column, color);

    public DiagnosticLabel WithMessage(string message)
    {
        Message = Some(message);
        return this;
    }
}
