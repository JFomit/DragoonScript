namespace Compiler.Core;

abstract record GeneralValue;
abstract record AtomicValue : GeneralValue;
record LiteralAtomic;
record VariableAtomic;

record FunctionCall(string FunctionName, params AtomicValue[] Args) : GeneralValue;

abstract record FunctionBodyExpression;
record ReturnExpression(AtomicValue ToReturn) : FunctionBodyExpression;
record BindingExpression(string LocalVariable, GeneralValue Value) : FunctionBodyExpression;
