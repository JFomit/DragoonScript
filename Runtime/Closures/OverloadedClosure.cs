using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class OverloadedClosure : IClosure
{
    public readonly struct Builder(int argsCount)
    {
        private readonly List<IClosure> _closures = [];
        private readonly int _argsCount = argsCount;

        public Result<OverloadedClosure, string> Build(string? format = null)
        {
            if (_closures.Count == 0)
            {
                return Error("Empty function group.");
            }

            var count = _argsCount;
            if (!_closures.All(ValidateClosures))
            {
                return Error("Function group contains functions of different arity.");
            }

            return Ok(new OverloadedClosure([.. _closures], format));

            bool ValidateClosures(IClosure closure) => closure.MaxArgsCount == count;
        }
    }

    public Option<string> Name { get; }

    public int MaxArgsCount { get; }
    public IClosure[] Closures { get; }

    private OverloadedClosure(IClosure[] closures, string? format = null)
    {
        Name = format.ToOption();
        Closures = closures;
    }

    public static Builder CreateBuilder(int count) => new(count);

    public object Call(Interpreter interpreter, object[] args)
    {
        if (args.Length > MaxArgsCount)
        {
            throw new InterpreterException("Extra arguments.", Some(Format()));
        }
        if (args.Length < MaxArgsCount)
        {
            throw new InterpreterException("Too few arguments provided.", Some(Format()));
        }

        return Delegate(interpreter, args);
    }

    public string Format() => Name.TryUnwrap(out var name) ? $"<{name}: group with {Closures.Length} functions>" : $"<group with {Closures.Length} functions>";
}