using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Core;

class FunctionDefinition(string name, FunctionBodyExpression[] body)
{
    public string Name { get; } = name;
    public FunctionBodyExpression[] Body { get; } = body;
}
