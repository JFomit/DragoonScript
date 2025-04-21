using DragoonScript.Core.Ast;
using DragoonScript.Semantic;
using DragoonScript.Syntax;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core;

//
// fn main x y = 
//   let q = x * 2 in
//   x + f y - x ^ q
//          =>
// block
// |-let q = (*)
// |          |-x
// |          |-2
// |-(-)
//    |-(+)
//    |  |-x
//    |  |-(f)
//    |     |-y
//    |-(^)
//       |-x
//       |-q
//          =>
// let main = \[x y].(
//   let q = (* x 2) in
//     let 't0 = (f y) in
//       let 't1 = (^ x q) in
//         let 't2 = (+ x 't0) in
//           let 't3 = (- 't2 't1) in
//             't3
// )
//
class FunctionBodyVisitor : AnnotatedSyntaxTreeVisitor<Value>
{
    private int _counter = 0;
    private readonly Stack<Binding> _terms = [];

    private Variable GetNextVariable() => new($"'t{_counter++}");
    public void Reset()
    {
        _counter = 0;
        _terms.Clear();
    }

    public LambdaTerm VisitFunctionBody(ParseTree tree)
    {
        Reset();
        var expression = Visit(tree.Children[^1]); // last is returning

        return FixExpressions(expression);
    }

    private Binding FixExpressions(Value value)
    {
        if (_terms.Count == 0)
        {
            var v = GetNextVariable();
            var binding = new ValueBinding(v, value);
            binding.Expression = Some<LambdaTerm>(value);
            return binding;
        }

        var result = _terms.Pop();
        result.Expression = Some<LambdaTerm>(value);
        while (_terms.TryPop(out var term))
        {
            term.Expression = Some<LambdaTerm>(result);
            result = term;
        }

        return result;
    }

    protected override Value VisitApplication(ParseTree tree)
    {
        var result = GetNextVariable();
        var function = Visit(tree.Children[0]);

        _terms.Push(new ApplicationBinding(result, function, tree.Children.Skip(1).Select(Visit).ToArray()));

        return result;
    }
    // protected override Value VisitMatchExpression(ParseTree tree, Option<ParseTree> value, Option<ParseTree> patterns)
    // {
    // }
    // protected override Value VisitMatchPatternList(ParseTree tree, (ParseTree bindingPattern, ParseTree expression)[] patterns)
    // {
    // }
    protected override Value VisitBlock(ParseTree tree)
    {
        var expression = Visit(tree.Children[^1]); // last is returning

        return FixExpressions(expression).Variable;
    }
    protected override Value VisitIfExpression(
        ParseTree tree,
        Option<ParseTree> condiotionOption,
        Option<ParseTree> thenBranchOption,
        Option<ParseTree> elseBranchOption)
    {
        var condition = Visit(condiotionOption.Unwrap());
        var then = Visit(thenBranchOption.Unwrap());
        var @else = Visit(elseBranchOption.Unwrap());

        return new IfValue(condition, then, @else);
    }
    protected override Value VisitLetBinding(ParseTree tree, Option<ParseTree> patternOption, Option<ParseTree> value)
    {
        var result = (Variable)Visit(patternOption.Unwrap()); // VisitLetPattern (which will be called) always returns a variable for now
        _terms.Push(new ValueBinding(result, Visit(value.Unwrap())));
        return result;
    }
    protected override Variable VisitLetPattern(ParseTree tree, Option<TokenTree> variableOption)
    {
        // TODO: patterns and destructuring
        return new Variable(variableOption.Unwrap().Stringify());
    }
    protected override Value VisitBindingReference(ParseTree tree, Option<TokenTree> bindingOption)
    {
        var binding = bindingOption.Unwrap().Stringify();
        return new Variable(binding);
    }
    protected override Value VisitLiteral(ParseTree tree, Option<TokenTree> tokenTrees)
    {
        var token = tokenTrees.Unwrap();
        return new Literal(token.Stringify());
    }
    protected override Value VisitPrefixExpression(ParseTree tree, Option<TokenTree> opOption, Option<ParseTree> rhsOption)
    {
        var op = OperatorHelpers.GetOperatorFunctionRef(opOption.Unwrap(), isPrefix: true);
        var rhs = rhsOption.Unwrap();

        var result = GetNextVariable();
        _terms.Push(new ApplicationBinding(result, Value.From(op), [Visit(rhs)]));

        return result;
    }
    protected override Value VisitBinaryExpression(
        ParseTree tree,
        Option<ParseTree> lhsOption,
        Option<TokenTree> opOption,
        Option<ParseTree> rhsOption)
    {
        var lhs = lhsOption.Unwrap();
        var op = OperatorHelpers.GetOperatorFunctionRef(opOption.Unwrap(), isPrefix: false);
        var rhs = rhsOption.Unwrap();

        var result = GetNextVariable();
        _terms.Push(new ApplicationBinding(result, Value.From(op), [Visit(lhs), Visit(rhs)]));

        return result;
    }
}