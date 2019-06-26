using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day9 : ISolution
    {
        private static Regex ParsingRegex { get; } =
            new Regex(@"(?<Players>\d+) players; last marble is worth (?<MarbleN>\d+) points");

        public int DayN => 9;

        public (string, string) GetAns(string[] input)
        {
            var match = ParsingRegex.Match(input[0]);
            var playerN = int.Parse(match.Groups["Players"].Value);
            var marbleN = int.Parse(match.Groups["MarbleN"].Value);
            var players = new long[playerN];
            var currentPlayer = 0;
            var currentMarble = new Marble(0);
            currentMarble.ClockWise = currentMarble;
            currentMarble.CounterClockWise = currentMarble;
            var part1Score = -1L;

            for (var biggestMarble = 1; biggestMarble <= marbleN * 100; biggestMarble++)
            {
                if (biggestMarble % 23 == 0)
                {
                    players[currentPlayer] += biggestMarble;
                    var marble6Counter = currentMarble.CounterClockWise.CounterClockWise.CounterClockWise
                        .CounterClockWise.CounterClockWise.CounterClockWise;
                    var marble7Counter = marble6Counter.CounterClockWise;
                    var marble8Counter = marble7Counter.CounterClockWise;
                    marble6Counter.CounterClockWise = marble8Counter;
                    marble8Counter.ClockWise = marble6Counter;
                    players[currentPlayer] += marble7Counter.Id;
                    currentMarble = marble6Counter;
                }
                else
                {
                    var marble1Clock = currentMarble.ClockWise;
                    var marble2Clock = marble1Clock.ClockWise;
                    var newMarble = new Marble(biggestMarble);
                    marble1Clock.ClockWise = newMarble;
                    newMarble.CounterClockWise = marble1Clock;
                    marble2Clock.CounterClockWise = newMarble;
                    newMarble.ClockWise = marble2Clock;
                    currentMarble = newMarble;
                }

                if (currentPlayer == playerN - 1)
                    currentPlayer = 0;
                else
                    currentPlayer++;

                if (biggestMarble == marbleN) part1Score = players.Max();
            }

            return (part1Score.ToString(), players.Max().ToString());
        }

        private class Marble
        {
            public Marble(int id)
            {
                Id = id;
                ClockWise = this;
                CounterClockWise = this;
            }

            public Marble ClockWise { get; set; }
            public Marble CounterClockWise { get; set; }
            public int Id { get; }
        }
    }
}