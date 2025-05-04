using System.Runtime.InteropServices;
using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Runtime;

class Interpreter(FunctionScope globals) : AstNodeVisitor<object>
{
    public FunctionScope Global { get; } = globals;
    public FunctionScope Current { get; private set; } = globals.Fork();

    public override object VisitValueBinding(ValueBinding binding)
    {
        var result = binding.Variable;
        Current.Define(result.Name, Visit(binding.Value));
        return Visit(binding.Expression.Unwrap());
    }
    public override object VisitApplicationBinding(ApplicationBinding binding)
    {
        var result = binding.Variable;
        var function = (Closure)Visit(binding.Function);
        PushScope();
        PopScope();
        return Visit(binding.Expression.Unwrap());
    }

    public override object VisitLiteral(Literal literal) => ExtractLiteral(literal);
    private static object ExtractLiteral(Literal l)
    {
        if (double.TryParse(l.Value, out var d))
        {
            return d;
        }
        if (l.Value == "()")
        {
            return Prelude.Unit;
        }

        return l.Value.Replace("\\\"", "\"")[1..^1];
    }

    public void PushScope()
    {
        Current = Current.Fork();
    }
    public void PopScope()
    {
        Current = Current.Parent.Unwrap();
    }
}
