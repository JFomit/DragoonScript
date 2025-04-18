namespace Compiler.Core;

abstract record AtomicValue;
record ConstantAtomic : AtomicValue;
record VariableAtomic : AtomicValue;

abstract record ValueExpression;
record CallExpression(AtomicValue Function, params AtomicValue[] Argument) : ValueExpression;
record LetBinding(VariableAtomic Variable, ValueExpression Expression);
