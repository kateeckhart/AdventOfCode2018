using System.Linq;

namespace AdventOfCode2018
{
    public class Day13 : ISolution
    {
        public int DayN => 13;

        public (string, string) GetAns(string[] input)
        {
            var grid = new Track[input.Length, input[0].Length];

            for (var i = 0; i < input.Length; i++)
            for (var j = 0; j < input[i].Length; j++)
                switch (input[i][j])
                {
                    case '+':
                        grid[i, j] = new Intersection(new Vec2(j, i));
                        break;
                    case ' ':
                        break;
                    default:
                        grid[i, j] = new TwoWay(new Vec2(j, i));
                        break;
                }

            var carts = 0;
            for (var i = 0; i < input.Length; i++)
            for (var j = 0; j < input[i].Length; j++)
            {
                TwoWay twoWay = null;
                Cart cart;
                if (grid[i, j] is TwoWay) twoWay = (TwoWay) grid[i, j];
                switch (input[i][j])
                {
                    case '<':
                        cart = new Cart(RealDirection.Left);
                        twoWay.CartOnTrack = cart;
                        carts++;
                        goto leftRight;
                    case '>':
                        cart = new Cart(RealDirection.Right);
                        twoWay.CartOnTrack = cart;
                        carts++;
                        goto leftRight;
                    case '-':
                        leftRight:
                        twoWay.Paths[0] = new TwoWay.Path(grid[i, j - 1], RealDirection.Left, RealDirection.Left);
                        twoWay.Paths[1] = new TwoWay.Path(grid[i, j + 1], RealDirection.Right, RealDirection.Right);
                        break;
                    case '^':
                        cart = new Cart(RealDirection.Up);
                        twoWay.CartOnTrack = cart;
                        carts++;
                        goto upDown;
                    case 'v':
                        cart = new Cart(RealDirection.Down);
                        twoWay.CartOnTrack = cart;
                        carts++;
                        goto upDown;
                    case '|':
                        upDown:
                        twoWay.Paths[0] = new TwoWay.Path(grid[i + 1, j], RealDirection.Down, RealDirection.Down);
                        twoWay.Paths[1] = new TwoWay.Path(grid[i - 1, j], RealDirection.Up, RealDirection.Up);
                        break;
                    case '/':
                        if (i < input.Length - 2 && input[i + 1][j] != ' ' && input[i + 1][j] != '-' &&
                            input[i + 1][j] != '\\' && input[i + 1][j] != '/')
                        {
                            twoWay.Paths[0] = new TwoWay.Path(grid[i, j + 1], RealDirection.Up, RealDirection.Right);
                            twoWay.Paths[1] = new TwoWay.Path(grid[i + 1, j], RealDirection.Left, RealDirection.Down);
                        }
                        else
                        {
                            twoWay.Paths[0] = new TwoWay.Path(grid[i - 1, j], RealDirection.Right, RealDirection.Up);
                            twoWay.Paths[1] = new TwoWay.Path(grid[i, j - 1], RealDirection.Down, RealDirection.Left);
                        }

                        break;
                    case '\\':
                        if (i < input.Length - 2 && input[i + 1][j] != ' ' && input[i + 1][j] != '-' &&
                            input[i + 1][j] != '\\' && input[i + 1][j] != '/')
                        {
                            twoWay.Paths[0] = new TwoWay.Path(grid[i + 1, j], RealDirection.Right, RealDirection.Down);
                            twoWay.Paths[1] = new TwoWay.Path(grid[i, j - 1], RealDirection.Up, RealDirection.Left);
                        }
                        else
                        {
                            twoWay.Paths[0] = new TwoWay.Path(grid[i - 1, j], RealDirection.Left, RealDirection.Up);
                            twoWay.Paths[1] = new TwoWay.Path(grid[i, j + 1], RealDirection.Down, RealDirection.Right);
                        }

                        break;
                    case '+':
                        var intersection = (Intersection) grid[i, j];
                        intersection.Tracks[(int) RealDirection.Down] = grid[i + 1, j];
                        intersection.Tracks[(int) RealDirection.Up] = grid[i - 1, j];
                        intersection.Tracks[(int) RealDirection.Left] = grid[i, j - 1];
                        intersection.Tracks[(int) RealDirection.Right] = grid[i, j + 1];
                        break;
                }
            }

            Track firstTrack = null;

            for (var i = input.Length - 1; i >= 0; i--)
            for (var j = input[0].Length - 1; j >= 0; j--)
            {
                if (grid[i, j] == null) continue;
                grid[i, j].NextToProcess = firstTrack;
                firstTrack = grid[i, j];
            }

            Vec2? locOfFirstCrash = null;

            var tick = 1;
            while (carts > 1)
            {
                var track = firstTrack;
                while (track != null)
                {
                    var locOfCrash = track.Process(tick);
                    if (locOfCrash != null)
                    {
                        carts -= 2;
                        if (locOfFirstCrash == null) locOfFirstCrash = locOfCrash;
                    }

                    track = track.NextToProcess;
                }

                tick++;
            }

            var lastCartTrack = firstTrack;
            while (lastCartTrack != null)
            {
                if (lastCartTrack.CartOnTrack != null) break;
                if (lastCartTrack.NextToProcess == null) break;
                lastCartTrack = lastCartTrack.NextToProcess;
            }

            return ($"{locOfFirstCrash.Value.X},{locOfFirstCrash.Value.Y}",
                $"{lastCartTrack.Loc.X},{lastCartTrack.Loc.Y}");
        }

