namespace Compiler.Diagnostic;

enum DiagnosticSeverity
{
    /// <summary>A kind of suggestion, be it code style or performance improvement. It is safe to ignore this diagnostic.</summary>
    Info,
    /// <summary>A more serius issue. Ignoring it may lead to undesired behaviour of the code or vastly degraded performance.</summary>
    Warning,
    /// <summary>A compilation error. Means that the code is syntactically or semantically incorrect.</summary>
    Error,
    /// <summary>Only for internal testing. Means that there is a bug that crashed the compiler.</summary>
    Fatal
}