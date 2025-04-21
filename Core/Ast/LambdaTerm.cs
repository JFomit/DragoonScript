using DragoonScript.Semantic;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core.Ast;

/// <summary>
/// A-normal form lambda calculus
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
abstract record Binding : LambdaTerm
{
    public Option<LambdaTerm> Expression { get; set; } = None;
}
record ValueBinding(Variable Variable, Value Value) : Binding
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
    public override string Stringify()
        => $"(let {Variable.Stringify()} = {Value.Stringify()} in\n{Expression.Unwrap().Stringify()})";
}
// TODO: make lambda calculus `correct' by currying right after the parser or *in* the parser
record ApplicationBinding(Variable Variable, Value Function, Value[] Arguments) : Binding
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
    public override string Stringify()
        => $"(let {Variable.Stringify()} = ({Function.Stringify()} {Arguments.Select(a => a.Stringify()).Aggregate((p, n) => $"{p} {n}")}) in\n{Expression.Unwrap().Stringify()})";
}
record IfBinding(Variable Variable, Value Condition, Value Then, Value Else) : Binding
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Variable;
            yield return Condition;
            yield return Then;
            yield return Else;
            if (Expression.TryUnwrap(out var e))
            {
                yield return e;
            }
        }
    }
    public override string Stringify()
        => $"(let {Variable.Stringify()} = if ({Condition.Stringify()}) then ({Then.Stringify()}) else ({Else.Stringify()}) in \n{Expression.Unwrap().Stringify()})";
}

//VAL ::= VAR
//      | CONST
//      | λ VAR . EXP
record Variable(string Name) : Value
{
    public override IEnumerable<AstNode> Children => [];
    public override string Stringify() => Name;
}
record FunctionVariable(FunctionReference Function) : Value
{
    public override IEnumerable<AstNode> Children => [];
    public override string Stringify() => Function.Name;
}
record Literal(string Value) : Value
{
    public override IEnumerable<AstNode> Children => [];
    public override string Stringify() => Value;
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
    public override string Stringify() => $"(\\{Variable.Stringify()}.{Expression.Stringify()})";
}
