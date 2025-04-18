using Compiler.Syntax.Lexing;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax;

record FunctionDeclaration(string Name, Option<ParseTree> Type, ParseTree Expression);
record TypeExpression();
record Expression();

static class AbstractSyntaxTreeAdaptor
{
    public static IEnumerable<FunctionDeclaration> EnumerateFunctions(ParseTree file)
    {
        if (file.Kind != TreeKind.File)
        {
            yield break;
        }

        foreach (var child in file.Children)
        {
            if (child.Kind == TreeKind.FnDecl)
            {
                var declaration = FunctionDeclaration(child);
                if (declaration.TryUnwrap(out var inner))
                {
                    yield return inner;
                }
            }
        }
    }

    static Option<FunctionDeclaration> FunctionDeclaration(ParseTree function)
    {
        if (IsErrored(function))
        {
            return None;
        }

        var name = function.GetNamedChild("NAME").Unwrap().Stringify();
        var type = function.GetNamedChild("TYPE");
        var body = function.GetNamedChild("BODY").Unwrap();

        return Some(new FunctionDeclaration(name, type, body));
    }
    static bool IsErrored(ParseTree tree) => tree.Children.Any(IsErrored);
    static bool IsToken(ParseTree tree, TokenKind kind) => tree.Kind == TreeKind.Token && ((TokenTree)tree).Token.Kind == kind;
    static Option<TokenTree> AsToken(ParseTree tree) => tree.Kind == TreeKind.Token ? Some((TokenTree)tree) : None;
}