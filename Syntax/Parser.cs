using System.Diagnostics;
using Compiler.Syntax;

namespace Compiler.Syntax;

class Parser(TokenStream lexer)
{
    public TokenStream Lexer { get; } = lexer;

    private int Fuel { get; set; } = 256;
    private static ParseTree EnterRule()
    {
        return new ParseTree(TreeKind.Error);
    }
    private static ParseTree ExitRule(ParseTree tree, TreeKind kind)
    {
        tree.Kind = kind;
        return tree;
    }
    private bool At(TokenKind kind)
    {
        if (Fuel == 0)
        {
            throw new InvalidOperationException("Parser is stuck.");
        }

        Fuel -= 1;
        return Lexer.Peek().Kind == kind;
    }
    private bool Eof() => At(TokenKind.EoF);
    private TokenTree Eat(TokenKind? expectedKind = null)
    {
        var next = Lexer.Next();
#if DEBUG
        if (expectedKind is TokenKind expected)
        {
            Debug.Assert(next.Kind == expected, $"Next.Kind was {next.Kind}, expected {expected}");
        }
#endif
        Fuel = 256;
        return new(next);
    }

    public ParseTree File()
    {
        var tree = EnterRule();

        while (!Eof())
        {
            if (At(TokenKind.Fn))
            {
                tree.AddChild(FunctionDeclaration());
            }
            else if (At(TokenKind.Type))
            {
                // tree.AddChild(TypeDeclaration());
            }
            else
            {
                tree.AddChild(Error("Expected a function, let-binding or type declaration."));
            }
        }
        tree.AddChild(Eat(TokenKind.EoF));

        return ExitRule(tree, TreeKind.File);
    }

    private ParseTree Error(string message)
    {
        var tree = EnterRule();
        tree.AddChild(Eat());
        Console.WriteLine(message);
        return ExitRule(tree, TreeKind.Error);
    }

    private ParseTree FunctionDeclaration()
    {
        var tree = EnterRule();
        tree.AddChild(Eat(TokenKind.Fn));
        tree.AddChild(Eat(TokenKind.Identifier));
        tree.AddChild(Eat(TokenKind.Identifier));
        tree.AddChild(Eat(TokenKind.Is));
        tree.AddChild(Eat(TokenKind.Identifier));
        tree.AddChild(Eat(TokenKind.Operator));
        tree.AddChild(Eat(TokenKind.Identifier));

        return ExitRule(tree, TreeKind.FnDecl);
    }
}