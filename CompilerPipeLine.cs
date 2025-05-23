using DragoonScript.Diagnostics;
using DragoonScript.Syntax;
using DragoonScript.Syntax.Lexing;
using DragoonScript.Syntax.Source;
using JFomit.Functional.Monads;
using JFomit.Functional.Extensions;
using static JFomit.Functional.Prelude;
using DragoonScript.Runtime;
using DragoonScript.Core;
using DragoonScript.Core.Transformations;
using DragoonScript.Debugging;

namespace DragoonScript;

class CompilerPipeLine(string identifier, string source)
{
    public SourceDocument Source { get; } = new(identifier, source);

    public Result<TokenStream, Diagnostic[]> Lex() => Ok((TokenStream)new Lexer(Source));
    public Result<ParseTree, Diagnostic[]> Parse(TokenStream tokens)
    {
        var parser = new Parser(tokens);
        var tree = parser.File();
        return parser.Diagnostics.Count switch
        {
            > 0 => Error(parser.Diagnostics.ToArray()),
            _ => Ok(tree)
        };
    }
    public ParseTree ParseForce(TokenStream tokens)
    {
        var parser = new Parser(tokens);
        var tree = parser.File();
        return tree;
    }
    public Result<int, RuntimeError> Execute(ParseTree tree)
    {
        var ast = new AstBuilder();
        var program = ast.VisitFile(tree);
        var tailcallOptimizer = new TailCallPass();
        var joinOptimizer = new JoinPass();
        var printer = new AstConsolePrinter();
        foreach (var (_, function) in program)
        {
            tailcallOptimizer.TransformFunction(function);
            joinOptimizer.TransformFunction(function);
            printer.Visit(function);
        }
        Console.ReadKey();

        try
        {
            var globalScope = Globals.GetInterpreterDefaults();

            foreach (var (name, function) in program)
            {
                globalScope.DefineUniqueOrFork(name, Closure.FromDeclaration(function), out globalScope);
            }

            var runner = new Interpreter(globalScope);

            var result = runner.Visit(program["main"]);
            return result switch
            {
                double v => Ok((int)v),
                _ => Ok(0)
            };
        }
        catch (InterpreterException e)
        {
            return Error(e.ToError());
        }
    }
}