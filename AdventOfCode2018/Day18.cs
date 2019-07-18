using System;
using System.Linq;
using AdventOfCode2018.Common;

namespace AdventOfCode2018
{
    public class Day18 : ISolution
    {
        public int DayN => 18;

        public (string, string) GetAns(string[] input)
        {
            var origGrid = new Acre[input[0].Length, input.Length];

            for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[0].Length; x++)
            {
                Acre tile;
                switch (input[y][x])
                {
                    case '.':
                        tile = Acre.Open;
                        break;
                    case '|':
                        tile = Acre.Tree;
                        break;
                    case '#':
                        tile = Acre.LumberYard;
                        break;
                    default:
                        throw new ArgumentException();
                }

                origGrid[x, y] = tile;
            }

            var turtle = new Acre[origGrid.GetLength(0), origGrid.GetLength(1)];
            foreach (var tile in TwoDIterItem<Acre>.TwoDIter(turtle)) tile.Item = origGrid[tile.Loc.X, tile.Loc.Y];

            turtle = NextGrid(turtle);

            var hare = new Acre[origGrid.GetLength(0), origGrid.GetLength(1)];
            foreach (var tile in TwoDIterItem<Acre>.TwoDIter(hare)) tile.Item = origGrid[tile.Loc.X, tile.Loc.Y];

            hare = NextGrid(hare);
            hare = NextGrid(hare);
            int? part1 = null;
            var steps = 0;

            while (TwoDIterItem<Acre>.TwoDIter(hare).Any(tile => tile.Item != turtle[tile.Loc.X, tile.Loc.Y]))
            {
                hare = NextGrid(hare);
                hare = NextGrid(hare);

                turtle = NextGrid(turtle);
                steps++;
                if (steps == 9) part1 = SumGrid(turtle);
            }

            foreach (var tile in TwoDIterItem<Acre>.TwoDIter(turtle)) tile.Item = origGrid[tile.Loc.X, tile.Loc.Y];

            var distanceToCycle = 0;

            while (TwoDIterItem<Acre>.TwoDIter(hare).Any(tile => tile.Item != turtle[tile.Loc.X, tile.Loc.Y]))
            {
                hare = NextGrid(hare);
                turtle = NextGrid(turtle);
                distanceToCycle++;
            }

            var period = 1;
            hare = NextGrid(turtle);
            while (TwoDIterItem<Acre>.TwoDIter(hare).Any(tile => tile.Item != turtle[tile.Loc.X, tile.Loc.Y]))
            {
                hare = NextGrid(hare);
                period++;
            }

            foreach (var tile in TwoDIterItem<Acre>.TwoDIter(turtle)) tile.Item = origGrid[tile.Loc.X, tile.Loc.Y];

            for (var i = 0; i < distanceToCycle; i++) turtle = NextGrid(turtle);

            var distanceFromCycle = (1000000000 - distanceToCycle) % period;

            for (var i = 0; i < distanceFromCycle; i++) turtle = NextGrid(turtle);

            return (part1.ToString(), SumGrid(turtle).ToString());
        }

        private static int GetAdjacent(Acre[,] grid, Vec2 pos, Acre type)
        {
            var sum = 0;
            if (pos.X - 1 >= 0)
            {
                if (grid[pos.X - 1, pos.Y] == type) sum++;
                if (pos.Y - 1 >= 0 && grid[pos.X - 1, pos.Y - 1] == type) sum++;
                if (pos.Y + 1 < grid.GetLength(1) && grid[pos.X - 1, pos.Y + 1] == type) sum++;
            }

            if (pos.Y - 1 >= 0 && grid[pos.X, pos.Y - 1] == type) sum++;
            if (pos.Y + 1 < grid.GetLength(1) && grid[pos.X, pos.Y + 1] == type) sum++;

            if (pos.X + 1 >= grid.GetLength(0)) return sum;
            if (grid[pos.X + 1, pos.Y] == type) sum++;
            if (pos.Y - 1 >= 0 && grid[pos.X + 1, pos.Y - 1] == type) sum++;
            if (pos.Y + 1 < grid.GetLength(1) && grid[pos.X + 1, pos.Y + 1] == type) sum++;

            return sum;
        }

        private static Acre NextAcre(Acre[,] grid, Vec2 pos)
        {
            switch (grid[pos.X, pos.Y])
            {
                case Acre.Open:
                    return GetAdjacent(grid, pos, Acre.Tree) >= 3 ? Acre.Tree : Acre.Open;
                case Acre.Tree:
                    return GetAdjacent(grid, pos, Acre.LumberYard) >= 3 ? Acre.LumberYard : Acre.Tree;
                case Acre.LumberYard:
                    if (GetAdjacent(grid, pos, Acre.Tree) >= 1 && GetAdjacent(grid, pos, Acre.LumberYard) >= 1)
                        return Acre.LumberYard;
                    return Acre.Open;
                default:
                    throw new ArgumentException();
            }
        }

        private static Acre[,] NextGrid(Acre[,] grid)
        {
            var newGrid = new Acre[grid.GetLength(0), grid.GetLength(1)];
            foreach (var tile in TwoDIterItem<Acre>.TwoDIter(newGrid)) tile.Item = NextAcre(grid, tile.Loc);

            return newGrid;
        }

        private static int SumGrid(Acre[,] grid)
        {
            var numberOfTrees = 0;
            var numberOfYards = 0;
            foreach (var tile in TwoDIterItem<Acre>.TwoDIter(grid))
                switch (tile.Item)
                {
                    case Acre.Tree:
                        numberOfTrees++;
                        break;
                    case Acre.LumberYard:
                        numberOfYards++;
                        break;
                }

            return numberOfTrees * numberOfYards;
        }

        private enum Acre : byte
        {
            Open = 0,
            Tree,
            LumberYard
        }
    }
}