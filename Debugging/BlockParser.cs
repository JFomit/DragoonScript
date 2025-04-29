
using DragoonScript.Syntax.Lexing;

class BlockParser(TokenStream stream)
{
    private readonly TokenStream _stream = stream;

    private int _offsets = 0;
    public void PrintBlocks()
    {
        Token last = default;
        while (true)
        {
            var next = _stream.Next();
            if (next.Kind == TokenKind.EoF)
            {
                break;
            }

            if (next.Kind == TokenKind.Indent)
            {
                _offsets += 1;
                Console.Write('>');
            }
            else if (next.Kind == TokenKind.Dedent)
            {
                _offsets -= 1;
                Console.Write('<');
            }
            else
            {
                if (last.Kind == TokenKind.NewLine)
                {
                    Console.Write('=');
                    Console.Write(new string(' ', _offsets));
                }
                else
                {
                    Console.Write(next.View.AsSpan().ToString());
                }
            }
            last = next;
        }
    }
}