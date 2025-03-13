namespace Compiler.Syntax;

enum TokenType
{
    Error = -1,
    EoF = 0,

    NewLine,
    WhiteSpace,
    LeftParenthesis,
    RightParenthesis,

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

readonly struct Token(TokenType type, int pos, int length)
{
    public readonly TokenType Type = type;
    public readonly int Pos = pos;
    public readonly int Length = length;

    public readonly ReadOnlySpan<char> AsSpan(string source)
        => source.AsSpan().Slice(Pos, Length);

    public static Token Create(TokenType type = TokenType.EoF, int pos = 0, int length = 0)
        => new(type, pos, length);
}