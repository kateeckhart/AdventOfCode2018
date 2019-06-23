using System;
using System.IO;

namespace AdventOfCode2018
{
    public static class Common
    {
        public static void Run(ISolution solution)
        {
            var input = File.ReadAllLines($"Input/Day{solution.DayN}.txt");
            var (part1, part2) = solution.GetAns(input);
            Console.WriteLine($"Day{solution.DayN} Part1: {part1}");
            if (part2 != null) Console.WriteLine($"Day{solution.DayN} Part2: {part2}");
        }
    }
}