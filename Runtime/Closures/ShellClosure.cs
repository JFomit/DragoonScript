using System.Collections.Specialized;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;
using JFomit.Functional;

namespace DragoonScript.Runtime;

static class ShellClosure
{
    public static IClosure PrepareCommand() => Closure.FromDelegate<string, Command>(x => Cli.Wrap(x).WithValidation(CommandResultValidation.None));

    public static IClosure WithArguments() => Closure.FromDelegate(static (Command cmd, string arg) => cmd.WithArguments(cmd.Arguments + ' ' + ArgumentsBuilder.Escape(arg)));
    public static IClosure Pipe() => Closure.FromDelegate((Command cmd, Command other) => cmd | other).Curry();
    public static IClosure Run() => Closure.FromDelegate((Command command) =>
    {
        using var stdOut = Console.OpenStandardOutput();
        using var stdErr = Console.OpenStandardError();

        var cmd = command | (stdOut, stdErr);
        return cmd.ExecuteBufferedAsync().GetAwaiter().GetResult().ExitCode;
    });
}
