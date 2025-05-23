using DragoonScript.Core.Ast;

namespace DragoonScript.Core.Transformations;

interface ITransformationPass
{
    void TransformFunction(FunctionDeclaration function);
}