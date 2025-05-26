using DragoonScript.Core;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

record OverloadedCallable : Callable
{
    public readonly struct Builder(int argsCount)
    {
        private readonly List<Callable> _closures = [];
        private readonly int _argsCount = argsCount;

        public Builder AddRange(IEnumerable<Callable> closures)
        {
            _closures.AddRange(closures);
            return this;
        }
        public Builder Add(Callable closure)
        {
            _closures.Add(closure);
            return this;
        }

        public Result<OverloadedCallable, string> Build(string? format = null)
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

            return Ok(new OverloadedCallable([.. _closures], format));

            bool ValidateClosures(Callable closure) => closure.MaxArgsCount == count;
        }
    }

    public Option<string> Name { get; }

    public override int MaxArgsCount { get; }
    public Callable[] Closures { get; }

    public override HMClosureType Type { get; }

    private OverloadedCallable(Callable[] closures, string? format = null)
    {
        Name = format.ToOption();
        Closures = closures;
        MaxArgsCount = Closures[0].MaxArgsCount;
        Type = new(Enumerable.Range(1, MaxArgsCount).Select(_ => new Any()).ToArray());
    }

    public static Builder CreateBuilder(int count) => new(count);

    public override string Format() => Name.TryUnwrap(out var name) ? $"<{name}: group with {Closures.Length} functions>" : $"<group with {Closures.Length} functions>";
}