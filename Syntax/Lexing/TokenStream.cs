using Compiler.Syntax.Source;
using JFomit.Functional.Monads;

namespace Compiler.Syntax.Lexing;

abstract class TokenStream
{
    public abstract SourceDocument Document { get; }

    public abstract Token Next();
    public abstract Token Peek();
}