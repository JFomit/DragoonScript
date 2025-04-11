using Compiler.Syntax.Source;

namespace Compiler.Syntax.Lexing;

enum TokenKind
{
    Error = -1,
    EoF = 0,

    NewLine,
    WhiteSpace,
    LParen,
    RParen,

    SignatureArrow,
    Colon,
    Unit,

    Operator,

    Is,
    Pipe,

    Let,
    Fn,
    Type,

    Identifier,
    Integer,

    VBegin,
    VEnd,

    VSemi,
}

readonly struct Token(TokenKind type, SourceSpan view)
{
    public readonly SourceDocument Source => View.Source;

    public readonly TokenKind Kind = type;

    public readonly SourceSpan View = view;
}