namespace Compiler.Core.Semantic;

class ArrowType(HMType from, HMType to) : HMType
{
    public HMType From { get; } = from;
    public HMType To { get; } = to;

    public static ArrowType CreateCurried(params HMType[] args)
    {
        var span = args.AsSpan();
        return new ArrowType(span[0], CreateCurriedArrow(span[1..]));

        static HMType CreateCurriedArrow(ReadOnlySpan<HMType> args) => args.Length switch
        {
            2 => new ArrowType(args[0], args[1]),
            _ => new ArrowType(args[0], CreateCurriedArrow(args[1..]))
        };
    }
}