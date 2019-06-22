namespace AdventOfCode2018
{
    public interface ISolution
    {
        int DayN { get; }

        int Part1(string[] input);
        int? Part2(string[] input);
    }
}