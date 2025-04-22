using System.Text;
using DragoonScript.Core.Ast;
using DragoonScript.Utils;
using JFomit.Functional;
using static JFomit.Functional.Prelude;

namespace DragoonScript.Debugging;

class AstPrinter : AstNodeVisitor<string>
{
    private readonly Stack<int> _indents = [];
}