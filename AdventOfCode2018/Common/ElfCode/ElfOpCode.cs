using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdventOfCode2018.Common.ElfCode
{
    public class ElfOpCode
    {
        public enum NumGet
        {
            Reg,
            Val,
            Null
        }

        public enum Ops
        {
            Add,
            Mul,
            And,
            Or,
            Set,
            Greater,
            Equal
        }

        static ElfOpCode()
        {
            OpCodes = new ReadOnlyCollection<ElfOpCode>(new[]
            {
                new ElfOpCode("addr", Ops.Add, NumGet.Reg, NumGet.Reg),
                new ElfOpCode("addi", Ops.Add, NumGet.Reg, NumGet.Val),
                new ElfOpCode("mulr", Ops.Mul, NumGet.Reg, NumGet.Reg),
                new ElfOpCode("muli", Ops.Mul, NumGet.Reg, NumGet.Val),
                new ElfOpCode("banr", Ops.And, NumGet.Reg, NumGet.Reg),
                new ElfOpCode("bani", Ops.And, NumGet.Reg, NumGet.Val),
                new ElfOpCode("borr", Ops.Or, NumGet.Reg, NumGet.Reg),
                new ElfOpCode("bori", Ops.Or, NumGet.Reg, NumGet.Val),
                new ElfOpCode("setr", Ops.Set, NumGet.Reg, NumGet.Null),
                new ElfOpCode("seti", Ops.Set, NumGet.Val, NumGet.Null),
                new ElfOpCode("gtir", Ops.Greater, NumGet.Val, NumGet.Reg),
                new ElfOpCode("gtri", Ops.Greater, NumGet.Reg, NumGet.Val),
                new ElfOpCode("gtrr", Ops.Greater, NumGet.Reg, NumGet.Reg),
                new ElfOpCode("eqir", Ops.Equal, NumGet.Val, NumGet.Reg),
                new ElfOpCode("eqri", Ops.Equal, NumGet.Reg, NumGet.Val),
                new ElfOpCode("eqrr", Ops.Equal, NumGet.Reg, NumGet.Reg)
            });
        }

        private ElfOpCode(string name, Ops op, NumGet a, NumGet b)
        {
            Name = name;
            Op = op;
            A = a;
            B = b;
        }

        public string Name { get; }
        public Ops Op { get; }
        public NumGet A { get; }
        public NumGet B { get; }

        public static IReadOnlyList<ElfOpCode> OpCodes { get; }

        public bool IsOpCode(IList<int> before, IList<int> after, int a, int b, int c)
        {
            var executedRegs = new List<int>(before);
            Call(executedRegs, a, b, c);

            return !executedRegs.Where((t, i) => after[i] != t).Any();
        }

        private static int GetNum(IList<int> regs, NumGet type, int code)
        {
            switch (type)
            {
                case NumGet.Reg:
                    return regs[code];
                case NumGet.Val:
                    return code;
                case NumGet.Null:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        private static int ExecOp(Ops op, int a, int b)
        {
            switch (op)
            {
                case Ops.Add:
                    return a + b;
                case Ops.Mul:
                    return a * b;
                case Ops.And:
                    return a & b;
                case Ops.Or:
                    return a | b;
                case Ops.Set:
                    return a;
                case Ops.Greater:
                    return a > b ? 1 : 0;
                case Ops.Equal:
                    return a == b ? 1 : 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        public void Call(IList<int> regs, int a, int b, int c)
        {
            var aVal = GetNum(regs, A, a);
            var bVal = GetNum(regs, B, b);

            regs[c] = ExecOp(Op, aVal, bVal);
        }
    }
}