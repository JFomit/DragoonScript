using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core.Transformations;

class JoinPass : ITransformationPass
{
    private readonly Stack<(IfExpressionBinding binding, LambdaTerm next)> _ifs = [];

    public void TransformFunction(FunctionDeclaration function)
    {
        TransformBlock(function.Body);

        while (_ifs.TryPop(out var tuple))
        {
            Patch(tuple.binding, tuple.next);
        }
    }

    private void TransformBlock(LambdaTerm term)
    {
        var expression = term;
        while (expression is Binding binding)
        {
            if (binding is IfExpressionBinding ifBinding)
            {
                var next = ifBinding.Expression.Unwrap();

                TransformBlock(ifBinding.Then);
                TransformBlock(ifBinding.Else);

                _ifs.Push((ifBinding, next));
            }
            expression = binding.Expression.Unwrap();
        }
    }

    private static void Patch(IfExpressionBinding ifBinding, LambdaTerm next)
    {
        var expression = ifBinding.Then;
        Option<Binding> last = None;
        while (expression is Binding binding)
        {
            last = Some(binding);
            expression = binding.Expression.Unwrap();
        }

        {
            if (last.TryUnwrap(out var binding))
            {
                binding.Expression = Some<LambdaTerm>(new Join((Value)expression, ifBinding.Variable, Some(next)));
            }
            else
            {
                ifBinding.Then = new Join((Value)expression, ifBinding.Variable, Some(next));
            }
        }

        expression = ifBinding.Else;
        last = None;
        while (expression is Binding binding)
        {
            last = Some(binding);
            expression = binding.Expression.Unwrap();
        }

        {
            if (last.TryUnwrap(out var binding))
            {
                binding.Expression = Some<LambdaTerm>(new Join((Value)expression, ifBinding.Variable, Some(next)));
            }
            else
            {
                ifBinding.Else = new Join((Value)expression, ifBinding.Variable, Some(next));
            }
        }
    }
}
