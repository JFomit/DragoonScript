using DragoonScript.Syntax;
using DragoonScript.Syntax.Lexing;
using DragoonScript.Syntax.Utils;
using DragoonScript.Utils;

namespace DragoonScript;

class Printer(bool minify) : ParseTreeVisitor
{
    private int _indent = 0;
    private readonly bool _minify = minify;

    public override void Visit(ParseTree tree)
    {
        Console.WriteLine($"{new string(' ', _indent)}{tree.Kind}");

        _indent += 2;
        foreach (IParseTreeItem item in tree.Children)
        {
            VisitTree(item);
        }
        _indent -= 2;
    }

    public override void Visit(TokenTree tokenTree)
    {
        if (tokenTree.Token.Kind == TokenKind.EoF)
        {
            Console.WriteLine($"{new string(' ', _indent)}{(_minify ? "T" : tokenTree.Token.Kind)}");
        }
        else
        {
            Console.WriteLine($"{new string(' ', _indent)}{(_minify ? "T" : tokenTree.Token.Kind)} '{tokenTree.Token.View.AsSpan()}'");
        }
    }
}