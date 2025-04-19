using Compiler.Utils;

namespace Compiler.Syntax.Utils;

interface IParseTreeItem
{
    void Accept(ParseTreeVisitor visitor);
}