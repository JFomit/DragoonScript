using System.Collections.Specialized;
using CliWrap;
using CliWrap.Buffered;
using JFomit.Functional;

namespace DragoonScript.Runtime;

static class ShellClallable
{
    public static Callable PrepareCommand() => Closure.FromDelegate<string, Command>(Cli.Wrap);
    public static Callable Pipe() => Closure.FromDelegate((Command cmd, Command other) => cmd | other).Curry();
    public static Callable Run() => Closure.FromDelegate((Command command) =>
    {
        using var stdOut = Console.OpenStandardOutput();
        using var stdErr = Console.OpenStandardError();

        var cmd = command | (stdOut, stdErr);
        return cmd.ExecuteBufferedAsync().GetAwaiter().GetResult().ExitCode;
    });
}
