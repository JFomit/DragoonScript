using Compiler;
using Compiler.Syntax;
using Pixie.Code;

var s = """
let x =
fn s x x = x * x
""";
var doc = new StringDocument("stdin", s);

var lexer = new Lexer(doc);
var parser = new Parser(new NoWS(lexer));

var tree = parser.File();
var printer = new Printer();
printer.Visit((IParseTreeItem)tree);

// var de = new WithDestructor();

// typeof(WithDestructor)
// .GetMethod("Finalize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(de, []);
// typeof(WithDestructor)
// .GetMethod("Finalize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(de, []);

// class WithDestructor
// {
//     ~WithDestructor()
//     {
//         Console.WriteLine("Destroyed!");
//     }
// }
