namespace AdventOfCode2018
{
    public static class Program
    {
        public static void Main(string[] input)
        {
            var solutions = new ISolution[] {new Day1(), new Day2(), new Day3(), new Day4(), new Day5()};
            foreach (var solution in solutions) Common.Run(solution);
        }
    }
}