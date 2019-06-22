using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
    public class Day1 : ISolution
    {
        public int DayN => 1;

        public int Part1(string[] input)
        {
            return ParseInput(input).Sum();
        }

        public int? Part2(string[] input)
        {
            var frequencies = new HashSet<int>(new[] {0});
            var frequency = 0;

            while (true)
                foreach (var num in ParseInput(input))
                {
                    frequency += num;

                    if (!frequencies.Add(frequency)) return frequency;
                }
        }

        private static IEnumerable<int> ParseInput(IEnumerable<string> input)
        {
            return input.Select(int.Parse);
        }
    }
}