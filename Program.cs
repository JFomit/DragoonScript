using System.Diagnostics;
using DragoonScript;
using DragoonScript.Core;
using DragoonScript.Debugging;
using DragoonScript.Diagnostics;
using DragoonScript.Syntax;
using DragoonScript.Syntax.Source;
using JFomit.Functional;
using JFomit.Functional.Extensions;

var s = """
fn (|>) f x = f x
fn (||>) pipe command = shellExecuteFromPipe command pipe

fn run str = shellExecute str
fn ignore x = ()

fn max x y =
    if x > y then x else y

fn main = (run "echo \"Hello, world!\"")
  ||> "cat"
  ||> "grep \"world\""
   |> ignore
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
// if x > 3 then
//   let q = x + 3 in
//   q
// else
//   42
//      ==
// let 't0 = x + 3 in
//   let q = 't0 in
//     q
// let 't1 = x > 3 in
//   let 't2 = if 't1 then 't0 else 42 in
//     't2

var doc = new SourceDocument("<stdin>", s);

var lexer = new Lexer(doc);
var parser = new Parser(lexer);

var tree = parser.File();
// var printer = new ParseTreePrinter(false);
// printer.VisitTree(tree);
var visitor = new FunctionBodyVisitor();

foreach (var item in tree.Children)
{
    if (item.Kind == TreeKind.Token)
    {
        continue;
    }

    var value = visitor.Visit(item);
    var printer = new AstConsolePrinter();
    printer.Visit(value);
    Console.WriteLine();
}

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
