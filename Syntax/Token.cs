using Compiler.Syntax.Source;

namespace Compiler.Syntax;

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
}

readonly struct Token(TokenKind type, SourceSpan view)
{
    public readonly TokenKind Kind = type;

    public readonly SourceSpan View = view;
}