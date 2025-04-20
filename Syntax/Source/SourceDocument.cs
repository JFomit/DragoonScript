namespace DragoonScript.Syntax.Source;

class SourceDocument(string identifier, string contents)
{
    public string Identifier { get; init; } = identifier;
    public string Contents { get; init; } = contents;

    public int Length { get; init; } = contents.Length;
}