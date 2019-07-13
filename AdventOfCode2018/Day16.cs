using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day16 : ISolution
    {
        private static OpCode[] OpCodes { get; } =
        {
            new OpCode("addr", (regs, a, b, c) => regs[c] = regs[a] + regs[b]),
            new OpCode("addi", (regs, a, b, c) => regs[c] = regs[a] + b),
            new OpCode("mulr", (regs, a, b, c) => regs[c] = regs[a] * regs[b]),
            new OpCode("muli", (regs, a, b, c) => regs[c] = regs[a] * b),
            new OpCode("banr", (regs, a, b, c) => regs[c] = regs[a] & regs[b]),
            new OpCode("bani", (regs, a, b, c) => regs[c] = regs[a] & b),
            new OpCode("borr", (regs, a, b, c) => regs[c] = regs[a] | regs[b]),
            new OpCode("bori", (regs, a, b, c) => regs[c] = regs[a] | b),
            new OpCode("setr", (regs, a, b, c) => regs[c] = regs[a]),
            new OpCode("seti", (regs, a, b, c) => regs[c] = a),
            new OpCode("gtir", (regs, a, b, c) => regs[c] = a > regs[b] ? 1 : 0),
            new OpCode("gtri", (regs, a, b, c) => regs[c] = regs[a] > b ? 1 : 0),
            new OpCode("gtrr", (regs, a, b, c) => regs[c] = regs[a] > regs[b] ? 1 : 0),
            new OpCode("eqir", (regs, a, b, c) => regs[c] = a == regs[b] ? 1 : 0),
            new OpCode("eqri", (regs, a, b, c) => regs[c] = regs[a] == b ? 1 : 0),
            new OpCode("eqrr", (regs, a, b, c) => regs[c] = regs[a] == regs[b] ? 1 : 0)
        };

        private static Regex Before { get; } = new Regex(@"Before: \[(?<digits>\d+(?:, \d+){3})]");
        private static Regex OpCodeParse { get; } = new Regex(@"(?<opCode>\d+) (?<a>\d+) (?<b>\d+) (?<c>\d+)");
        private static Regex After { get; } = new Regex(@"After:  \[(?<digits>\d+(?:, \d+){3})]");

        public int DayN => 16;

        public (string, string) GetAns(string[] input)
        {
            var part1 = 0;
            var regs = new List<int>();
            for (var i = 0; i < 4; i++) regs.Add(0);
            using (var lineIter = ((IEnumerable<string>) input).GetEnumerator())
            {
                if (!lineIter.MoveNext()) throw new ArgumentException();
                var mappingTableBuilder = new List<HashSet<OpCode>>();
                foreach (var _ in OpCodes)
                {
                    var possibleSet = new HashSet<OpCode>();
                    foreach (var opCode in OpCodes) possibleSet.Add(opCode);
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

                    var actsLike = OpCodes.Count(opCode => opCode.IsOpCode(beforeRegs, afterRegs, a, b, c));

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
                                    if (knownOpCode.Func != op.Func) continue;
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

                while (lineIter.MoveNext())
                {
                    var opCodeMatch = OpCodeParse.Match(lineIter.Current);
                    var opCode = mappingTable[int.Parse(opCodeMatch.Groups["opCode"].Value)];
                    var a = int.Parse(opCodeMatch.Groups["a"].Value);
                    var b = int.Parse(opCodeMatch.Groups["b"].Value);
                    var c = int.Parse(opCodeMatch.Groups["c"].Value);
                    opCode.Func(regs, a, b, c);
                }
            }

            return (part1.ToString(), regs[0].ToString());
        }

        private delegate void OpCodeFunc(IList<int> regs, int a, int b, int c);

        private struct OpCode
        {
            public OpCode(string name, OpCodeFunc func)
            {
                Name = name;
                Func = func;
            }

            private string Name { get; }
            public OpCodeFunc Func { get; }

            public bool IsOpCode(IList<int> before, IList<int> after, int a, int b, int c)
            {
                var executedRegs = new List<int>(before);
                Func(executedRegs, a, b, c);

                return !executedRegs.Where((t, i) => after[i] != t).Any();
            }
        }
    }
}