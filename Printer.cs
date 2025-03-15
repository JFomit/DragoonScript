using Compiler.Syntax;

namespace Compiler;

class Printer : ParseTreeVisitor
{
    private int _indent = 0;

    public override void Visit(ParseTree tree)
    {
        Console.WriteLine($"{new string(' ', _indent)}{tree.Kind}");

        _indent += 2;
        foreach (IParseTreeItem item in tree.Children)
        {
            Visit(item);
        }
        _indent -= 2;
    }

    public override void Visit(TokenTree tokenTree)
    {
        if (tokenTree.Token.Kind == TokenKind.EoF)
        {
            Console.WriteLine($"{new string(' ', _indent)}{tokenTree.Token.Kind}");
        }
        else
        {
            Console.WriteLine($"{new string(' ', _indent)}{tokenTree.Token.Kind} '{tokenTree.Token.View.AsSpan()}'");
        }
    }
}