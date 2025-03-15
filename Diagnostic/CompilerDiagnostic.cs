using Compiler.Syntax.Source;
using JFomit.Functional.Monads;

namespace Compiler.Diagnostic;

record CompilerDiagnostic(SourceDocument Document)
{
    public required int Start { get; init; }
    public required int Length { get; init; }

    public required DiagnosticSeverity Severity { get; init; }

    public required string Message { get; init; }
    public List<string> Context { get; init; } = [];
    public Option<string> Suggestion { get; init; }
}