using Compiler.Syntax.Source;
using JFomit.Functional.Monads;

namespace Compiler.Diagnostic;

record CompilerDiagnostic(SourceDocument Document)
{
    public int Start { get; init; } = 0;
    public int Length { get; init; } = Document.Length;

    public DiagnosticSeverity Severity { get; init; } = DiagnosticSeverity.Fatal;

    public required string Message { get; init; }
    public Option<string> Suggestion { get; init; }
}