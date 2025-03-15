using Compiler;
using Compiler.Syntax;
using Compiler.Syntax.Utils;
using Pixie.Code;

// var s = """
// let x =
// fn s x x = x * x
// """;
// var doc = new StringDocument("stdin", s);

// var lexer = new Lexer(doc);
// var parser = new Parser(new NoWS(lexer));

// var tree = parser.File();
// var printer = new Printer();
// printer.Visit((IParseTreeItem)tree);

var s = """
->
fn
""";
var doc = new StringDocument("stdin", s);
var lexer = new Lexer(doc);

var pratt = new PrattParser(new NoWS(lexer));
var result = pratt.TypeExpression();
var printer = new Printer();
printer.Visit((IParseTreeItem)result);