namespace AdventOfCode2018
{
    public interface ISolution
    {
        int DayN { get; }
        (string, string) GetAns(string[] input);
    }
}