using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2018.Common.ElfCode
{
    public struct ElfInstruction
    {
        public ElfInstruction(ElfOpCode op, int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
            Op = op;
        }

        private static Regex PcRegex { get; } = new Regex(@"#ip (?<pc>\d+)");
        private static Regex InstructionRegex { get; } = new Regex(@"(?<name>\w+) (?<a>\d+) (?<b>\d+) (?<c>\d+)");

        public static int GetPcReg(string reg)
        {
            var match = PcRegex.Match(reg);
            return int.Parse(match.Groups["pc"].Value);
        }

        public static ElfInstruction ParseInstruction(string line)
        {
            var match = InstructionRegex.Match(line);
            var name = match.Groups["name"].Value;
            var a = int.Parse(match.Groups["a"].Value);
            var b = int.Parse(match.Groups["b"].Value);
            var c = int.Parse(match.Groups["c"].Value);

            var opCode = ElfOpCode.OpCodes.First(op => op.Name == name);
            return new ElfInstruction(opCode, a, b, c);
        }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public ElfOpCode Op { get; set; }
    }
}