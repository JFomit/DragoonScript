using System.Text;
using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Debugging;

class AstPrinter : AstNodeVisitor<string>
{
    public override string VisitApplicationBinding(ApplicationBinding binding)
        => $"(let {Visit(binding.Variable)} = ({Visit(binding.Function)} {string.Join(' ', binding.Arguments.Select(Visit))}) in\n{Visit(binding.Expression.Unwrap())})";
    public override string VisitValueBinding(ValueBinding binding)
        => $"(let {Visit(binding.Variable)} = {Visit(binding.Value)} in\n{Visit(binding.Expression.Unwrap())})";
    public override string VisitVariable(Variable variable) => variable.Name;
    public override string VisitLiteral(Literal literal) => literal.Value;
    public override string VisitFunctionVariable(FunctionVariable functionVariable) => functionVariable.Function.Name;
    public override string VisitIfExpressionBinding(IfExpressionBinding binding)
        => $"(let {Visit(binding.Variable)} = if ({Visit(binding.Condition)}) then ({Visit(binding.Then)}) else ({Visit(binding.Else)}) in\n{Visit(binding.Expression.Unwrap())})";
    public override string VisitAbstraction(Abstraction abstraction)
        => $"\\[{FormatCommaSperated(abstraction.Variables)}].(\n{Visit(abstraction.Expression)}\n)";

    private string FormatCommaSperated(LambdaTerm[] array) => array switch
    {
    [] => "",
    [var one] => $"{Visit(one)}",
        _ => array.Select(Visit).Aggregate((p, n) => $"{p}, {n}")
    };
}

class AstConsolePrinter : AstNodeVisitor<Unit>
{
    private int _indent = 0;

    public override Unit VisitAbstraction(Abstraction abstraction)
    {
        var body = abstraction.Expression;
        var variables = abstraction.Variables;
        Console.WriteLine($"{new string(' ', _indent)}\\[{FormatVariables(variables)}].");
        _indent += 2;
        Visit(body);
        _indent -= 2;
        return [];
    }
    public override Unit VisitApplicationBinding(ApplicationBinding binding)
    {
        var body = binding.Expression.Unwrap();
        var variable = binding.Variable;
        var function = binding.Function;
        var args = binding.Arguments;

        Console.WriteLine($"{new string(' ', _indent)}let {variable.Name} = {FormatAtomics(function)} {FormatAtomics(args)} in");
        _indent += 2;
        Visit(body);
        _indent -= 2;

        return [];
    }

    private static string FormatVariables(LambdaTerm[] array) => array.OfType<Variable>().ToArray() switch
    {
    [] => "",
    [var one] => $"{one.Name}",
        var a => a.Select(x => x.Name).Aggregate((p, n) => $"{p}, {n}")
    };

    private static string FormatAtomics(LambdaTerm one) => one switch
    {
        Variable v => v.Name,
        Literal l => l.Value,
        FunctionVariable f => f.Function.Name,
        _ => ""
    };
    private static string FormatAtomics(LambdaTerm[] array) => array.Aggregate("",
        (string p, LambdaTerm n) => n switch
        {
            Variable v => $"{p} {v.Name}",
            Literal l => $"{p} {l.Value}",
            _ => p
        }
    );
}