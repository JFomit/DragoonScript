using DragoonScript.Syntax;
using DragoonScript.Syntax.Lexing;
using DragoonScript.Syntax.Utils;
using DragoonScript.Utils;
using JFomit.Functional.Monads;
using JFomit.Functional.Extensions;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Core;

abstract class AnnotatedSyntaxTreeVisitor<T> : ParseTreeVisitor<T>
    where T : class
{
    protected virtual T Default() => default!;
    protected virtual T Aggregate(T before, T next) => next;

    public override T Visit(TokenTree tokenTree) => VisitChildren(tokenTree);
    protected virtual T VisitChildren(ParseTree tree)
    {
        var result = Default();
        for (int i = 0; i < tree.Children.Count; i++)
        {
            if (!ShouldVisitNextChildren(tree, result))
            {
                break;
            }

            result = Aggregate(result, tree.Children[i].Accept(this));
        }

        return result;
    }
    protected virtual bool ShouldVisitNextChildren(ParseTree tree, T currentResult) => true;
    protected static bool ContainsLevelErrors(ParseTree tree)
    {
        return IsError(tree) || tree.Children.Any(IsError);

        static bool IsError(ParseTree tree) => tree.Kind switch
        {
            TreeKind.Token => ((TokenTree)tree).Token.Kind == TokenKind.Error,
            TreeKind.Error => true,
            _ => false,
        };
    }
    protected static bool ContainsErrors(ParseTree tree) => tree.Kind switch
    {
        TreeKind.Token => ((TokenTree)tree).Token.Kind == TokenKind.Error,
        TreeKind.Error => true,
        _ => tree.Children.Any(ContainsErrors)
    };

    public override T Visit(ParseTree tree)
    {
        if (ContainsLevelErrors(tree))
        {
            return VisitError(tree);
        }

        return tree.Kind switch
        {
            TreeKind.File => VisitFile(tree),
            TreeKind.LetBind => VisitLetBinding(
                tree,
                tree.GetNamedChild("PATTERN"),
                tree.GetNamedChild("VALUE")),
            TreeKind.FnDecl => VisitFunctionDeclaration(
                tree,
                tree.GetNamedChild("NAME"),
                tree.GetNamedChild("PARAMS"),
                tree.GetNamedChild("BODY")),
            TreeKind.LetPattern => VisitLetPattern(tree, tree.Children[0].AsToken()),

            TreeKind.Expr or
            TreeKind.InfixExpr or
            TreeKind.PrefixExpr or
            TreeKind.IfExpr or
            TreeKind.VariableRefExpr or
            TreeKind.FnApply or
            TreeKind.LiteralExpr => VisitExpression(tree),

            _ => VisitError(tree),
        };
    }

    protected virtual T VisitExpression(ParseTree tree)
        => tree.Kind switch
        {
            TreeKind.Expr => VisitExpression(tree.GetNamedChild("INNER").Expect("Bug in the parser.")),
            TreeKind.InfixExpr => VisitBinaryExpression(
                tree,
                tree.GetNamedChild("LHS"),
                tree.GetNamedChild("OP").SelectMany(Extensions.AsToken),
                tree.GetNamedChild("RHS")),
            TreeKind.PrefixExpr => VisitPrefixExpression(
                tree,
                tree.GetNamedChild("OP").SelectMany(Extensions.AsToken),
                tree.GetNamedChild("RHS")
            ),
            TreeKind.IfExpr => VisitIfExpression(
                tree,
                tree.GetNamedChild("CONDITION"),
                tree.GetNamedChild("THEN"),
                tree.GetNamedChild("ELSE")
            ),
            TreeKind.VariableRefExpr => VisitBindingReference(tree, tree.Children[0].AsToken()),
            TreeKind.LiteralExpr => VisitLiteral(tree, tree.Children[0].AsToken()),
            TreeKind.BlockExpr => VisitBlock(tree),
            TreeKind.FnApply => VisitApplication(tree),

            _ => VisitError(tree)
        };

    protected virtual T VisitApplication(ParseTree tree)
        => VisitChildren(tree);

    protected virtual T VisitBlock(ParseTree tree)
        => VisitChildren(tree);
    protected virtual T VisitLiteral(
        ParseTree tree,
        Option<TokenTree> tokenTrees)
        => VisitChildren(tree);
    protected virtual T VisitBindingReference(
        ParseTree tree,
        Option<TokenTree> bindingOption)
        => VisitChildren(tree);
    protected virtual T VisitIfExpression(
        ParseTree tree,
        Option<ParseTree> condiotionOption,
        Option<ParseTree> thenBranchOption,
        Option<ParseTree> elseBranchOption)
        => VisitChildren(tree);
    protected virtual T VisitPrefixExpression(
        ParseTree tree,
        Option<TokenTree> opOption,
        Option<ParseTree> rhsOption)
        => VisitChildren(tree);
    protected virtual T VisitBinaryExpression(
        ParseTree tree,
        Option<ParseTree> lhsOption,
        Option<TokenTree> opOption,
        Option<ParseTree> rhsOption)
        => VisitChildren(tree);
    protected virtual T VisitLetPattern(
        ParseTree tree,
        Option<TokenTree> variableOption)
        => VisitChildren(tree);
    protected virtual T VisitFunctionDeclaration(
        ParseTree tree,
        Option<ParseTree> nameOption,
        Option<ParseTree> parametersOption,
        Option<ParseTree> bodyOption)
        => VisitChildren(tree);
    protected virtual T VisitError(ParseTree tree) => VisitChildren(tree);
    protected virtual T VisitFile(ParseTree tree) => VisitChildren(tree);
    protected virtual T VisitLetBinding(
        ParseTree tree,
        Option<ParseTree> pattern,
        Option<ParseTree> value)
        => VisitChildren(tree);
}

internal static class Extensions
{
    internal static Option<TokenTree> AsToken(this ParseTree tree)
        => tree.Kind switch
        {
            TreeKind.Token => Some((TokenTree)tree),
            _ => None
        };
}