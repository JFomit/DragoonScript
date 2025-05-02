using DragoonScript.Core.Ast;
using DragoonScript.Debugging;
using JetBrains.Annotations;
using JFomit.Functional;

namespace DragoonScript.Runtime;

class Closure(Func<Interpreter, object[], object> function)
{
    public Func<Interpreter, object[], object> Function { get; } = function;

    public object Call(Interpreter interpreter, object[] args) => Function(interpreter, args);

    public static Closure FromDelegate(Func<Unit> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 0)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func();
        });
    }

    public static Closure FromDelegate<TResult>(Func<TResult> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 0)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            return func()!;
        });
    }
    public static Closure FromDelegate<T1, TResult>(Func<T1, TResult> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 1)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            if (args.Length == 0)
            {
                throw new InvalidOperationException("Too few arguments provided.");
            }
            return func((T1)args[0])!;
        });
    }
    public static Closure FromDelegate<T1, T2, TResult>(Func<T1, T2, TResult> func)
    {
        return new((_, args) =>
        {
            if (args.Length > 2)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            if (args.Length == 0)
            {
                throw new InvalidOperationException("Too few arguments provided.");
            }
            if (args.Length == 1)
            {
                return FromDelegate<T2, TResult>(next =>
                {
                    return func((T1)args[0], next);
                });
            }
            return func((T1)args[0], (T2)args[1])!;
        });
    }

    public static Closure FromDeclaration(FunctionDeclaration declaration) => FromDeclarationCurried(declaration, []);
    private static Closure FromDeclarationCurried(FunctionDeclaration declaration, object[] oldArgs)
    {
        var parameters = declaration.Parameters;
        return new((interpreter, otherArgs) =>
        {
            var args = new object[oldArgs.Length + otherArgs.Length];
            Array.Copy(oldArgs, args, oldArgs.Length);
            Array.Copy(otherArgs, 0, args, oldArgs.Length, otherArgs.Length);

            if (args.Length > parameters.Length)
            {
                throw new InvalidOperationException("Extra arguments.");
            }
            if (args.Length == parameters.Length)
            {
                interpreter.PushScope();
                for (int i = 0; i < parameters.Length; i++)
                {
                    Variable? item = parameters[i];
                    interpreter.Scope.UpdateWithShadow(item.Name, args[i]);
                }
                var result = interpreter.Visit(declaration);
                interpreter.PopScope();
                return result;
            }

            return FromDeclarationCurried(declaration, args);
        });
    }

    public static Closure FromLambda(Abstraction abstraction, object[]? oldArgs = null)
    {
        var parameters = abstraction.Variables;
        var body = abstraction.Body;

        return new((interpreter, otherArgs) =>
        {
            object[] args;
            if (oldArgs is null)
            {
                args = otherArgs;
            }
            else
            {
                args = new object[oldArgs.Length + otherArgs.Length];
                Array.Copy(oldArgs, args, oldArgs.Length);
                Array.Copy(otherArgs, 0, args, oldArgs.Length, otherArgs.Length);
            }

            if (args.Length > parameters.Length)
            {
                throw new InvalidOperationException("Extra arguments.");
            }

            if (args.Length == parameters.Length)
            {
                interpreter.PushScope();
                for (int i = 0; i < args.Length; i++)
                {
                    var item = parameters[i];
                    interpreter.Scope.UpdateWithShadow(item.Name, args[i]);
                }
                var result = interpreter.Visit(body);
                interpreter.PopScope();
                return result;
            }

            return FromLambda(abstraction, args);
        });
    }
}
