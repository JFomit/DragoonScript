using System.Diagnostics;
using CommandLine;
using DragoonScript.Runtime;
using JFomit.Functional.Extensions;

namespace DragoonScript;

class Driver
{
    public static int Run(string[] args)
    {
        return Parser.Default.ParseArguments<ScriptRunnerOptions>(args)
            .MapResult(
                ScriptRunner,
                HandleErrors
            );
    }

    private static int HandleErrors(IEnumerable<Error> errs)
    {
        return 1;
    }

    private static int ScriptRunner(ScriptRunnerOptions script)
    {
        bool isatty = !Console.IsOutputRedirected;

        string source = "";
        string identifier = "<stdin>";
        try
        {
            using var sourceInput = new StreamReader(script.Input);
            source = sourceInput.ReadToEnd();
            identifier = Path.GetFileName(script.Input);
        }
        catch (FileNotFoundException)
        {
            WriteError($"File not found: {script.Input}.");
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

    private static void WriteError(string message)
    {
        Console.Error.WriteLine(message);
    }
    private static void WriteRuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"Runtime error: {error.Message}");
        Console.Error.WriteLine($"In {error.Function}");
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

    [Verb("run", isDefault: true, aliases: ["r"], HelpText = "Runs a passed script file.")]
    public class ScriptRunnerOptions
    {
        [Value(0, HelpText = "The file to run.", MetaName = "Input", Required = true)]
        public string Input { get; set; } = "<stdin>";
    }

    [Verb("debug", isDefault: false, Hidden = true, HelpText = "Interpreter debugging utilities.")]
    public class DebugDumpOptions
    { }
}
