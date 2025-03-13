using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax;

class CharStream(TextReader stream)
{
    public TextReader Stream { get; } = stream;

    public Option<char> Next()
    {
        var next = Stream.Read();
        return next == -1 ? None : Some((char)next);
    }

    public Option<char> Peek()
    {
        var next = Stream.Peek();
        return next == -1 ? None : Some((char)next);
    }
}