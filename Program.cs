using System.Diagnostics;
using Compiler;
using Compiler.Diagnostics;
using Compiler.Syntax;
using Compiler.Syntax.Source;
using Compiler.Syntax.Utils;
using JFomit.Functional.Extensions;

var s = """
fn bind o f: Result -> int =
""";
var doc = new SourceDocument("test", s);

var lexer = new Lexer(doc);
var parser = new Parser(lexer);

var tree = parser.File();
var printer = new Printer(false);
printer.Visit((IParseTreeItem)tree);

parser.Diagnostics.ForEach(d => d.Print());

file static class Extensions
{
    internal static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
    {
        foreach (var item in self)
        {
            action(item);
        }
    }
    internal static void Print(this Diagnostic diagnostic)
    {
        var label = diagnostic.Severity switch
        {
            DiagnosticSeverity.Info => "INF",
            DiagnosticSeverity.Warning => "WRN",
            DiagnosticSeverity.Error => "ERR",
            DiagnosticSeverity.Fatal => "FTL",

            _ => throw new UnreachableException()
        };
        var pos = diagnostic.DiagnosticSource.Pos;
        var length = diagnostic.DiagnosticSource.Length;
        var text = diagnostic.DiagnosticSource.Document.Contents.AsSpan().Slice(pos, length);
        Console.WriteLine($"[{label}] in {diagnostic.DiagnosticSource.Document.Identifier} {pos}..{pos + length}:");
        if (text.Length != 0)
        {
            Console.WriteLine($" {text} ");
            Console.WriteLine($" ^{new string('~', Math.Max(0, text.Length - 1))} HERE");
        }
        Console.WriteLine(diagnostic.Message);
        diagnostic.Note.Select(s => $"NOTE: {s}").IfSome(Console.WriteLine);
    }
}
