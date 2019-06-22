using System;
using System.IO;

namespace AdventOfCode2018
{
    public static class Common
    {
        public static void Run(ISolution solution)
        {
            var input = File.ReadAllLines($"Input/Day{solution.DayN}.txt");
            Console.WriteLine($"Day{solution.DayN} Part1: {solution.Part1(input)}");
            var part2Out = solution.Part2(input);
            if (part2Out != null) Console.WriteLine($"Day{solution.DayN} Part2: {part2Out}");
        }
    }
}