namespace Compiler.Syntax;

interface IParseTreeItem
{
    void Accept(ParseTreeVisitor visitor);
}