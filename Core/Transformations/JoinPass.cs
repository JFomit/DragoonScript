using DragoonScript.Core.Ast;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core.Transformations;

class JoinPass : ITransformationPass
{
    private readonly Stack<(IfExpressionBinding binding, LambdaTerm next)> _ifs = [];

    public void TransformFunction(FunctionDeclaration function)
    {
        var expression = function.Body;
        while (expression is Binding binding)
        {
            if (binding is IfExpressionBinding ifBinding)
            {
                var next = ifBinding.Expression.Unwrap();
                _ifs.Push((ifBinding, next));
            }
            expression = binding.Expression.Unwrap();
        }

        while (_ifs.TryPop(out var tuple))
        {
            Patch(tuple.binding, tuple.next);
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
                binding.Expression = Some<LambdaTerm>(new Join((Value)expression, Some(next)));
            }
            else
            {
                ifBinding.Then = new Join((Value)expression, Some(next));
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
                binding.Expression = Some<LambdaTerm>(new Join((Value)expression, Some(next)));
            }
            else
            {
                ifBinding.Else = new Join((Value)expression, Some(next));
            }
        }
    }
}
