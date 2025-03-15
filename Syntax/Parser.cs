using Compiler.Diagnostics;
using System.Diagnostics;
using Compiler.Syntax.Source;
using Compiler.Syntax.Lexing;

namespace Compiler.Syntax;

class Parser
{
    public TokenStream Lexer { get; }
    public SourceDocument Document => Lexer.Document;

    private int Fuel { get; set; } = 256;
    public List<Diagnostic> Diagnostics { get; }

    private readonly TypeExpressionParser _typeExpressions;

    public Parser(TokenStream lexer)
    {
        Lexer = lexer;
        Diagnostics = [];
        _typeExpressions = new(lexer, Diagnostics);
    }

    private void PushDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }

    private static ParseTree EnterRule()
    {
        return new ParseTree(TreeKind.Error);
    }
    private static ParseTree ExitRule(ParseTree tree, TreeKind kind)
    {
        tree.Kind = kind;
        return tree;
    }
    private bool At(TokenKind kind, bool skipWs = true)
    {
        if (Fuel == 0)
        {
            throw new InvalidOperationException("Parser is stuck.");
        }

        Fuel -= 1;
        return Peek(skipWs: skipWs).Kind == kind;
    }
    private bool Eof(bool skipWs = true) => At(TokenKind.EoF, skipWs: skipWs);
    private ParseTree Error(string message)
    {
        var tree = EnterRule();
        var error = Eat();
        tree.PushBack(error);
        var diagnostic = Diagnostic.Create(DiagnosticLabel.Create(Document, error.Token.View.Pos, error.Token.View.Length))
            .WithSeverity(DiagnosticSeverity.Error)
            .WhitMessage(message).Build();

        PushDiagnostic(diagnostic);
        return ExitRule(tree, TreeKind.Error);
    }
    private ParseTree Expect(TokenKind kind, bool skipWs = true)
    {
        if (At(kind, skipWs: skipWs))
        {
            return Eat(kind, skipWs: skipWs);
        }
        else
        {
            var token = Peek();
            var diagnostic = Diagnostic.Create(DiagnosticLabel.Create(token))
                .WithSeverity(DiagnosticSeverity.Error)
                .WhitMessage(token.Kind == TokenKind.EoF ? "Unexpected end-of-file." : "Invalid token.")
                .WithNote($"Expected {kind}.")
                .Build();
            PushDiagnostic(diagnostic);
            return new ParseTree(TreeKind.Error);
        }
    }
    private TokenTree Eat(TokenKind? expectedKind = null, bool skipWs = true)
    {
        var next = Next(skipWs: skipWs);
#if DEBUG
        if (expectedKind is TokenKind expected)
        {
            Debug.Assert(next.Kind == expected, $"Next.Kind was {next.Kind}, expected {expected}");
        }
#endif
        Fuel = 256;
        return new(next);
    }

    private Token Next(bool skipWs = true)
    {
        while (true)
        {
            var next = Lexer.Next();
            if (skipWs && (next.Kind is TokenKind.NewLine or TokenKind.WhiteSpace))
            {
                continue;
            }

            return next;
        }
    }
    private Token Peek(bool skipWs = true)
    {
        while (true)
        {
            var peeked = Lexer.Peek();
            if (skipWs && (peeked.Kind is TokenKind.NewLine or TokenKind.WhiteSpace))
            {
                Lexer.Next();
                continue;
            }

            return peeked;
        }
    }

    // File ::= (FunctionDeclaration | TypeDeclaration | LetBinding)* EOF;
    public ParseTree File()
    {
        var tree = EnterRule();

        while (!Eof())
        {
            if (At(TokenKind.Fn))
            {
                tree.PushBack(FunctionDeclaration());
            }
            else if (At(TokenKind.Type))
            {
                // tree.AddChild(TypeDeclaration());
            }
            else if (At(TokenKind.Let))
            {
                tree.PushBack(LetBinding());
            }
            else
            {
                tree.PushBack(Error("Expected a function, let-binding or type declaration."));
            }
        }
        tree.PushBack(Eat(TokenKind.EoF));

        return ExitRule(tree, TreeKind.File);
    }

    // FunctionDeclaration ::= 'fn' ID parameterList '='
    private ParseTree FunctionDeclaration()
    {
        var tree = EnterRule();
        tree.PushBack(Eat(TokenKind.Fn)); // 'fn'
        tree.PushBack(Expect(TokenKind.Identifier)); // name
        tree.PushBack(ParameterList()); // params
        if (At(TokenKind.Colon))
        {
            tree.PushBack(Eat(TokenKind.Colon));
            tree.PushBack(TypeExpression());
        }
        tree.PushBack(Expect(TokenKind.Is)); // '='
                                             // TODO: expressions

        return ExitRule(tree, TreeKind.FnDecl);
    }

    // TypeExpressions ::= ...
    private ParseTree TypeExpression()
    {
        var tree = EnterRule();
        tree.PushBack(_typeExpressions.ParseExpression());
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
                tree.PushBack(Parameter());
            }
            else
            {
                break;
            }
        }
        return ExitRule(tree, TreeKind.FnParameterList);
    }
    // Parameter ::= ID
    private ParseTree Parameter()
    {
        var tree = EnterRule();
        tree.PushBack(Eat(TokenKind.Identifier));
        return ExitRule(tree, TreeKind.FnParameter);
    }
    // LetBinding ::= 'let' BindingPattern '='
    private ParseTree LetBinding()
    {
        var tree = EnterRule();

        tree.PushBack(Eat(TokenKind.Let));   // 'let'
        tree.PushBack(BindingPattern());     // pattern
        tree.PushBack(Expect(TokenKind.Is)); // '='
                                             // TODO: expressions

        return ExitRule(tree, TreeKind.LetBind);
    }
    // BindingPattern ::= ID
    private ParseTree BindingPattern()
    {
        var tree = EnterRule();

        tree.PushBack(Expect(TokenKind.Identifier)); // varName

        return ExitRule(tree, TreeKind.LetPattern);
    }
}
