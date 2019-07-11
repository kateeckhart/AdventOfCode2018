using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
    public class Day15 : ISolution
    {
        public int DayN => 15;

        public (string, string) GetAns(string[] input)
        {
            var deadElves = true;
            var value = 0;
            var attack = 4;
            while (deadElves)
            {
                var ans = GetAns(input, attack);
                value = ans.Item1;
                deadElves = ans.Item2;

                attack++;
            }

            return (GetAns(input, 3).Item1.ToString(), value.ToString());
        }

        private static (int, bool) GetAns(string[] input, int attack)
        {
            var grid = new Tile[input[0].Length, input.Length];
            var state = new State(grid);
            for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[0].Length; x++)
            {
                Tile newTile;
                switch (input[y][x])
                {
                    case '#':
                        newTile = Wall.Instance;
                        break;
                    case '.':
                        newTile = Empty.Instance;
                        break;
                    case 'E':
                        newTile = new Elf(attack);
                        state.ElfCount++;
                        break;
                    case 'G':
                        newTile = new Goblin();
                        state.GoblinCount++;
                        break;
                    default:
                        throw new ArgumentException();
                }

                grid[x, y] = newTile;
            }

            var initialElves = state.ElfCount;
            var tick = 0;
            while (true)
            {
                tick++;

                for (var y = 0; y < input.Length; y++)
                for (var x = 0; x < input[0].Length; x++)
                {
                    if (grid[x, y].Process(state, tick, x, y)) goto endLoop;
                    if (attack > 3 && initialElves != state.ElfCount) return (0, true);
                }
            }

            endLoop:
            tick--;
            var totalHp = 0;
            for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[0].Length; x++)
                if (state.Grid[x, y] is Unit unit)
                    totalHp += unit.Hp;

            return (tick * totalHp, initialElves != state.ElfCount);
        }

        private class State
        {
            public State(Tile[,] grid)
            {
                Grid = grid;
            }

            public int GoblinCount { get; set; }
            public int ElfCount { get; set; }

            public Tile[,] Grid { get; }
        }

        private abstract class Tile
        {
            public virtual bool Process(State state, int tick, int x, int y)
            {
                return false;
            }
        }

        private class Empty : Tile
        {
            private Empty()
            {
            }

            public static Empty Instance { get; } = new Empty();
        }

        private class Wall : Tile
        {
            private Wall()
            {
            }

            public static Wall Instance { get; } = new Wall();
        }

        private abstract class Unit : Tile
        {
            protected Unit(int attackPower)
            {
                AttackPower = attackPower;
            }

            public int Hp { get; private set; } = 200;
            private int AttackPower { get; }
            private int Tick { get; set; }

            private bool IsEnemy(Unit other)
            {
                return GetType() != other.GetType();
            }

            private static int[,] CalculateDistance(Tile[,] grid, int x, int y)
            {
                var ret = new int[grid.GetLength(0), grid.GetLength(1)];
                for (var h = 0; h < grid.GetLength(0); h++)
                for (var v = 0; v < grid.GetLength(1); v++)
                    ret[h, v] = int.MaxValue;

                ret[x, y] = 0;
                var nodes = new List<(int, int)>(grid.Length) {(x, y)};

                for (var h = 0; h < grid.GetLength(0); h++)
                for (var v = 0; v < grid.GetLength(1); v++)
                {
                    if (!(grid[h, v] is Empty)) continue;
                    nodes.Add((h, v));
                }

                while (nodes.Count > 0)
                {
                    var closestIndex = 0;
                    var closestDistance = int.MaxValue - 1;
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        if (ret[nodes[i].Item1, nodes[i].Item2] >= closestDistance) continue;
                        closestIndex = i;
                        closestDistance = ret[nodes[i].Item1, nodes[i].Item2];
                    }

                    var (closestX, closestY) = nodes[closestIndex];
                    nodes.RemoveAt(closestIndex);

                    void TestDistance(int closeX, int closeY)
                    {
                        if (ret[closeX, closeY] > closestDistance + 1 && grid[closeX, closeY] is Empty)
                            ret[closeX, closeY] = closestDistance + 1;
                    }

                    TestDistance(closestX - 1, closestY);
                    TestDistance(closestX + 1, closestY);
                    TestDistance(closestX, closestY - 1);
                    TestDistance(closestX, closestY + 1);
                }

                return ret;
            }

            private static List<(int, int)> CalcPaths(int[,] distance, int x, int y)
            {
                var shortestPaths = new List<(int, int)>();
                var currentDistance = distance[x, y];
                if (currentDistance == 1)
                {
                    shortestPaths.Add((x, y));
                    return shortestPaths;
                }

                var shortestDistance = int.MaxValue;
                var shortestNodes = new List<(int, int)>();

                void TestDistance(int h, int v)
                {
                    if (distance[h, v] > shortestDistance) return;
                    if (distance[h, v] != shortestDistance) shortestNodes.Clear();

                    shortestDistance = distance[h, v];
                    shortestNodes.Add((h, v));
                }

                TestDistance(x - 1, y);
                TestDistance(x + 1, y);
                TestDistance(x, y - 1);
                TestDistance(x, y + 1);

                foreach (var (h, v) in shortestNodes) shortestPaths.AddRange(CalcPaths(distance, h, v));

                return new List<(int, int)>(shortestPaths.Distinct());
            }

            protected abstract void KillEnemy(State state);

            private static int SortReadOrder((int, int) first, (int, int) second)
            {
                var yCompare = first.Item2.CompareTo(second.Item2);
                return yCompare == 0 ? first.Item1.CompareTo(second.Item1) : yCompare;
            }

            public override bool Process(State state, int tick, int thisX, int thisY)
            {
                base.Process(state, tick, thisX, thisY);
                if (Tick >= tick) return false;
                if (state.ElfCount == 0 || state.GoblinCount == 0) return true;
                Tick = tick;
                var distanceGrid = CalculateDistance(state.Grid, thisX, thisY);
                var closestEnemyDistance = int.MaxValue;
                int? closestEnemyY = null;
                int? closestEnemyX = null;

                IEnumerable<(int, int)> Adjacent(int x, int y, Func<Tile, bool> isValid)
                {
                    if (isValid(state.Grid[x, y - 1])) yield return (x, y - 1);

                    if (isValid(state.Grid[x - 1, y])) yield return (x - 1, y);

                    if (isValid(state.Grid[x + 1, y])) yield return (x + 1, y);

                    if (isValid(state.Grid[x, y + 1])) yield return (x, y + 1);
                }

                var adjacentToEnemy = new List<(int, int)>();
                for (var y = 0; y < state.Grid.GetLength(1); y++)
                for (var x = 0; x < state.Grid.GetLength(0); x++)
                {
                    if (!(state.Grid[x, y] is Unit && IsEnemy((Unit) state.Grid[x, y]))) continue;
                    adjacentToEnemy.AddRange(Adjacent(x, y, tile => tile is Empty || tile == this));
                }

                adjacentToEnemy.Sort(SortReadOrder);

                foreach (var (h, v) in adjacentToEnemy)
                {
                    if (distanceGrid[h, v] >= closestEnemyDistance) continue;
                    closestEnemyY = v;
                    closestEnemyX = h;
                    closestEnemyDistance = distanceGrid[h, v];
                }

                if (closestEnemyX == null) return false;

                if (closestEnemyDistance > 0)
                {
                    var paths = CalcPaths(distanceGrid, closestEnemyX.Value,
                        closestEnemyY.Value);
                    paths.Sort(SortReadOrder);

                    state.Grid[thisX, thisY] = Empty.Instance;
                    thisX = paths[0].Item1;
                    thisY = paths[0].Item2;
                    state.Grid[thisX, thisY] = this;
                }

                var lowestHp = int.MaxValue;
                int? lowestHpX = null;
                int? lowestHpY = null;
                foreach (var (enemyX, enemyY) in Adjacent(thisX, thisY,
                    tile => tile is Unit unit && IsEnemy(unit)))
                {
                    var enemy = (Unit) state.Grid[enemyX, enemyY];
                    if (enemy.Hp >= lowestHp) continue;
                    lowestHp = enemy.Hp;
                    lowestHpX = enemyX;
                    lowestHpY = enemyY;
                }

                if (lowestHpX == null) return false;
                var target = (Unit) state.Grid[lowestHpX.Value, lowestHpY.Value];
                target.Hp -= AttackPower;
                if (target.Hp > 0) return false;
                state.Grid[lowestHpX.Value, lowestHpY.Value] = Empty.Instance;
                KillEnemy(state);
                return false;
            }
        }

        private class Elf : Unit
        {
            public Elf(int attackPower) : base(attackPower)
            {
            }

            protected override void KillEnemy(State state)
            {
                state.GoblinCount--;
            }
        }

        private class Goblin : Unit
        {
            public Goblin() : base(3)
            {
            }

            protected override void KillEnemy(State state)
            {
                state.ElfCount--;
            }
        }
    }
}