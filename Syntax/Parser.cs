using Compiler.Diagnostics;
using System.Diagnostics;
using Compiler.Syntax.Source;
using Compiler.Syntax.Lexing;
using Compiler.Syntax.Utils;
using JFomit.Functional.Extensions;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Syntax;

class Parser(TokenStream lexer)
{
    public TokenStream Lexer { get; } = lexer;
    public SourceDocument Document => Lexer.Document;

    private int Fuel { get; set; } = 256;
    public List<Diagnostic> Diagnostics { get; } = [];

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
    private ParseTree SwallowError(string message)
    {
        var tree = EnterRule();
        var error = Eat();
        tree.PushBack(error);
        var diagnostic = Diagnostic.Create(DiagnosticLabel.Create(error.Token))
            .WithSeverity(DiagnosticSeverity.Error)
            .WhitMessage(message).Build();

        PushDiagnostic(diagnostic);
        return ExitRule(tree, TreeKind.Error);
    }
    private ParseTree GobbleError(string message)
    {
        var tree = EnterRule();
        var error = new TokenTree(Peek());
        var diagnostic = Diagnostic.Create(DiagnosticLabel.Create(error.Token))
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
            Fuel--;

            return peeked;
        }
    }
    /// <summary>
    /// </summary>
    /// <param name="lookahead"><c>0</c> just peeks the next token with whitespaces.</param>
    private Token WhitespaceLookahead(int lookahead)
    {
        if (Fuel == 0)
        {
            throw new InvalidOperationException("Parser is stuck.");
        }

        Fuel -= lookahead + 1;
        return Lexer.Peek(lookahead);
    }

    // File ::= (FunctionDeclaration | TypeDeclaration | LetBinding)* EOF;
    public ParseTree File()
    {
        var tree = EnterRule();

        while (!Eof())
        {
            if (At(TokenKind.Fn) || At(TokenKind.Identifier))
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
                tree.PushBack(SwallowError("Expected a function, let-binding or type declaration."));
            }
        }
        tree.PushBack(Eat(TokenKind.EoF));

        return ExitRule(tree, TreeKind.File);
    }

    // FunctionDeclaration ::= 'fn' ID parameterList (':' typeExpression)? '='
    //                       | 'fn' (operator) parameterList (':' typeExpression)? '='
    private ParseTree FunctionDeclaration()
    {
        var tree = EnterRule();
        tree.PushBack(Expect(TokenKind.Fn)); // 'fn'
        if (At(TokenKind.LParen)) // operator
        {
            tree.PushBack(Eat(TokenKind.LParen));
            if (At(TokenKind.Operator) || At(TokenKind.Pipe))
            {
                tree.PushBack(Eat(), "NAME");
            }
            else if (At(TokenKind.RParen))
            {
                tree.PushBack(GobbleError("Expected operator."));
            }

            tree.PushBack(Expect(TokenKind.RParen));
        }
        else
        {
            tree.PushBack(Expect(TokenKind.Identifier), "NAME");
        }
        tree.PushBack(ParameterList(), "PARAMS"); // params
        if (At(TokenKind.Colon))
        {
            tree.PushBack(Eat(TokenKind.Colon));
            tree.PushBack(TypeExpression(), "TYPE");
        }
        else if (At(TokenKind.SignatureArrow))
        {
            tree.PushBack(Expect(TokenKind.Colon));
            tree.PushBack(TypeExpression());
        }

        tree.PushBack(Expect(TokenKind.Is)); // '='
        tree.PushBack(BlockExpression(), "BODY");

        return ExitRule(tree, TreeKind.FnDecl);
    }

    private ParseTree BlockExpression()
    {
        var tree = EnterRule();

        if (!At(TokenKind.Let) && !At(TokenKind.Return) && !IsAtExpressionStart())
        {
            tree.PushBack(GobbleError("Expected a let-biniding or expression."));
        }

        while (true)
        {
            if (At(TokenKind.Let))
            {
                tree.PushBack(LetBinding());
            }
            else if (IsAtExpressionStart())
            {
                tree.PushBack(Expression(), "RETURN");
                return ExitRule(tree, TreeKind.BlockExpr);
            }
            else
            {
                tree.PushBack(GobbleError("Incomplete expression block."));
                break;
            }
        }

        return ExitRule(tree, TreeKind.BlockExpr);
    }

    // TypeExpressions ::= '(' type_expression ')' | type_expression '->' type_expression | ID | ID type_expression
    private ParseTree TypeExpression()
    {
        var head = SimpleTypeExpression();

        while (At(TokenKind.SignatureArrow))
        {
            var tree = EnterRule();
            tree.PushBack(head);
            tree.PushBack(Eat(TokenKind.SignatureArrow));
            tree.PushBack(TypeExpression());
            head = ExitRule(tree, TreeKind.TypeArrowExpr);
        }

        return head;

        ParseTree SimpleTypeExpression()
        {
            if (At(TokenKind.LParen))
            {
                var tree = EnterRule();
                tree.PushBack(Eat(TokenKind.LParen));
                tree.PushBack(TypeExpression());
                tree.PushBack(Expect(TokenKind.RParen));
                return ExitRule(tree, TreeKind.TypeExpr);
            }
            else if (At(TokenKind.Identifier))
            {
                var tree = EnterRule();
                tree.PushBack(Eat(TokenKind.Identifier));
                while (true)
                {
                    if (At(TokenKind.Identifier))
                    {
                        tree.PushBack(Eat(TokenKind.Identifier));
                    }
                    else if (At(TokenKind.LParen))
                    {
                        tree.PushBack(SimpleTypeExpression());
                    }
                    else
                    {
                        break;
                    }
                }

                return ExitRule(tree, TreeKind.TypeConstructor);
            }
            return GobbleError("Expected a type.");
        }
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
    // LetBinding ::= 'let' BindingPattern '=' expr
    private ParseTree LetBinding()
    {
        var tree = EnterRule();

        tree.PushBack(Eat(TokenKind.Let));   // 'let'
        tree.PushBack(BindingPattern(), "PATTERN");     // pattern
        tree.PushBack(Expect(TokenKind.Is)); // '='
        tree.PushBack(Expression(), "VALUE");         // expression
        tree.PushBack(Expect(TokenKind.In)); // 'in'

        return ExitRule(tree, TreeKind.LetBind);
    }
    // BindingPattern ::= ID
    private ParseTree BindingPattern()
    {
        var tree = EnterRule();

        tree.PushBack(Expect(TokenKind.Identifier)); // varName

        return ExitRule(tree, TreeKind.LetPattern);
    }

    private ParseTree Expression()
    {
        return Expression(0);

        ParseTree Expression(int minPrecedence)
        {
            var result = PrimaryExpression();

            while (true)
            {
                var next = Peek();

                if (next.Kind != TokenKind.Operator && next.Kind != TokenKind.Pipe)
                {
                    break;
                }

                var (precedence, accosiativity) = GetPrecedence(next).Unwrap();
                if (precedence < minPrecedence)
                {
                    break;
                }

                var nextMinPrecedence = accosiativity == Associativity.Left ? precedence + 1 : precedence;
                var op = Eat();
                var rhs = Expression(nextMinPrecedence);

                var tree = EnterRule();

                tree.PushBack(result, "LHS").PushBack(op, "OP").PushBack(rhs, "RHS");

                result = ExitRule(tree, TreeKind.InfixExpr);
            }

            return result;
        }

        Option<CustomInfixOperator> GetPrecedence(Token op)
        {
            if (op.Kind == TokenKind.EoF)
            {
                return None;
            }

            var result = CustomInfixOperator.CreateFromToken(op);

            return result.ToOption();
        }
    }

    private ParseTree PrimaryExpression()
    {
        var list = new List<ParseTree>();
        while (IsAtExpressionStart())
        {
            list.Add(SimplePrimaryExpression());
        }

        if (list.Count == 0)
        {
            return SwallowError("Expected primary expression");
        }
        if (list.Count == 1)
        {
            return list[0];
        }
        else
        {
            var tree = EnterRule();
            tree.PushBack(list[0], "FUNCTION");
            for (int i = 1; i < list.Count; i++)
            {
                tree.PushBack(list[i]);
            }

            return ExitRule(tree, TreeKind.FnApply);
        }

        ParseTree SimplePrimaryExpression()
        {
            var tree = EnterRule();
            TreeKind kind;

            if (At(TokenKind.Identifier)) // Variable Reference
            {
                tree.PushBack(Eat(TokenKind.Identifier));
                kind = TreeKind.VariableRefExpr;
            }
            else if (At(TokenKind.Integer) || At(TokenKind.Float) || At(TokenKind.String)) // Literal
            {
                tree.PushBack(Eat());
                kind = TreeKind.LiteralExpr;
            }
            else if (At(TokenKind.LParen)) // Parenthesised Expression
            {
                tree.PushBack(Eat(TokenKind.LParen));
                tree.PushBack(Expression(), "INNER");
                tree.PushBack(Expect(TokenKind.RParen));
                kind = TreeKind.Expr;
            }
            else if (At(TokenKind.Operator) || At(TokenKind.Pipe)) // prefix operator
            {
                tree.PushBack(Eat(), "OP");
                tree.PushBack(PrimaryExpression(), "RHS");
                kind = TreeKind.PrefixExpr; // right associative
            }
            else if (At(TokenKind.If)) // If expression
            {
                tree.PushBack(Eat(TokenKind.If));
                tree.PushBack(Expression(), "CONDITION");
                tree.PushBack(Expect(TokenKind.Then));
                tree.PushBack(BlockExpression(), "THEN");
                tree.PushBack(Expect(TokenKind.Else));
                tree.PushBack(Expression(), "ELSE");
                kind = TreeKind.IfExpr;
            }
            else
            {
                return SwallowError("Expected primary expression.");
            }

            return ExitRule(tree, kind);
        }
    }

    bool IsAtExpressionStart()
    {
        var simpleStart =
            At(TokenKind.Identifier)
            || At(TokenKind.Integer)
            || At(TokenKind.Float)
            || At(TokenKind.LParen)
            || At(TokenKind.String)
            || At(TokenKind.If);

        if (simpleStart)
        {
            return true;
        }

        if (At(TokenKind.Operator) || At(TokenKind.Pipe))
        {
            var afterNext = WhitespaceLookahead(1);
            if (afterNext.Kind == TokenKind.WhiteSpace || afterNext.Kind == TokenKind.NewLine)
            {
                return false; // binary expression, e. g. f - 2 -> (- f 2)
            }
            else
            {
                return true; // prefix operation, e. g. f -2 -> (f (-2))
            }
        }

        return false;
    }
}
