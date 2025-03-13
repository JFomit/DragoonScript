using Compiler.Syntax;

namespace Compiler;

enum TreeNodeKind
{
    Error,
    Token,

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
    LetExpr,
}

class ParseTree
{
    public TreeNodeKind Kind { get; set; }
    public List<ParseTreeChild> Children { get; } = [];
}

abstract record ParseTreeChild;
sealed record TokenChild(Token Token) : ParseTreeChild;
sealed record TreeChild(ParseTree Tree) : ParseTreeChild;
