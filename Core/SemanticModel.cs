using JFomit.Functional;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Core;

class SemanticModel
{
    public HashSet<FunctionDefinition> Functions { get; } = [];

    public Result<UnitValue, string> RegisterFunction()
    {
        return Ok();
    }
}
