using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core.Optimization;

class TailCallPass : AstNodeVisitor<Unit>, IOptimizationPass
{
    public void TransformFunction(FunctionDeclaration function)
    {
        TransformBlock(function.Body);
    }
    private void TransformBlock(LambdaTerm expression)
    {
        var last = expression;
        Option<Binding> beforeLast = None;
        while (last is Binding binding)
        {
            beforeLast = Some(binding);
            last = binding.Expression.Unwrap();
        }

        if (beforeLast.TryUnwrap(out var before))
        {
            if (before is ApplicationBinding application)
            {
                // tailcall
                application.IsTailcall = true;
            }
            else if (before is IfExpressionBinding ifExpression)
            {
                TransformBlock(ifExpression.Then);
                TransformBlock(ifExpression.Else);
            }
        }
    }
}
