using AdventOfCode2018.Common;

namespace AdventOfCode2018
{
    public class Day11 : ISolution
    {
        public int DayN => 11;

        public (string, string) GetAns(string[] input)
        {
            var serial = int.Parse(input[0]);
            var grid = new sbyte[300, 300];
            foreach (var gridLevel in TwoDIterItem<sbyte>.TwoDIter(grid))
            {
                var rackId = gridLevel.Loc.X + 11;
                var powerLevel = rackId * (gridLevel.Loc.Y + 1);
                powerLevel += serial;
                powerLevel *= rackId;
                powerLevel %= 1000;
                powerLevel /= 100;
                powerLevel -= 5;
                gridLevel.Item = (sbyte) powerLevel;
            }

            var (part1, _) = CalcPowerSquare(grid, 3);
            var part2 = (new Vec2(), 0);
            var bestSize = 0;

            for (var i = 1; i < 300; i++)
            {
                var newGrid = CalcPowerSquare(grid, i);
                if (newGrid.Item2 <= part2.Item2) continue;
                part2 = newGrid;
                bestSize = i;
            }

            return ($"{part1.X},{part1.Y}", $"{part2.Item1.X},{part2.Item1.Y},{bestSize}");
        }

        private static (Vec2, int) CalcPowerSquare(sbyte[,] grid, int size)
        {
            var best = new Vec2();
            var gridPower = 0;
            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++)
                gridPower += grid[x, y];
            var greatestPower = gridPower;
            var moveBack = false;
            for (var x = 0; x <= 300 - size; x++)
            {
                if (!moveBack)
                {
                    for (var y = 0; y < 300 - size; y++)
                    {
                        for (var h = x; h < x + size; h++)
                        {
                            gridPower -= grid[h, y];
                            gridPower += grid[h, y + size];
                        }

                        if (gridPower <= greatestPower) continue;
                        best = new Vec2(x, y + 1);
                        greatestPower = gridPower;
                    }

                    if (x != 300 - size)
                    {
                        for (var y = 300 - size; y < 300; y++)
                        {
                            gridPower -= grid[x, y];
                            gridPower += grid[x + size, y];
                        }

                        if (gridPower > greatestPower)
                        {
                            best = new Vec2(x + 1, 300 - size);
                            greatestPower = gridPower;
                        }
                    }
                }
                else
                {
                    for (var y = 299; y >= size; y--)
                    {
                        for (var h = x; h < x + size; h++)
                        {
                            gridPower -= grid[h, y];
                            gridPower += grid[h, y - size];
                        }

                        if (gridPower <= greatestPower) continue;
                        best = new Vec2(x, y - size);
                        greatestPower = gridPower;
                    }

                    if (x != 300 - size)
                    {
                        for (var y = 0; y < size; y++)
                        {
                            gridPower -= grid[x, y];
                            gridPower += grid[x + size, y];
                        }

                        if (gridPower > greatestPower)
                        {
                            best = new Vec2(x + 1, 0);
                            greatestPower = gridPower;
                        }
                    }
                }

                moveBack = !moveBack;
            }

            return (best + new Vec2(1, 1), greatestPower);
        }
    }
}