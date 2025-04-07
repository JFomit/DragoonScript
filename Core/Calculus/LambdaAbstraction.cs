using Compiler.Core.Semantic;

namespace Compiler.Core.Calculus;

class LambdaAbstraction(LambdaVariable variable, LambdaTerm term) : LambdaTerm
{
    public LambdaVariable Variable { get; } = variable;

    public LambdaTerm Term { get; } = term;
}
