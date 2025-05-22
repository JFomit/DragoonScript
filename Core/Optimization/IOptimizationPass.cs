using DragoonScript.Core.Ast;

namespace DragoonScript.Core.Optimization;

interface IOptimizationPass
{
    void TransformFunction(FunctionDeclaration function);
}