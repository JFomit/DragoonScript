using Compiler.Syntax;
using Compiler.Utils;

namespace Compiler.Core;

class IrVisitor : ParseTreeVisitor<int>
{
    public override int Visit(ParseTree tree)
    {
        throw new NotImplementedException();
    }
    public override int Visit(TokenTree tokenTree)
    {
        throw new NotImplementedException();
    }
}