using Compiler.Syntax.Lexing;
using Compiler.Syntax.Utils;

namespace Compiler.Syntax;

enum TreeKind
{
    Error = -1,
    Token,

    File,

    UnionTypeDecl,
    RecordTypeDecl,

    TypeExpr,
    SimpleTypeExpr,
    TypeArrowExpr,

    FnDecl,
    FnParameterList,
    FnParameter,

    FnApply,
    LiteralExpr,
    VariableRefExpr,
    LetBind,

    LetPattern,
}

class ParseTree(TreeKind kind = TreeKind.Error) : IParseTreeItem
{
    public TreeKind Kind { get; set; } = kind;
    public List<ParseTree> Children { get; } = [];

    public ParseTree AddChild(ParseTree child)
    {
        Children.Add(child);
        return this;
    }

    public virtual void Accept(ParseTreeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
sealed class TokenTree(Token token) : ParseTree(TreeKind.Token), IParseTreeItem
{
    public Token Token { get; set; } = token;
    public override void Accept(ParseTreeVisitor visitor)
    {
        visitor.Visit(this);
    }
}