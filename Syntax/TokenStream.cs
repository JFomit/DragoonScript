using Compiler.Syntax;
using JFomit.Functional.Monads;

namespace Compiler;

abstract class TokenStream
{
    public abstract Token Next();
    public abstract Token Peek();
}