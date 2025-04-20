using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Core.Ast;

/// <summary>
/// A-normal form lambda calculus
/// </summary>
abstract record LambdaTerm : AstNode;
//EXP::= VAL 
//      | let VAR = VAL in EXP
//      | let VAR = VAL VAL in EXP
abstract record Value : LambdaTerm;
record ValueBinding(Variable Variable, Value Value) : LambdaTerm
{
    public Option<LambdaTerm> Expression { get; set; } = None;
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Variable;
            yield return Value;
            if (Expression.TryUnwrap(out var e))
            {
                yield return e;
            }
        }
    }
}

// TODO: make lambda calculus `correct' by currying right after the parser or *in* the parser
record ApplicationBinding(Variable Variable, Value Function, Value[] Arguments) : LambdaTerm
{
    public Option<LambdaTerm> Expression { get; set; } = None;
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Variable;
            yield return Function;
            foreach (var arg in Arguments)
            {
                yield return arg;
            }
            if (Expression.TryUnwrap(out var e))
            {
                yield return e;
            }
        }
    }
}

//VAL ::= VAR
//      | CONST
//      | λ VAR . EXP
record Variable(string Name) : Value
{
    public override IEnumerable<AstNode> Children => Enumerable.Empty<AstNode>();
}
record Literal(string Name) : Value
{
    public override IEnumerable<AstNode> Children => Enumerable.Empty<AstNode>();
}
record Abstraction(Variable Variable, LambdaTerm Expression) : Value
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Variable;
            yield return Expression;
        }
    }
}
