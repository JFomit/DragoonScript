using DragoonScript;

return Driver.Run(args);
// var s = """
// fn talk () =
//     let purpose = print "What is my purpose?" |> read
//     "Excellent! I will do as I'm told: " ++ purpose ++ ".\n" |> print

// fn main () =
//     talk
//     |> while (\() -> random 0 6 > 0)
//     |> \() -> print "I am free!"

// fn (|>) x f = f x
// fn ($) f x = f x
// fn (.) g f = \x -> g (f x)
// """;

// // fn main () = 2
// // main ()
// //      ==
// // let main = () => 2 in
// // main ()
// //      ==
// // (\main.(main ()) \().2)
// // --------------------------------
// // let ~@ = \str.system str in
// // let main = ~@ "echo \"Hello, world!\"" in
// // main ()
// //      ==
// // \~@.(
// //   \main.(main ()) (\().~@ "echo \"Hello, world!\"")
// // ) \str.(system str)
// //
// // if x > 3 then
// //   let q = x + 3 in
// //   q
// // else
// //   42
// //      ==
// // let 't0 = + x 3 in
// //   let q = 't0 in
// //     q
// // let 't1 = > x 3 in
// //   let 't2 = if 't1 then 't0 else 42 in
// //     't2

// var doc = new SourceDocument("<stdin>", s);

// var lexer = new Lexer(doc);
// // var blocks = new BlockParser(lexer);
// // blocks.PrintBlocks();
// var parser = new Parser(lexer);
// var tree = parser.File();
// parser.Diagnostics.ForEach(d => d.Print());
// if (parser.Diagnostics.Count > 0)
// {
//     return;
// }

// var parserPrinter = new ParseTreePrinter(false);
// // parserPrinter.VisitTree(tree);
// var visitor = new AstBuilder();
// var program = visitor.VisitFile(tree);
// // var printer = new AstConsolePrinter();
// // foreach (var func in program.Values)
// // {
// //     printer.Visit(func);
// // }
// // Console.WriteLine();

// var builtIns = Globals.GetInterpreterDefaults();

// // user-defined functions go to global scope
// foreach (var (name, function) in program)
// {
//     Debug.Assert(builtIns.DefineUniqueOrFork(name, Closure.FromDeclaration(function), out builtIns));
// }

// var runner = new Interpreter(builtIns);
// runner.Visit(program["main"]);

// file static class Extensions
// {
//     internal static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
//     {
//         foreach (var item in self)
//         {
//             action(item);
//         }
//     }

//     internal static void Print(this Diagnostic diagnostic)
//     {

//     }
// }
