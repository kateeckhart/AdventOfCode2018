using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using AdventOfCode2018.Common.ElfCode;

namespace AdventOfCode2018
{
    public class Day16 : ISolution
    {
        private static Regex Before { get; } = new Regex(@"Before: \[(?<digits>\d+(?:, \d+){3})]");
        private static Regex OpCodeParse { get; } = new Regex(@"(?<opCode>\d+) (?<a>\d+) (?<b>\d+) (?<c>\d+)");
        private static Regex After { get; } = new Regex(@"After:  \[(?<digits>\d+(?:, \d+){3})]");

        public int DayN => 16;

        public (string, string) GetAns(string[] input)
        {
            var part1 = 0;
            using (var lineIter = ((IEnumerable<string>) input).GetEnumerator())
            {
                if (!lineIter.MoveNext()) throw new ArgumentException();
                var mappingTableBuilder = new List<HashSet<ElfOpCode>>();
                foreach (var _ in ElfOpCode.OpCodes)
                {
                    var possibleSet = new HashSet<ElfOpCode>();
                    foreach (var opCode in ElfOpCode.OpCodes) possibleSet.Add(opCode);
                    mappingTableBuilder.Add(possibleSet);
                }

                while (lineIter.Current != "")
                {
                    var beforeMatch = Before.Match(lineIter.Current);
                    var beforeRegs = beforeMatch.Groups["digits"].Value
                        .Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    if (!lineIter.MoveNext()) throw new ArgumentException();
                    var opCodeMatch = OpCodeParse.Match(lineIter.Current);
                    var op = int.Parse(opCodeMatch.Groups["opCode"].Value);
                    var a = int.Parse(opCodeMatch.Groups["a"].Value);
                    var b = int.Parse(opCodeMatch.Groups["b"].Value);
                    var c = int.Parse(opCodeMatch.Groups["c"].Value);
                    if (!lineIter.MoveNext()) throw new ArgumentException();
                    var afterMatch = After.Match(lineIter.Current);
                    var afterRegs = afterMatch.Groups["digits"].Value
                        .Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    if (!lineIter.MoveNext() || lineIter.Current != "") throw new ArgumentException();
                    if (!lineIter.MoveNext()) throw new ArgumentException();

                    var actsLike = ElfOpCode.OpCodes.Count(opCode => opCode.IsOpCode(beforeRegs, afterRegs, a, b, c));

                    mappingTableBuilder[op].RemoveWhere(code => !code.IsOpCode(beforeRegs, afterRegs, a, b, c));

                    if (actsLike >= 3) part1++;
                }

                bool setShank;
                do
                {
                    setShank = false;
                    foreach (var set in mappingTableBuilder)
                    {
                        if (set.Count == 1) continue;
                        var setCount = set.Count;
                        set.RemoveWhere(op =>
                        {
                            foreach (var otherSet in mappingTableBuilder)
                            {
                                if (setCount == 1) break;
                                if (otherSet.Count != 1) continue;
                                using (var singleIter = otherSet.GetEnumerator())
                                {
                                    singleIter.MoveNext();
                                    var knownOpCode = singleIter.Current;
                                    if (knownOpCode != op) continue;
                                    setShank = true;
                                    setCount--;
                                    return true;
                                }
                            }

                            return false;
                        });
                    }
                } while (setShank);

                var mappingTable = mappingTableBuilder.SelectMany(x => x).ToList();

                if (!lineIter.MoveNext() || lineIter.Current != "") throw new ArgumentException();

                var ilGen = new Day16IlGen();
                while (lineIter.MoveNext())
                {
                    var opCodeMatch = OpCodeParse.Match(lineIter.Current);
                    var opCode = mappingTable[int.Parse(opCodeMatch.Groups["opCode"].Value)];
                    var a = int.Parse(opCodeMatch.Groups["a"].Value);
                    var b = int.Parse(opCodeMatch.Groups["b"].Value);
                    var c = int.Parse(opCodeMatch.Groups["c"].Value);
                    ilGen.AddOpcode(opCode, a, b, c);
                }

                return (part1.ToString(), ilGen.Method().ToString());
            }
        }

        private class Day16IlGen : ElfIlGen<Func<int>>
        {
            public Day16IlGen()
            {
                Regs = Gen.DeclareLocal(typeof(IList<int>));
                Gen.EmitInt(4);
                Gen.Emit(OpCodes.Newarr, typeof(int));
                Gen.Emit(OpCodes.Stloc_0);
            }

            private LocalBuilder Regs { get; }

            protected override void EmitAfterOp(int a, int b, int c)
            {
                Gen.Emit(OpCodes.Stelem_I4);
            }

            protected override void EmitBeforeOp(int a, int b, int c)
            {
                Gen.EmitLoadLocal(Regs);
                Gen.EmitInt(c);
            }

            protected override void EmitGetReg(int reg)
            {
                Gen.EmitLoadLocal(Regs);
                Gen.EmitInt(reg);
                Gen.Emit(OpCodes.Ldelem_I4);
            }

            protected override void EmitEnd()
            {
                Gen.EmitLoadLocal(Regs);
                Gen.EmitInt(0);
                Gen.Emit(OpCodes.Ldelem_I4);
                Gen.Emit(OpCodes.Ret);
            }
        }
    }
}