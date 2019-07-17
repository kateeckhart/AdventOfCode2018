using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2018.Common;

namespace AdventOfCode2018
{
    public class Day17 : ISolution
    {
        private static Regex ParsingRegex { get; } =
            new Regex(@"(?<dir1>x|y)=(?<dir1Num>\d+), (?<dir2>x|y)=(?<dir2start>\d+)..(?<dir2end>\d+)");

        public int DayN => 17;

        public (string, string) GetAns(string[] input)
        {
            var lowestX = int.MaxValue;
            var lowestY = int.MaxValue;
            var highestX = 0;
            var highestY = 0;
            foreach (var point in ParseInput(input))
            {
                if (point.X < lowestX) lowestX = point.X;
                if (point.X > highestX) highestX = point.X;
                if (point.Y < lowestY) lowestY = point.Y;
                if (point.Y > highestY) highestY = point.Y;
            }

            var grid = new Tile[highestX - lowestX + 3, highestY - lowestY + 1];

            foreach (var point in ParseInput(input)) grid[point.X - lowestX + 1, point.Y - lowestY] = Tile.Clay;

            var pendingWater = new Queue<Vec2>();
            pendingWater.Enqueue(new Vec2(501 - lowestX, 0));

            while (pendingWater.TryDequeue(out var water))
            {
                while (true)
                {
                    if (water.Y >= grid.GetLength(1)) goto nextWater;

                    ref var tile = ref grid[water.X, water.Y];
                    if (TileIsSupport(tile)) break;

                    tile = Tile.WetSand;
                    water.Y++;
                }

                water.Y--;
                var leftBound = water.X;
                var canSettle = true;
                while (!TileIsSupport(grid[leftBound, water.Y]))
                {
                    grid[leftBound, water.Y] = Tile.WetSand;
                    if (!TileIsSupport(grid[leftBound, water.Y + 1]))
                    {
                        canSettle = false;
                        break;
                    }

                    leftBound--;
                }

                var rightBound = water.X;
                while (!TileIsSupport(grid[rightBound, water.Y]))
                {
                    grid[rightBound, water.Y] = Tile.WetSand;
                    if (!TileIsSupport(grid[rightBound, water.Y + 1]))
                    {
                        canSettle = false;
                        break;
                    }

                    rightBound++;
                }

                if (canSettle)
                {
                    for (var i = leftBound + 1; i < rightBound; i++) grid[i, water.Y] = Tile.StillWater;
                    pendingWater.Enqueue(water + new Vec2(0, -1));
                }
                else
                {
                    if (!TileIsSupport(grid[leftBound, water.Y + 1]))
                        pendingWater.Enqueue(new Vec2(leftBound, water.Y));

                    if (!TileIsSupport(grid[rightBound, water.Y + 1]))
                        pendingWater.Enqueue(new Vec2(rightBound, water.Y));
                }

                nextWater: ;
            }

            var wetSandSum = grid.Cast<Tile>().Count(tile => tile == Tile.StillWater || tile == Tile.WetSand);
            var stillWaterSum = grid.Cast<Tile>().Count(tile => tile == Tile.StillWater);

            return (wetSandSum.ToString(), stillWaterSum.ToString());
        }

        private static IEnumerable<Vec2> ParseInput(IEnumerable<string> input)
        {
            foreach (var line in input)
            {
                var match = ParsingRegex.Match(line);

                if (match.Groups["dir1"].Value == match.Groups["dir2"].Value) throw new ArgumentException();

                var fixedNum = int.Parse(match.Groups["dir1Num"].Value);

                var start = int.Parse(match.Groups["dir2start"].Value);
                var end = int.Parse(match.Groups["dir2end"].Value);

                var isX = match.Groups["dir1"].Value == "x";

                for (var i = start; i <= end; i++)
                    if (isX)
                        yield return new Vec2(fixedNum, i);
                    else
                        yield return new Vec2(i, fixedNum);
            }
        }

        private static bool TileIsSupport(Tile tile)
        {
            return tile == Tile.Clay || tile == Tile.StillWater;
        }

        private enum Tile
        {
            DrySand = 0,
            WetSand,
            StillWater,
            Clay
        }
    }
}