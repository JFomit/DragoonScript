using Compiler;
using Compiler.Syntax;
using Compiler.Syntax.Source;
using Compiler.Syntax.Utils;

var s = """
let x =
fn s x x: =
""";
var doc = new SourceDocument("stdin", s);

var lexer = new Lexer(doc);
var parser = new Parser(lexer);

var tree = parser.File();
var printer = new Printer();
printer.Visit((IParseTreeItem)tree);

parser.Diagnostics.ForEach(Console.WriteLine);

// var s = """
// ->
// fn
// """;
// var doc = new StringDocument("stdin", s);
// var lexer = new Lexer(doc);

// var pratt = new PrattParser(new NoWS(lexer));
// var result = pratt.TypeExpression();
// var printer = new Printer();
// printer.Visit((IParseTreeItem)result);

file static class Extensions
{
    internal static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
    {
        foreach (var item in self)
        {
            action(item);
        }
    }
}
