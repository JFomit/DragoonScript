using DragoonScript.Syntax.Source;

namespace DragoonScript.Syntax.Lexing;

enum TokenKind
{
    Error = -1,
    EoF = 0,

    NewLine,
    WhiteSpace,
    LParen,
    RParen,

    Arrow,
    Colon,
    Unit,

    Operator,

    Is,
    Pipe,

    Let,
    Fn,
    Type,
    If,
    Then,
    Else,
    In,
    Match,
    With,

    Identifier,
    Integer,
    Float,
    String,

    Semi,
    Indent,
    Dedent,
}

readonly struct Token(TokenKind type, SourceSpan view)
{
    public readonly SourceDocument Source => View.Source;

    public readonly TokenKind Kind = type;

    public readonly SourceSpan View = view;
}