using DragoonScript.Semantic;
using DragoonScript.Utils;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core.Ast;

/// <summary>
/// A-normal form for lambda calculus
/// </summary>
abstract record LambdaTerm : AstNode;
//EXP::= VAL 
//      | let VAR = VAL in EXP
//      | let VAR = VAL VAL in EXP
abstract record Value : LambdaTerm
{
    public static Value From(FunctionReference reference) => new FunctionVariable(reference);
    public static Value From(string name) => new Variable(name);
}
abstract record Binding(Variable Variable) : LambdaTerm
{
    public Option<LambdaTerm> Expression { get; set; } = None;
}
record ValueBinding(Variable Variable, Value Value) : Binding(Variable)
{
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
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitValueBinding(this);
}
// TODO: make lambda calculus `correct' by currying right after the parser or *in* the parser
record ApplicationBinding(Variable Variable, Value Function, Value[] Arguments) : Binding(Variable)
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
            if (Expression.TryUnwrap(out var e))
            {
                yield return e;
            }
        }
    }
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitApplicationBinding(this);
}

//VAL ::= VAR
//      | CONST
//      | λ VAR . EXP
record Variable(string Name) : Value
{
    public override IEnumerable<AstNode> Children => [];
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitVariable(this);
}
record FunctionVariable(FunctionReference Function) : Value
{
    public override IEnumerable<AstNode> Children => [];
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitFunctionVariable(this);
}
record Literal(string Value) : Value
{
    public override IEnumerable<AstNode> Children => [];
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitLiteral(this);
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
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitAbstraction(this);
}
record IfExpressionBinding(Variable Variable, Value Condition, LambdaTerm Then, LambdaTerm Else) : Binding(Variable)
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Condition;
            yield return Then;
            yield return Else;
            if (Expression.TryUnwrap(out var e))
            {
                yield return e;
            }
        }
    }
    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor) => visitor.VisitIfExpressionBinding(this);
}