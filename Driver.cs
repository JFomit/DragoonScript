using System.Diagnostics;
using ConsoleAppFramework;
using DragoonScript.Debugging;
using DragoonScript.Runtime;
using JFomit.Functional.Extensions;

namespace DragoonScript;

class Driver
{
    public static void Run(string[] args)
    {
        ConsoleApp.Version = "0.0.1";
        var app = ConsoleApp.Create();
        app.Add<ScriptRunnerCommands>();
#if DEBUG
        app.Add<DebugCommands>();
#endif
        app.Add<DriverDefault>();
        app.Run(args);
    }

    internal class DriverDefault
    {
#pragma warning disable CA1822 // Mark a member as static
        /// <summary>
        /// Runs a file written in Dragoon Script.
        /// </summary>
        /// <param name="input">Script to run.</param>
        [Command("")]
        public void Run([Argument] string input, ConsoleAppContext context) => ScriptRunner(input, context.EscapedArguments);
#pragma warning restore CA1822 // Mark a member as static
    }

    internal class ScriptRunnerCommands
    {
#pragma warning disable CA1822 // Mark a member as static
        /// <summary>
        /// Runs passed script.
        /// </summary>
        /// <param name="input">Script to run.</param>
        [Command("run")]
        public void Run([Argument] string input, ConsoleAppContext context) => ScriptRunner(input, context.EscapedArguments);
#pragma warning restore CA1822 // Mark a member as static
    }
#if DEBUG
    internal class DebugCommands
    {
#pragma warning disable CA1822 // Mark a member as static
        /// <summary>
        /// Dumps parse tree.
        /// </summary>
        /// <param name="input">Script to examine.</param>
        [Command("dumpast")]
        public void Dump([Argument] string input) => DumpAst(input);
#pragma warning restore CA1822 // Mark a member as static
    }
#endif

    private static int ScriptRunner(string scriptSourcePath, ReadOnlySpan<string> _)
    {
        bool isatty = !Console.IsOutputRedirected;

        string source = "";
        string identifier = "<stdin>";
        try
        {
            using var sourceInput = new StreamReader(scriptSourcePath);
            source = sourceInput.ReadToEnd();
            identifier = Path.GetFileName(scriptSourcePath);
        }
        catch (FileNotFoundException)
        {
            WriteError($"File not found: {scriptSourcePath}.");
            return 1;
        }
        catch (IOException e)
        {
            WriteError(e.Message);
            return 1;
        }

        var pipeline = new CompilerPipeLine(identifier, source);
        var program = pipeline.Lex().SelectMany(pipeline.Parse);
        if (program.TryUnwrapSuccess(out var tree))
        {
            var result = pipeline.Execute(tree);
            return result.Match(
                ok: (int x) => x,
                error: (error) =>
                {
                    WriteRuntimeError(error);
                    return 1;
                }
            );
        }
        else
        {
            var diagnostics = program.Error;
            foreach (var item in diagnostics.Take(5)) // TODO: fix trainwreck protection
            {
                WriteDiagnostic(item);
            }

            return 1;
        }
    }

    private static int DumpAst(string scriptSourcePath)
    {
        string source = "";
        string identifier = "<stdin>";
        try
        {
            using var sourceInput = new StreamReader(scriptSourcePath);
            source = sourceInput.ReadToEnd();
            identifier = Path.GetFileName(scriptSourcePath);
        }
        catch (FileNotFoundException)
        {
            WriteError($"File not found: {scriptSourcePath}.");
            return 1;
        }
        catch (IOException e)
        {
            WriteError(e.Message);
            return 1;
        }

        var pipeline = new CompilerPipeLine(identifier, source);
        var program = pipeline.Lex().Select(pipeline.ParseForce);
        if (program.TryUnwrapSuccess(out var t))
        {
            var visitpr = new ParseTreePrinter(false);
            visitpr.VisitTree(t);
            return 0;
        }
        else
        {
            return 1;
        }
    }

    private static void WriteError(string message)
    {
        Console.Error.WriteLine(message);
    }
    private static void WriteRuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"Runtime error: {error.Message}");
        if (error.Function.TryUnwrap(out var f))
        {
            Console.Error.WriteLine($"In {f}.");
        }
    }

    private static void WriteDiagnostic(Diagnostics.Diagnostic diagnostic)
    {
        var label = diagnostic.Severity switch
        {
            Diagnostics.DiagnosticSeverity.Info => "INF",
            Diagnostics.DiagnosticSeverity.Warning => "WRN",
            Diagnostics.DiagnosticSeverity.Error => "ERR",
            Diagnostics.DiagnosticSeverity.Fatal => "FTL",

            _ => throw new UnreachableException()
        };
        var pos = diagnostic.DiagnosticSource.Pos;
        var length = diagnostic.DiagnosticSource.Length;
        var line = diagnostic.DiagnosticSource.Line;
        var column = diagnostic.DiagnosticSource.Column;
        var text = diagnostic.DiagnosticSource.Document.Contents.AsSpan().Slice(pos, length);
        Console.Error.WriteLine($"[{label}] in {diagnostic.DiagnosticSource.Document.Identifier} Ln {line},Col {column}:");
        if (text.Length != 0)
        {
            Console.Error.WriteLine($" {text} ");
            Console.Error.WriteLine($" ^{new string('~', Math.Max(0, text.Length - 1))} HERE");
        }
        Console.Error.WriteLine(diagnostic.Message);
        diagnostic.Note.Select(s => $"NOTE: {s}").IfSome(Console.Error.WriteLine);
    }
}
