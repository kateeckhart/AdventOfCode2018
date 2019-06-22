namespace AdventOfCode2018
{
    public interface ISolution
    {
        int DayN { get; }

        string Part1(string[] input);

        //Return string is null if unimplemented
        string Part2(string[] input);
    }
}