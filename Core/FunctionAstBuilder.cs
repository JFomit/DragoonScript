using Compiler.Core.Ast;
using Compiler.Syntax;
using JFomit.Functional.Monads;

namespace Compiler.Core;

//
// fn main x y = 
//   x + f y - x ^ 2
//          =>
// (-)
//  |-(+)
//  |  |-x
//  |  |-(f)
//  |     |-y
//  |-(^)
//     |-x
//     |-2
//          =>
// let main = \[x y].(
//   let 't0 = (f y) in
//     let 't1 = (^ x 2) in
//       let 't2 = (+ x 't0) in
//         let 't3 = (- 't2 't1) in
//           't3
// )
//
class FunctionAstVisitor : AnnotatedSyntaxTreeVisitor<Value>
{
    private int _counter = 0;
    private readonly Stack<LambdaTerm> _terms = [];

    private Variable GetNextVariable() => new($"'t{_counter++}");
    private Variable GetCurrentVariable() => new($"'t{_counter}");
    public void Reset()
    {
        _counter = 0;
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
        var op = opOption.Unwrap();
        var rhs = rhsOption.Unwrap();

        var result = GetNextVariable();
        // _terms.Push(new ApplicationBinding(result, op, [Visit(rhs)]));

        return result;
    }
}