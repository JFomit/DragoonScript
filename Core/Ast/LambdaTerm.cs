
namespace Compiler.Core.Ast;

//EXP::= VAL 
//      | let VAR = VAL in EXP
//      | let VAR = VAL VAL in EXP
abstract record LambdaTerm : AstNode;
abstract record Value : LambdaTerm;
record ValueBinding(Variable Variable, Value Value, LambdaTerm Expression) : LambdaTerm
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Variable;
            yield return Value;
            yield return Expression;
        }
    }
}

// TODO: make lambda calculus `correct' by currying right after the parser or *in* the parser
record ApplicationBinding(Variable Variable, Value Function, Value[] Arguments, LambdaTerm Expression) : LambdaTerm
{
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
            yield return Expression;
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
