using System.Runtime.InteropServices;
using JFomit.Functional.Monads;
using static JFomit.Functional.Prelude;

namespace Compiler.Runtime;

enum OpCode : byte
{
    Nop,
    Halt,

    Add,
    Sub,
    Mul,
    Div,

    LdConst,
    LdClass,
    StClass
}

[StructLayout(LayoutKind.Explicit)]
readonly struct Instruction(OpCode code)
{
    [FieldOffset(0)]
    public readonly OpCode OpCode = code;

    public static Instruction Nop() => new(OpCode.Nop);
    public static Instruction HaltAndCatchFire() => new(OpCode.Halt);

    public static Instruction Add() => new(OpCode.Add);
    public static Instruction Sub() => new(OpCode.Sub);
    public static Instruction Mul() => new(OpCode.Mul);
    public static Instruction Div() => new(OpCode.Div);

    public readonly byte ToByte() => (byte)OpCode;
    public static Option<Instruction> FromByte(ReadOnlySpan<byte> code)
    {
        if (code.Length == 0)
        {
            return None;
        }

        return Some(new Instruction(code: (OpCode)code[0]));
    }
    public static Instruction FromByteUnsafe(ReadOnlySpan<byte> code) => new((OpCode)code[0]);
}