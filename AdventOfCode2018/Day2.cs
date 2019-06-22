using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
    public class Day2 : ISolution
    {
        private const int AmountOfLetters = 'z' - 'a' + 1;
        public int DayN => 2;

        public string Part1(string[] input)
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

            return (appearsTwice * appearsThreeTimes).ToString();
        }

        public string Part2(string[] input)
        {
            var oneOffs = new HashSet<OneOffString>();

            foreach (var id in input)
            foreach (var oneOff in OneOffString.AllOneOffs(id))
                if (!oneOffs.Add(oneOff))
                    return oneOff.OneOff;

            return "";
        }

        private class OneOffString
        {
            private OneOffString(int pos, string str)
            {
                Pos = pos;
                var rawOneOff = new char[str.Length - 1];
                str.CopyTo(0, rawOneOff, 0, pos);
                str.CopyTo(pos + 1, rawOneOff, pos, rawOneOff.Length - pos);

                OneOff = new string(rawOneOff);
            }

            private int Pos { get; }
            public string OneOff { get; }

            public static IEnumerable<OneOffString> AllOneOffs(string str)
            {
                return str.Select((t, i) => new OneOffString(i, str));
            }

            public override bool Equals(object other)
            {
                if (other is OneOffString otherStr) return Pos == otherStr.Pos && OneOff == otherStr.OneOff;

                return false;
            }

            public override int GetHashCode()
            {
                return (Pos, OneOff).GetHashCode();
            }
        }
    }
}