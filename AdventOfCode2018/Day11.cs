namespace AdventOfCode2018
{
    public class Day11 : ISolution
    {
        public int DayN => 11;

        public (string, string) GetAns(string[] input)
        {
            var serial = int.Parse(input[0]);
            var grid = new sbyte[300, 300];
            for (var x = 0; x < 300; x++)
            for (var y = 0; y < 300; y++)
            {
                var rackId = x + 11;
                var powerLevel = rackId * (y + 1);
                powerLevel += serial;
                powerLevel *= rackId;
                powerLevel %= 1000;
                powerLevel /= 100;
                powerLevel -= 5;
                grid[x, y] = (sbyte) powerLevel;
            }

            var ((part1X, part1Y), _) = CalcPowerSquare(grid, 3);
            var part2 = ((0, 0), 0);
            var bestSize = 0;

            for (var i = 1; i < 300; i++)
            {
                var newGrid = CalcPowerSquare(grid, i);
                if (newGrid.Item2 <= part2.Item2) continue;
                part2 = newGrid;
                bestSize = i;
            }

            return ($"{part1X},{part1Y}", $"{part2.Item1.Item1},{part2.Item1.Item2},{bestSize}");
        }

        private static ((int, int), int) CalcPowerSquare(sbyte[,] grid, int size)
        {
            var bestX = 0;
            var bestY = 0;
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
                        bestX = x;
                        bestY = y + 1;
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
                            bestX = x + 1;
                            bestY = 300 - size;
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
                        bestX = x;
                        bestY = y - size;
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
                            bestX = x + 1;
                            bestY = 0;
                            greatestPower = gridPower;
                        }
                    }
                }

                moveBack = !moveBack;
            }

            return ((bestX + 1, bestY + 1), greatestPower);
        }
    }
}