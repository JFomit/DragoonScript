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
    private ICactusStack<Binding> _terms = CactusStack.CreateRoot<Binding>();

    private Variable GetNextVariable() => new($"'t{_counter++}");

    public void Reset()
    {
        _counter = 0;
        _terms = CactusStack.CreateRoot<Binding>();
    }

    // public LambdaTerm VisitFunctionBody(ParseTree tree)
    // {
    //     Reset();
    //     Value expression = Visit(tree.Children[^1]); // last is returning
    //     var body = FixExpressions(expression);
    //     return new Abstraction(tree.Children.SkipLast(1).Select(Visit).OfType<Variable>().ToArray(), body);
    // }

    protected override Value VisitFunctionDeclaration(ParseTree tree,
        Option<ParseTree> nameOption,
        Option<ParseTree> parametersOption,
        Option<ParseTree> bodyOption)
    {
        Reset();
        var body = bodyOption.Unwrap();
        var expression = FixExpressions(Visit(body));

        var parametersTree = parametersOption.Unwrap();
        var parameters = parametersTree.Children.Select(VisitFunctionParameter).OfType<Variable>().ToArray();

        return new Abstraction(parameters, expression);
    }

    protected override Value VisitFunctionParameter(ParseTree tree) => new Variable(tree.Stringify());

    private void PushUp(Binding binding)
    {
        _terms = _terms.PushUp(binding);
    }
    private void PushSide(Binding binding, out ICactusStack<Binding> newNode, out ICactusStack<Binding> stemRoot)
    {
        stemRoot = _terms.PushSide(binding, out newNode);
        _terms = newNode;
    }
    private bool TryPop(out Binding item)
    {
        if (_terms.IsRoot)
        {
            item = default!;
            return false;
        }
        item = _terms.Pop(out var parent);
        _terms = parent!;
        return true;
    }

    private LambdaTerm FixExpressions(Value value)
    {
        if (_terms.IsRoot)
        {
            return value;
        }

        TryPop(out var result);
        result.Expression = Some<LambdaTerm>(value);
        while (TryPop(out var term))
        {
            term.Expression = Some<LambdaTerm>(result);
            result = term;
        }

        return result;
    }
    private LambdaTerm FixExpressions(Value value, ICactusStack<Binding> sentinel)
    {
        if (_terms.IsRoot)
        {
            return value;
        }
        if (_terms == sentinel)
        {
            return value;
        }

        TryPop(out var result);
        result.Expression = Some<LambdaTerm>(value);
        while (_terms != sentinel && TryPop(out var term))
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

        PushUp(new ApplicationBinding(result, function, tree.Children.Skip(1).Select(Visit).ToArray()));

        return result;
    }
    protected new LambdaTerm VisitBlock(ParseTree tree)
    {
        PushSide(null!, out var stemRoot, out var root);
        _terms = stemRoot;
        Value expression = Visit(tree.Children[^1]); // last is returning
        var result = FixExpressions(expression, stemRoot);
        _terms = root;
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

        PushUp(node);
        return result;
    }
    protected override Value VisitLetBinding(ParseTree tree, Option<ParseTree> patternOption, Option<ParseTree> value)
    {
        Variable result = (Variable)Visit(patternOption.Unwrap()); // VisitLetPattern (which will be called) always returns a variable for now
        PushUp(new ValueBinding(result, Visit(value.Unwrap())));
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
        PushUp(new ApplicationBinding(result, Value.From(op), [Visit(rhs)]));

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
        PushUp(new ApplicationBinding(result, Value.From(op), [Visit(lhs), Visit(rhs)]));

        return result;
    }
}