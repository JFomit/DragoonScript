using Compiler.Syntax.Lexing;
using Compiler.Syntax.Utils;
using JFomit.Functional.Monads;
using JFomit.Functional.Extensions;
using static JFomit.Functional.Prelude;
using Compiler.Utils;

namespace Compiler.Syntax;

enum TreeKind
{
    Error = -1,
    Token,

    File,

    UnionTypeDecl,
    RecordTypeDecl,

    TypeExpr,
    TypeConstructor,
    TypeArrowExpr,

    FnDecl,
    FnParameterList,
    FnParameter,

    FnApply,

    LiteralExpr,
    VariableRefExpr,

    Expr,
    IfExpr,
    InfixExpr,
    PrefixExpr,
    PostfixExpr,
    BlockExpr,
    ReturnExpr,

    LetBind,

    LetPattern,
}

class ParseTree(TreeKind kind = TreeKind.Error) : IParseTreeItem
{
    public TreeKind Kind { get; set; } = kind;

    public IReadOnlyList<ParseTree> Children => _children;
    private readonly List<ParseTree> _children = [];
    private Option<Dictionary<string, ParseTree>> NamedChildren { get; set; } = None;

    public ParseTree PushBack(ParseTree child)
    {
        _children.Add(child);
        return this;
    }
    public ParseTree PushFront(ParseTree child)
    {
        _children.Insert(0, child);
        return this;
    }

    public ParseTree PushBack(ParseTree child, string name)
    {
        if (NamedChildren.TryUnwrap(out var dict))
        {
            dict.Add(name, child);
        }
        else
        {
            NamedChildren = Some(new Dictionary<string, ParseTree>()
            {
                [name] = child
            });
        }

        _children.Add(child);
        return this;
    }
    public ParseTree PushFront(ParseTree child, string name)
    {
        if (NamedChildren.TryUnwrap(out var dict))
        {
            dict.Add(name, child);
        }
        else
        {
            NamedChildren = Some(new Dictionary<string, ParseTree>()
            {
                [name] = child
            });
        }

        _children.Insert(0, child);
        return this;
    }

    public virtual void Accept(ParseTreeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public Option<ParseTree> GetNamedChild(string name)
        => NamedChildren
            .SelectMany(name, (d, n) => d.GetValue(n))
            .Flatten();

    public string Stringify()
        => Children.Select(c => c.Stringify()).Aggregate((p, n) => string.Join(p, n));
}
sealed class TokenTree(Token token) : ParseTree(TreeKind.Token), IParseTreeItem
{
    public Token Token { get; set; } = token;
    public override void Accept(ParseTreeVisitor visitor)
    {
        visitor.Visit(this);
    }
}