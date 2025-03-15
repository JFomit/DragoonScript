using Compiler.Syntax.Source;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Diagnostic;

sealed record Diagnostic(DiagnosticLabel DiagnosticSource)
{
    public required string Message { get; init; }
    public required DiagnosticSeverity Severity { get; init; }
    public List<DiagnosticLabel> Labels { get; init; } = [];
    public Option<string> Note { get; init; } = None;

    public static DiagnosticBuilder Create(DiagnosticLabel source) => new(source);
}
