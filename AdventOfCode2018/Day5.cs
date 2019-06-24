using System.Collections.Generic;

namespace AdventOfCode2018
{
    public class Day5 : ISolution
    {
        public int DayN => 5;

        public (string, string) GetAns(string[] input)
        {
            var size = int.MaxValue;
            for (var letter = 'a'; letter <= 'z'; letter++)
            {
                var newSize = ReactPolymer(input[0], letter);
                if (newSize < size) size = newSize;
            }

            return (ReactPolymer(input[0], null).ToString(), size.ToString());
        }

        private static int ReactPolymer(string polymerIn, char? remove)
        {
            var polymer = new List<char>(polymerIn);
            var index = 0;
            while (index + 1 != polymer.Count)
            {
                var a = polymer[index];
                if (char.ToLower(a) == remove)
                {
                    polymer.RemoveAt(index);
                    if (index > 0)
                    {
                        index--;
                        continue;
                    }
                }

                var b = polymer[index + 1];
                if (a != b && char.ToLower(a) == char.ToLower(b))
                {
                    polymer.RemoveRange(index, 2);
                    if (index > 0)
                    {
                        index--;
                        continue;
                    }
                }

                index++;
            }

            return polymer.Count;
        }
    }
}