namespace DragoonScript.Semantic;

enum FunctionKind
{
    Callable,
    Operator
}

record FunctionReference(string Name, FunctionKind Kind = FunctionKind.Callable);