using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2018.Common;

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
            foreach (var tile in TwoDIterItem<Tile>.TwoDIter(grid))
                switch (input[tile.Loc.Y][tile.Loc.X])
                {
                    case '#':
                        tile.Item = Wall.Instance;
                        break;
                    case '.':
                        tile.Item = Empty.Instance;
                        break;
                    case 'E':
                        tile.Item = new Elf(attack);
                        state.ElfCount++;
                        break;
                    case 'G':
                        tile.Item = new Goblin();
                        state.GoblinCount++;
                        break;
                    default:
                        throw new ArgumentException();
                }

            var initialElves = state.ElfCount;
            var tick = 0;
            while (true)
            {
                tick++;

                foreach (var tile in TwoDIterItem<Tile>.TwoDIter(grid))
                {
                    if (tile.Item.Process(state, tick, tile.Loc)) goto endLoop;
                    if (attack > 3 && initialElves != state.ElfCount) return (0, true);
                }
            }

            endLoop:
            tick--;
            var totalHp = 0;
            foreach (var tile in TwoDIterItem<Tile>.TwoDIter(grid))
                if (tile.Item is Unit unit)
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
            public virtual bool Process(State state, int tick, Vec2 loc)
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

            private static int[,] CalculateDistance(Tile[,] grid, Vec2 loc)
            {
                var queue = new PriorityQueue<DistanceNode>();
                var nodes = new DistanceNode[grid.GetLength(0), grid.GetLength(1)];

                foreach (var node in TwoDIterItem<DistanceNode>.TwoDIter(nodes))
                    node.Item = new DistanceNode(node.Loc);

                nodes[loc.X, loc.Y] = new DistanceNode(queue, loc, 0);
                foreach (var tile in TwoDIterItem<Tile>.TwoDIter(grid).Where(x => x.Item is Empty))
                    nodes[tile.Loc.X, tile.Loc.Y] = new DistanceNode(queue, tile.Loc);

                while (queue.Count > 0)
                {
                    var closest = queue.Pop();
                    if (closest.Distance == int.MaxValue) break;

                    void TestDistance(Vec2 close)
                    {
                        if (grid[close.X, close.Y] is Empty && nodes[close.X, close.Y].Distance > closest.Distance + 1)
                            nodes[close.X, close.Y].Distance = closest.Distance + 1;
                    }

                    TestDistance(closest.Loc + new Vec2(0, -1));
                    TestDistance(closest.Loc + new Vec2(-1, 0));
                    TestDistance(closest.Loc + new Vec2(1, 0));
                    TestDistance(closest.Loc + new Vec2(0, 1));
                }

                var ret = new int[grid.GetLength(0), grid.GetLength(1)];
                for (var x = 0; x < grid.GetLength(0); x++)
                for (var y = 0; y < grid.GetLength(1); y++)
                    ret[x, y] = nodes[x, y].Distance;

                return ret;
            }

            private static List<Vec2> CalcPaths(int[,] distance, Vec2 loc)
            {
                var shortestPaths = new List<Vec2>();
                var currentDistance = distance[loc.X, loc.Y];
                if (currentDistance == 1)
                {
                    shortestPaths.Add(loc);
                    return shortestPaths;
                }

                var shortestDistance = int.MaxValue;
                var shortestNodes = new List<Vec2>();

                void TestDistance(Vec2 testingLoc)
                {
                    if (distance[testingLoc.X, testingLoc.Y] > shortestDistance) return;
                    if (distance[testingLoc.X, testingLoc.Y] != shortestDistance) shortestNodes.Clear();

                    shortestDistance = distance[testingLoc.X, testingLoc.Y];
                    shortestNodes.Add(testingLoc);
                }

                TestDistance(loc + new Vec2(0, -1));
                TestDistance(loc + new Vec2(-1, 0));
                TestDistance(loc + new Vec2(1, 0));
                TestDistance(loc + new Vec2(0, 1));

                foreach (var node in shortestNodes) shortestPaths.AddRange(CalcPaths(distance, node));

                return new List<Vec2>(shortestPaths.Distinct());
            }

            protected abstract void KillEnemy(State state);

            public override bool Process(State state, int tick, Vec2 thisLoc)
            {
                base.Process(state, tick, thisLoc);
                if (Tick >= tick) return false;
                if (state.ElfCount == 0 || state.GoblinCount == 0) return true;
                Tick = tick;
                var distanceGrid = CalculateDistance(state.Grid, thisLoc);
                var closestEnemyDistance = int.MaxValue;
                Vec2? closestEnemy = null;

                IEnumerable<Vec2> Adjacent(Vec2 loc, Func<Tile, bool> isValid)
                {
                    if (isValid(state.Grid[loc.X, loc.Y - 1])) yield return new Vec2(loc.X, loc.Y - 1);

                    if (isValid(state.Grid[loc.X - 1, loc.Y])) yield return new Vec2(loc.X - 1, loc.Y);

                    if (isValid(state.Grid[loc.X + 1, loc.Y])) yield return new Vec2(loc.X + 1, loc.Y);

                    if (isValid(state.Grid[loc.X, loc.Y + 1])) yield return new Vec2(loc.X, loc.Y + 1);
                }

                var adjacentToEnemy = new List<Vec2>();

                foreach (var tile in TwoDIterItem<Tile>.TwoDIter(state.Grid))
                {
                    if (!(tile.Item is Unit unit && IsEnemy(unit))) continue;
                    adjacentToEnemy.AddRange(Adjacent(tile.Loc, til => til is Empty || til == this));
                }

                adjacentToEnemy.Sort();

                foreach (var tile in adjacentToEnemy)
                {
                    if (distanceGrid[tile.X, tile.Y] >= closestEnemyDistance) continue;
                    closestEnemy = tile;
                    closestEnemyDistance = distanceGrid[tile.X, tile.Y];
                }

                if (closestEnemy == null) return false;

                if (closestEnemyDistance > 0)
                {
                    var paths = CalcPaths(distanceGrid, closestEnemy.Value);
                    paths.Sort();

                    state.Grid[thisLoc.X, thisLoc.Y] = Empty.Instance;
                    thisLoc = paths[0];
                    state.Grid[thisLoc.X, thisLoc.Y] = this;
                }

                var lowestHp = int.MaxValue;
                Vec2? lowestHpLoc = null;
                foreach (var enemyLoc in Adjacent(thisLoc,
                    tile => tile is Unit unit && IsEnemy(unit)))
                {
                    var enemy = (Unit) state.Grid[enemyLoc.X, enemyLoc.Y];
                    if (enemy.Hp >= lowestHp) continue;
                    lowestHp = enemy.Hp;
                    lowestHpLoc = enemyLoc;
                }

                if (lowestHpLoc == null) return false;
                var target = (Unit) state.Grid[lowestHpLoc.Value.X, lowestHpLoc.Value.Y];
                target.Hp -= AttackPower;
                if (target.Hp > 0) return false;
                state.Grid[lowestHpLoc.Value.X, lowestHpLoc.Value.Y] = Empty.Instance;
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

        private class DistanceNode : IComparable<DistanceNode>
        {
            private readonly PriorityQueue<DistanceNode>.Node _node;
            private int _distance;

            public DistanceNode(Vec2 loc)
            {
                _distance = int.MaxValue;
                Loc = loc;
            }

            public DistanceNode(PriorityQueue<DistanceNode> queue, Vec2 loc, int distance = int.MaxValue)
            {
                Loc = loc;
                _distance = distance;
                _node = queue.AddGetNode(this);
            }

            public int Distance
            {
                get => _distance;
                set
                {
                    _distance = value;
                    _node?.Update();
                }
            }

            public Vec2 Loc { get; }

            public int CompareTo(DistanceNode other)
            {
                if (ReferenceEquals(this, other)) return 0;
                return other is null ? 1 : _distance.CompareTo(other._distance);
            }
        }
    }
}