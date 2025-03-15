using Compiler.Syntax.Source;

namespace Compiler.Diagnostic;

record struct DiagnosticLabel(SourceDocument Document, int Pos, int Length, ColorIndex color = ColorIndex.Default);
