using System.Diagnostics;
using System.Reflection.Metadata;
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
    private ParseTree Expect(TokenKind kind)
    {
        if (At(kind))
        {
            return Eat(kind);
        }
        else
        {
            Console.WriteLine($"Invalid token, expected {kind}.");
            return new ParseTree(TreeKind.Error);
        }
    }
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
    // File ::= (FunctionDeclaration | TypeDeclaration | LetBinding)* EOF;
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
            else if (At(TokenKind.Let))
            {
                tree.AddChild(LetBinding());
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

    // FunctionDeclaration ::= 'fn' ID parameterList '='
    private ParseTree FunctionDeclaration()
    {
        var tree = EnterRule();
        tree.AddChild(Eat(TokenKind.Fn)); // 'fn'
        tree.AddChild(Expect(TokenKind.Identifier)); // name
        tree.AddChild(ParameterList()); // params
        if (At(TokenKind.Colon))
        {
            tree.AddChild(TypeExpression());
        }
        tree.AddChild(Expect(TokenKind.Is)); // '='
        // TODO: expressions

        return ExitRule(tree, TreeKind.FnDecl);
    }

    // TypeExpressions ::= ...
    private ParseTree TypeExpression()
    {
        var tree = EnterRule();

        return ExitRule(tree, TreeKind.TypeExpr);
    }

    // ParameterList ::= Parameter*
    private ParseTree ParameterList()
    {
        var tree = EnterRule();
        while (!At(TokenKind.Is) && !Eof())
        {
            if (At(TokenKind.Identifier))
            {
                tree.AddChild(Parameter());
            }
            else
            {
                break;
            }
        }
        return ExitRule(tree, TreeKind.FnParameterList);
    }
    // Parameter ::= ID
    private TokenTree Parameter()
    {
        return Eat(TokenKind.Identifier);
    }
    // LetBinding ::= 'let' BindingPattern '='
    private ParseTree LetBinding()
    {
        var tree = EnterRule();

        tree.AddChild(Eat(TokenKind.Let)); // 'let'
        tree.AddChild(BindingPattern()); // pattern
        tree.AddChild(Expect(TokenKind.Is)); // '='
        // TODO: expressions

        return ExitRule(tree, TreeKind.LetBind);
    }
    // BindingPattern ::= ID
    private ParseTree BindingPattern()
    {
        var tree = EnterRule();

        tree.AddChild(Expect(TokenKind.Identifier)); // varName

        return ExitRule(tree, TreeKind.LetPattern);
    }
}
