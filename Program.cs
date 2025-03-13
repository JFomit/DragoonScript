using Compiler.Syntax;

var s = """
type Bool = True | False
let otherwise = True

fn ^ x y
  | y == 0    = 1
  | y == 1    = x
  | y < 0     = 1 / (^ x -y)
  | otherwise = (x * x) ^ (y - 1)

type Option x = Some x | None

fn min a b: int -> int -> int
  | a <= b    = a
  | otherwise = b

type Rational = record
  numerator: int
  denominator: int

fn f x = x ^ 2

fn main = ()
""";
var lexer = new Lexer();

var t = lexer.NextToken(s);
while (t.Type != TokenType.EoF)
{
    while (t.Type == TokenType.WhiteSpace || t.Type == TokenType.NewLine)
    {
        t = lexer.NextToken(s);
    }
    Console.WriteLine($"{t.Type} {t.AsSpan(s)}");
    t = lexer.NextToken(s);
}
Console.WriteLine($"{TokenType.EoF}");