using Compiler.Syntax;
using Compiler.Syntax.Lexing;

namespace Compiler.Semantic;

static class OperatorHelpers
{
    public static FunctionReference GetOperatorFunctionRef(TokenTree op, bool isPrefix = false)
        => new(isPrefix ? $"~{op.Stringify()}" : op.Stringify(), FunctionKind.Operator);
}