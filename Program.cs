using System.Diagnostics;
using DragoonScript;
using DragoonScript.Core;
using DragoonScript.Diagnostics;
using DragoonScript.Syntax;
using DragoonScript.Syntax.Source;
using JFomit.Functional.Extensions;

var s = """
fn ignore x =
    match x with
    | A ->
        let x = 5 in
        x + 5
    | B -> 3
""";

// fn main () = 2
// main ()
//      ==
// let main = () => 2 in
// main ()
//      ==
// (\main.(main ()) \().2)
// --------------------------------
// let ~@ = \str.system str in
// let main = ~@ "echo \"Hello, world!\"" in
// main ()
//      ==
// \~@.(
//   \main.(main ()) (\().~@ "echo \"Hello, world!\"")
// ) \str.(system str)
//

var doc = new SourceDocument("<stdin>", s);

var lexer = new Lexer(doc);
var parser = new Parser(lexer);

var tree = parser.File();
var printer = new Printer(false);
printer.VisitTree(tree);
// var visitor = new FunctionBodyVisitor();
// var value = visitor.VisitFunctionBody(tree.Children[0]);
// Console.WriteLine(value.Stringify());

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
