using Compiler.Core.Semantic;

namespace Compiler.Core.Calculus;

class LambdaApply(LambdaAbstraction left, LambdaTerm right) : LambdaTerm
{
    public LambdaAbstraction Left { get; } = left;

    public LambdaTerm Right { get; } = right;
}