        private static IntersectionDirection NextIntersection(IntersectionDirection dir)
        {
            return dir == IntersectionDirection.Right ? IntersectionDirection.Left : dir + 1;
        }

        private enum IntersectionDirection
        {
            Left = 0,
            Forward,
            Right
        }

        private enum RealDirection
        {
            Up = 0,
            Down,
            Left,
            Right
        }

        private struct Cart
        {
            public Cart(RealDirection direction) : this()
            {
                Direction = direction;
            }

            public Cart(RealDirection direction, IntersectionDirection nextIntersection, int tick) : this(direction)
            {
                NextIntersection = nextIntersection;
                Tick = tick;
            }

            public RealDirection Direction { get; }
            public IntersectionDirection NextIntersection { get; }

            public int Tick { get; }
        }

        private struct Vec2
        {
            public Vec2(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        private abstract class Track
        {
            protected Track(Vec2 loc)
            {
                Loc = loc;
            }

            public Track NextToProcess { get; set; }
            public Cart? CartOnTrack { get; set; }

            public Vec2 Loc { get; }

            public abstract Vec2? Process(int tick);

            public Vec2? SetCartOnTrack(Cart cart)
            {
                if (CartOnTrack != null)
                {
                    CartOnTrack = null;
                    return Loc;
                }

                CartOnTrack = cart;
                return null;
            }
        }

        private class TwoWay : Track
        {
            public TwoWay(Vec2 loc) : base(loc)
            {
            }

            public Path[] Paths { get; } = new Path[2];

            public override Vec2? Process(int tick)
            {
                if (CartOnTrack == null || CartOnTrack.Value.Tick >= tick) return null;
                var path = Paths.First(x => x.From == CartOnTrack.Value.Direction);
                var newCart = new Cart(path.To, CartOnTrack.Value.NextIntersection, tick);
                CartOnTrack = null;

                return path.PathTrack.SetCartOnTrack(newCart);
            }

            public struct Path
            {
                public Path(Track pathTrack, RealDirection from, RealDirection to)
                {
                    PathTrack = pathTrack;
                    From = from;
                    To = to;
                }

                public Track PathTrack { get; }
                public RealDirection From { get; }
                public RealDirection To { get; }
            }
        }

        private class Intersection : Track
        {
            public Intersection(Vec2 loc) : base(loc)
            {
            }

            public Track[] Tracks { get; } = new Track[4];

            public override Vec2? Process(int tick)
            {
                if (CartOnTrack == null || CartOnTrack.Value.Tick >= tick) return null;

                var newDirection = CartOnTrack.Value.Direction;

                switch (CartOnTrack.Value.NextIntersection)
                {
                    case IntersectionDirection.Left:
                        switch (newDirection)
                        {
                            case RealDirection.Up:
                                newDirection = RealDirection.Left;
                                break;
                            case RealDirection.Down:
                                newDirection = RealDirection.Right;
                                break;
                            case RealDirection.Left:
                                newDirection = RealDirection.Down;
                                break;
                            case RealDirection.Right:
                                newDirection = RealDirection.Up;
                                break;
                        }

                        break;
                    case IntersectionDirection.Forward:
                        break;
                    case IntersectionDirection.Right:
                        switch (newDirection)
                        {
                            case RealDirection.Up:
                                newDirection = RealDirection.Right;
                                break;
                            case RealDirection.Down:
                                newDirection = RealDirection.Left;
                                break;
                            case RealDirection.Left:
                                newDirection = RealDirection.Up;
                                break;
                            case RealDirection.Right:
                                newDirection = RealDirection.Down;
                                break;
                        }

                        break;
                }

                var newCart = new Cart(newDirection, NextIntersection(CartOnTrack.Value.NextIntersection), tick);
                CartOnTrack = null;

                return Tracks[(int) newDirection].SetCartOnTrack(newCart);
            }
        }
    }
}