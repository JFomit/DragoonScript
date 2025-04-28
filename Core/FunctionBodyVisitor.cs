using System.Runtime.CompilerServices;
using DragoonScript.Core.Ast;
using DragoonScript.Semantic;
using DragoonScript.Syntax;
using DragoonScript.Utils;
using JetBrains.Annotations;
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

internal class FunctionBodyVisitor : AnnotatedSyntaxTreeVisitor<Value>
{
    private int _counter;
    private readonly Stack<Binding> _terms = [];

    private Variable GetNextVariable() => new($"'t{_counter++}");

    public void Reset()
    {
        _counter = 0;
        _terms.Clear();
    }

    protected override Value VisitFunctionDeclaration(ParseTree tree,
        Option<ParseTree> nameOption,
        Option<ParseTree> parametersOption,
        Option<ParseTree> bodyOption)
    {
        Reset();
        var body = bodyOption.Unwrap();
        var expression = FixExpressions(VisitBlock(body));

        var parametersTree = parametersOption.Unwrap();
        var parameters = parametersTree.Children.Select(VisitFunctionParameter).OfType<Variable>().ToArray();

        return new Abstraction(parameters, expression);
    }

    protected override Value VisitFunctionParameter(ParseTree tree) => new Variable(tree.Stringify());

    private LambdaTerm FixExpressions(LambdaTerm value)
    {
        if (_terms.Count == 0)
        {
            return value;
        }

        var result = _terms.Pop();
        result.Expression = Some(value);
        while (_terms.TryPop(out var term))
        {
            term.Expression = Some<LambdaTerm>(result);
            result = term;
        }

        return result;
    }
    private LambdaTerm FixExpressions(LambdaTerm value, Binding sentinel)
    {
        if (_terms.Count == 0)
        {
            return value;
        }
        if (_terms.Peek() == sentinel)
        {
            return value;
        }

        var result = _terms.Pop();
        result.Expression = Some(value);
        while (_terms.Peek() != sentinel && _terms.TryPop(out var term))
        {
            term.Expression = Some<LambdaTerm>(result);
            result = term;
        }

        return result;
    }

    protected override Value VisitApplication(ParseTree tree)
    {
        Variable result = GetNextVariable();
        Value function = Visit(tree.Children[0]);

        _terms.Push(new ApplicationBinding(result, function, tree.Children.Skip(1).Select(Visit).ToArray()));

        return result;
    }
    protected new LambdaTerm VisitBlock(ParseTree tree)
    {
        var nullBinding = new NullBinding();
        _terms.Push(nullBinding);
        Value value = Visit(tree.Children[^1]); // last is returning
        // var expression = new Halt(value);
        var result = FixExpressions(value, nullBinding);
        _terms.Pop();
        return result;
    }
    protected override Value VisitIfExpression(
        ParseTree tree,
        Option<ParseTree> condiotionOption,
        Option<ParseTree> thenBranchOption,
        Option<ParseTree> elseBranchOption)
    {
        var result = GetNextVariable();
        Value condition = Visit(condiotionOption.Unwrap());
        var node = new IfExpressionBinding(result, condition, VisitBlock(thenBranchOption.Unwrap()), VisitBlock(elseBranchOption.Unwrap()));

        _terms.Push(node);
        return result;
    }
    protected override Value VisitLetBinding(ParseTree tree, Option<ParseTree> patternOption, Option<ParseTree> value)
    {
        Variable result = (Variable)Visit(patternOption.Unwrap()); // VisitLetPattern (which will be called) always returns a variable for now
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
        string binding = bindingOption.Unwrap().Stringify();
        return new Variable(binding);
    }
    protected override Value VisitLiteral(ParseTree tree, Option<TokenTree> tokenTrees)
    {
        TokenTree token = tokenTrees.Unwrap();
        return new Literal(token.Stringify());
    }
    protected override Value VisitPrefixExpression(ParseTree tree, Option<TokenTree> opOption, Option<ParseTree> rhsOption)
    {
        FunctionReference op = OperatorHelpers.GetOperatorFunctionRef(opOption.Unwrap(), isPrefix: true);
        ParseTree rhs = rhsOption.Unwrap();

        Variable result = GetNextVariable();
        _terms.Push(new ApplicationBinding(result, Value.From(op), [Visit(rhs)]));

        return result;
    }
    protected override Value VisitBinaryExpression(
        ParseTree tree,
        Option<ParseTree> lhsOption,
        Option<TokenTree> opOption,
        Option<ParseTree> rhsOption)
    {
        ParseTree lhs = lhsOption.Unwrap();
        FunctionReference op = OperatorHelpers.GetOperatorFunctionRef(opOption.Unwrap(), isPrefix: false);
        ParseTree rhs = rhsOption.Unwrap();

        Variable result = GetNextVariable();
        _terms.Push(new ApplicationBinding(result, Value.From(op), [Visit(lhs), Visit(rhs)]));

        return result;
    }
}

file record NullBinding() : Binding((Variable)null!)
{
    public override IEnumerable<AstNode> Children
    {
        get
        {
            throw new InvalidOperationException("Tried to walk a null-binding.");
        }
    }

    public override TResult Accept<TResult>(AstNodeVisitor<TResult> visitor)
    {
        throw new InvalidOperationException("Tried to visit a null-binding.");
    }
}