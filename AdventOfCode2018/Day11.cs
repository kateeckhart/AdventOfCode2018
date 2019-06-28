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
            for (var x = 0; x < 300 - size; x += 2)
            {
                for (var y = size; y < 300; y++)
                {
                    for (var h = x; h < x + size; h++)
                    {
                        gridPower -= grid[h, y - size];
                        gridPower += grid[h, y];
                    }

                    if (gridPower <= greatestPower) continue;
                    bestX = x;
                    bestY = y - size + 1;
                    greatestPower = gridPower;
                }

                for (var y = 300 - size; y < 300; y++)
                {
                    gridPower -= grid[x, y];
                    gridPower += grid[x + size, y];
                }

                if (gridPower > greatestPower)
                {
                    bestX = x + 1;
                    bestY = 299 - size;
                }

                if (x != 299 - size)
                    for (var y = 299 - size; y >= 0; y--)
                    {
                        for (var h = x + 1; h < x + size + 1; h++)
                        {
                            gridPower -= grid[h, y + size];
                            gridPower += grid[h, y];
                        }

                        if (gridPower <= greatestPower) continue;
                        bestX = x + 1;
                        bestY = y - size + 1;
                        greatestPower = gridPower;
                    }

                if (x == 300 - size || x == 299 - size) continue;
                for (var y = 0; y < size; y++)
                {
                    gridPower -= grid[x + 1, y];
                    gridPower += grid[x + size + 1, y];
                }

                if (gridPower <= greatestPower) continue;
                bestX = x + 2;
                bestY = 0;
            }

            return ((bestX + 1, bestY + 1), greatestPower);
        }
    }
}