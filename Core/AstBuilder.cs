using System.Diagnostics;
using DragoonScript.Core.Ast;
using DragoonScript.Syntax;
using JFomit.Functional.Monads;
using System.Linq;

namespace DragoonScript.Core;

class AstBuilder
{
    private readonly FunctionBodyVisitor _bodyVisitor = new();

    public Dictionary<string, FunctionDeclaration> VisitFile(ParseTree tree)
    {
        Debug.Assert(tree.Kind == TreeKind.File);

        return tree.Children
            .Where(child => child.Kind != TreeKind.Token)
            .Select(_bodyVisitor.VisitFunction)
            .ToDictionary(func => func.Name);
    }
}