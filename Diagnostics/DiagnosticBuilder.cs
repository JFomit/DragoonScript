using DragoonScript.Syntax.Source;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Diagnostics;

record struct DiagnosticBuilder(DiagnosticLabel Document);
record struct DSeverityBuilder(DiagnosticLabel Document, DiagnosticSeverity Severity);
record struct DLabelBuilder(DiagnosticLabel Document,
                            DiagnosticSeverity Severity,
                            string Message,
                            List<DiagnosticLabel> Labels,
                            Option<string> Note = default);

public static class DiagnosticBuilderExtensions
{
    internal static DSeverityBuilder WithSeverity(this DiagnosticBuilder builder, DiagnosticSeverity severity)
        => new(builder.Document, severity);
    internal static DLabelBuilder WhitMessage(this DSeverityBuilder builder, string message)
        => new(builder.Document, builder.Severity, message, []);
    internal static DLabelBuilder WithLabel(this DLabelBuilder builder, DiagnosticLabel label)
    {
        builder.Labels.Add(label);
        return builder;
    }
    internal static Diagnostic Build(this DLabelBuilder builder)
        => new(builder.Document)
        {
            Message = builder.Message,
            Severity = builder.Severity,
            Labels = builder.Labels,
            Note = builder.Note
        };
    internal static DLabelBuilder WithNote(this DLabelBuilder builder, string note)
    {
        builder.Note = Some(note);
        return builder;
    }
}