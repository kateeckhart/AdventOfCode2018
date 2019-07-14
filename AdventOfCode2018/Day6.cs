using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2018.Common;

namespace AdventOfCode2018
{
    public class Day6 : ISolution
    {
        private static Regex CordParse { get; } = new Regex(@"(?<X>\d+), (?<Y>\d+)");
        public int DayN => 6;

        public (string, string) GetAns(string[] input)
        {
            var cords = new Vec2[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                var match = CordParse.Match(input[i]);
                var x = int.Parse(match.Groups["X"].Value);
                var y = int.Parse(match.Groups["Y"].Value);
                cords[i] = new Vec2(x, y);
            }

            var widthOfGrid = 0;
            var heightOfGrid = 0;
            foreach (var cord in cords)
            {
                if (cord.X > widthOfGrid) widthOfGrid = cord.X;

                if (cord.Y > heightOfGrid) heightOfGrid = cord.Y;
            }

            widthOfGrid += 2;
            heightOfGrid += 2;

            var grid = new GridSquare[widthOfGrid, heightOfGrid];
            for (var h = 0; h < widthOfGrid; h++)
            for (var v = 0; v < heightOfGrid; v++)
                grid[h, v] = new GridSquare(new List<EmitterDistance>(cords.Length))
                {
                    Closest = new EmitterDistance(null, int.MaxValue)
                };

            var emitters = new List<Emitter>(cords.Length);

            foreach (var rawCord in cords)
            {
                var x = rawCord.X + 1;
                var y = rawCord.Y + 1;
                var cord = new Vec2(x, y);
                var emitter = new Emitter();
                emitters.Add(emitter);

                for (var h = 0; h < widthOfGrid; h++)
                for (var v = 0; v < heightOfGrid; v++)
                {
                    var distanceTo = cord.DistanceTo(new Vec2(h, v));
                    var emitterDistance = new EmitterDistance(emitter, distanceTo);
                    grid[h, v].Distances.Add(emitterDistance);
                    if (distanceTo < grid[h, v].Closest.DistanceTo)
                    {
                        grid[h, v].Closest = emitterDistance;
                        grid[h, v].Tie = false;
                    }
                    else if (distanceTo == grid[h, v].Closest.DistanceTo)
                    {
                        grid[h, v].Tie = true;
                    }
                }
            }

            var biggestArea = 0;

            foreach (var emitter in emitters)
            {
                if (IsInfiniteEmitter(emitter, grid)) continue;

                var area = 0;
                for (var h = 0; h < widthOfGrid; h++)
                for (var v = 0; v < heightOfGrid; v++)
                    if (!grid[h, v].Tie && grid[h, v].Closest.Emit == emitter)
                        area++;

                if (area > biggestArea) biggestArea = area;
            }

            var safeArea = 0;

            for (var h = 1; h < widthOfGrid - 1; h++)
            for (var v = 1; v < heightOfGrid - 1; v++)
            {
                var totalDistance = grid[h, v].Distances.Select(distance => distance.DistanceTo).Sum();
                if (totalDistance < 10000) safeArea++;
            }

            return (biggestArea.ToString(), safeArea.ToString());
        }

        private static bool IsInfiniteEmitter(Emitter emitter, GridSquare[,] grid)
        {
            var widthOfGrid = grid.GetLength(0);
            var heightOfGrid = grid.GetLength(1);
            for (var h = 0; h < widthOfGrid; h++)
            {
                if (!grid[h, 0].Tie && grid[h, 0].Closest.Emit == emitter) return true;

                if (!grid[h, heightOfGrid - 1].Tie && grid[h, heightOfGrid - 1].Closest.Emit == emitter) return true;
            }

            for (var v = 0; v < heightOfGrid; v++)
            {
                if (!grid[0, v].Tie && grid[0, v].Closest.Emit == emitter) return true;

                if (!grid[widthOfGrid - 1, v].Tie && grid[widthOfGrid - 1, v].Closest.Emit == emitter) return true;
            }

            return false;
        }

        private struct EmitterDistance
        {
            public EmitterDistance(Emitter emit, int distanceTo)
            {
                Emit = emit;
                DistanceTo = distanceTo;
            }

            public Emitter Emit { get; }
            public int DistanceTo { get; }
        }

        private struct GridSquare
        {
            public GridSquare(List<EmitterDistance> distances) : this()
            {
                Distances = distances;
            }

            public EmitterDistance Closest { get; set; }
            public bool Tie { get; set; }

            public List<EmitterDistance> Distances { get; }
        }

        private class Emitter
        {
        }
    }
}