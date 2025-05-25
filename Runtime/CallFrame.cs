using DragoonScript.Core.Ast;

namespace DragoonScript.Runtime;

record struct CallFrame(LambdaTerm ReturnTarget, Callable Function);