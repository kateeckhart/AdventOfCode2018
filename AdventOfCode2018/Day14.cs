using System.Collections.Generic;

namespace AdventOfCode2018
{
    public class Day14 : ISolution
    {
        public int DayN => 14;

        public (string, string) GetAns(string[] input)
        {
            var scoreBoard = new List<byte>(new byte[] {3, 7});
            var elves = new[] {0, 1};
            var n = int.Parse(input[0]);
            var nDigits = new List<byte>();
            var workingN = n;
            while (workingN > 0)
            {
                nDigits.Add((byte) (workingN % 10));
                workingN /= 10;
            }

            nDigits.Reverse();
            int? digitsToLeft = null;
            var digitsMatched = 0;

            while (scoreBoard.Count < n + 10 || digitsToLeft == null)
            {
                var newRecipe = scoreBoard[elves[0]] + scoreBoard[elves[1]];
                if (newRecipe > 9)
                {
                    var recipe1 = newRecipe / 10;
                    var recipe2 = newRecipe % 10;

                    scoreBoard.Add((byte) recipe1);
                    MatchDigits(ref digitsToLeft, ref digitsMatched, nDigits, recipe1, scoreBoard.Count);
                    scoreBoard.Add((byte) recipe2);
                    MatchDigits(ref digitsToLeft, ref digitsMatched, nDigits, recipe2, scoreBoard.Count);
                }
                else
                {
                    scoreBoard.Add((byte) newRecipe);
                    MatchDigits(ref digitsToLeft, ref digitsMatched, nDigits, newRecipe, scoreBoard.Count);
                }

                for (var j = 0; j < elves.Length; j++)
                {
                    elves[j] += scoreBoard[elves[j]] + 1;
                    elves[j] %= scoreBoard.Count;
                }
            }

            var last10 = 0L;

            for (var i = n; i < n + 10; i++)
            {
                last10 *= 10;
                last10 += scoreBoard[i];
            }

            return (last10.ToString("D10"), digitsToLeft.Value.ToString());
        }

        private static void MatchDigits(ref int? digitsToLeft, ref int digitsMatched, IList<byte> nDigits, int score,
            int count)
        {
            if (digitsToLeft == null && score == nDigits[digitsMatched])
            {
                digitsMatched++;
                if (digitsMatched == nDigits.Count) digitsToLeft = count - nDigits.Count;
            }
            else
            {
                digitsMatched = 0;
            }
        }
    }
}