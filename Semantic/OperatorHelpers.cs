using DragoonScript.Syntax;
using DragoonScript.Syntax.Lexing;

namespace DragoonScript.Semantic;

static class OperatorHelpers
{
    public static FunctionReference GetOperatorFunctionRef(TokenTree op, bool isPrefix = false)
        => new(isPrefix ? $"~{op.Stringify()}" : op.Stringify(), FunctionKind.Operator);
}