using System;
using System.Linq;

namespace AdventOfCode2018
{
    public class Day2 : ISolution
    {
        private const int AmountOfLetters = 'z' - 'a' + 1;
        public int DayN => 2;

        public int Part1(string[] input)
        {
            var appearsTwice = 0;
            var appearsThreeTimes = 0;
            foreach (var id in input)
            {
                var letterCounts = new int[AmountOfLetters];
                foreach (var letter in id)
                {
                    if (letter < 'a' || letter > 'z') throw new ArgumentOutOfRangeException();
                    letterCounts[letter - 'a']++;
                }

                if (letterCounts.Contains(2)) appearsTwice++;

                if (letterCounts.Contains(3)) appearsThreeTimes++;
            }

            return appearsTwice * appearsThreeTimes;
        }

        public int? Part2(string[] input)
        {
            return null;
        }
    }
}