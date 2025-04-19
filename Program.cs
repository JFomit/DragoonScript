using System.Diagnostics;
using Compiler;
using Compiler.Diagnostics;
using Compiler.Syntax;
using Compiler.Syntax.Source;
using Compiler.Syntax.Utils;
using JFomit.Functional.Extensions;

var s = """
fn (|>) f x = f x

fn main m = m
    |> map
    |> filter
    |> collect

fn y =
    let x = 5 in
    let y = 42 * x in
    x + y

fn test =
    let x = 5 in
    let q =
        if x == 5 then
            let y = 18 in
            x + y
        else
            -1
    in q * x
""";

var doc = new SourceDocument("test", s);

var lexer = new Lexer(doc);
var parser = new Parser(lexer);

var tree = parser.File();
var printer = new Printer(false);
printer.VisitTree(tree);

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
        var line = diagnostic.DiagnosticSource.Line;
        var column = diagnostic.DiagnosticSource.Column;
        var text = diagnostic.DiagnosticSource.Document.Contents.AsSpan().Slice(pos, length);
        Console.WriteLine($"[{label}] in {diagnostic.DiagnosticSource.Document.Identifier} Ln {line},Col {column}:");
        if (text.Length != 0)
        {
            Console.WriteLine($" {text} ");
            Console.WriteLine($" ^{new string('~', Math.Max(0, text.Length - 1))} HERE");
        }
        Console.WriteLine(diagnostic.Message);
        diagnostic.Note.Select(s => $"NOTE: {s}").IfSome(Console.WriteLine);
    }
}
