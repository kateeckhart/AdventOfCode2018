namespace AdventOfCode2018
{
    public static class Program
    {
        public static void Main(string[] input)
        {
            var solutions = new ISolution[]
            {
                new Day1(), new Day2(), new Day3(), new Day4(), new Day5(), new Day6(), new Day7(), new Day8(),
                new Day9(), new Day10(), new Day11()
            };
            foreach (var solution in solutions) Common.Run(solution);
        }
    }
}