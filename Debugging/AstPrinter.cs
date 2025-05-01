using System.Text;
using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Debugging;

class AstConsolePrinter : AstNodeVisitor<Unit>
{
    private int _indent = 0;
    private string Indent => new(' ', _indent);

    public override Unit VisitFunctionDeclaration(FunctionDeclaration function)
    {
        var name = function.Name;
        var body = function.Body;
        var variables = function.Parameters;
        Console.WriteLine($"{Indent}{name} = \\[{FormatVariables(variables)}].");
        _indent += 2;
        Visit(body);
        _indent -= 2;
        return [];
    }

    public override Unit VisitAbstraction(Abstraction abstraction)
    {
        var body = abstraction.Expression;
        var variables = abstraction.Variables;
        Console.WriteLine($"{Indent}\\[{FormatVariables(variables)}].");
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

        Console.WriteLine($"{Indent}let {variable.Name} = ({FormatAtomic(function)} {FormatAtomics(args)}) in");
        _indent += 2;
        Visit(body);
        _indent -= 2;

        return [];
    }
    public override Unit VisitHalt(Value value)
    {
        Console.WriteLine($"{Indent}halt {FormatAtomic(value)}");
        return [];
    }

    public override Unit VisitIfExpressionBinding(IfExpressionBinding binding)
    {
        var variable = binding.Variable;
        var cond = binding.Condition;
        var then = binding.Then;
        var @else = binding.Else;
        Console.WriteLine($"{Indent}let {FormatAtomic(variable)} = if {FormatAtomic(cond)} then");
        _indent += 2;
        Visit(then);
        _indent -= 2;
        Console.WriteLine($"{Indent}else");
        _indent += 2;
        Visit(@else);
        _indent -= 2;
        Console.WriteLine($"{Indent}in");
        _indent += 2;
        Visit(binding.Expression.Unwrap());
        _indent -= 2;
        return [];
    }
    public override Unit VisitValueBinding(ValueBinding binding)
    {
        var body = binding.Expression.Unwrap();
        var variable = binding.Variable;
        var value = binding.Value;

        Console.WriteLine($"{Indent}let {variable.Name} = {FormatAtomic(value)} in");
        _indent += 2;
        Visit(body);
        _indent -= 2;

        return [];
    }
    public override Unit VisitLiteral(Literal literal)
    {
        Console.WriteLine($"{Indent}{literal.Value}");
        return [];
    }
    public override Unit VisitVariable(Variable variable)
    {
        Console.WriteLine($"{Indent}{variable.Name}");
        return [];
    }

    private static string FormatVariables(LambdaTerm[] array) => array.OfType<Variable>().ToArray() switch
    {
    [] => "",
    [var one] => $"{one.Name}",
        var a => a.Select(x => x.Name).Aggregate((p, n) => $"{p}, {n}")
    };

    private static string FormatAtomic(Value one) => one switch
    {
        Variable v => v.Name,
        Literal l => l.Value,
        FunctionVariable f => f.Function.Name,
        _ => ""
    };
    private static string FormatAtomics(Value[] array) => array switch
    {
    [] => "",
    [var atomic] => FormatAtomic(atomic),
        var atomics => atomics.Select(FormatAtomic).Aggregate((p, n) => $"{p} {n}")
    };
}
