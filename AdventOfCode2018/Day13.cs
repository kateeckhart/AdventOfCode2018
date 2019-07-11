using System;
using System.Collections.Generic;
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

            var carts = new List<Cart>();
            for (var i = 0; i < input.Length; i++)
            for (var j = 0; j < input[i].Length; j++)
            {
                TwoWay twoWay = null;
                Cart cart;
                if (grid[i, j] is TwoWay) twoWay = (TwoWay) grid[i, j];
                switch (input[i][j])
                {
                    case '<':
                        cart = new Cart(RealDirection.Left, twoWay);
                        twoWay.CartOnTrack = cart;
                        carts.Add(cart);
                        goto leftRight;
                    case '>':
                        cart = new Cart(RealDirection.Right, twoWay);
                        twoWay.CartOnTrack = cart;
                        carts.Add(cart);
                        goto leftRight;
                    case '-':
                        leftRight:
                        twoWay.Paths[0] = new TwoWay.Path(grid[i, j - 1], RealDirection.Left, RealDirection.Left);
                        twoWay.Paths[1] = new TwoWay.Path(grid[i, j + 1], RealDirection.Right, RealDirection.Right);
                        break;
                    case '^':
                        cart = new Cart(RealDirection.Up, twoWay);
                        twoWay.CartOnTrack = cart;
                        carts.Add(cart);
                        goto upDown;
                    case 'v':
                        cart = new Cart(RealDirection.Down, twoWay);
                        twoWay.CartOnTrack = cart;
                        carts.Add(cart);
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

            Vec2? locOfFirstCrash = null;

            while (carts.Count > 1)
            {
                carts.Sort();
                for (var i = carts.Count - 1; i >= 0; i--)
                    if (carts[i].OnTrack == null)
                        carts.RemoveAt(i);
                    else
                        break;
                if (carts.Count == 1) break;
                foreach (var cart in carts)
                {
                    var locOfCrash = cart.OnTrack?.Process();
                    if (locOfCrash == null) continue;
                    if (locOfFirstCrash == null) locOfFirstCrash = locOfCrash;
                }
            }

            return ($"{locOfFirstCrash.Value.X},{locOfFirstCrash.Value.Y}",
                $"{carts[0].OnTrack.Loc.X},{carts[0].OnTrack.Loc.Y}");
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

        private class Cart : IComparable<Cart>
        {
            public Cart(RealDirection direction, Track onTrack)
            {
                Direction = direction;
                OnTrack = onTrack;
            }

            public RealDirection Direction { get; set; }
            public IntersectionDirection NextIntersection { get; set; }
            public Track OnTrack { get; set; }

            public int CompareTo(Cart other)
            {
                if (OnTrack == null) return other.OnTrack == null ? 0 : 1;

                if (other.OnTrack == null) return -1;

                var xCompare = OnTrack.Loc.X.CompareTo(other.OnTrack.Loc.X);
                return xCompare != 0 ? xCompare : OnTrack.Loc.Y.CompareTo(other.OnTrack.Loc.Y);
            }
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

            public Cart CartOnTrack { get; set; }

            public Vec2 Loc { get; }

            public abstract Vec2? Process();

            public Vec2? SetCartOnTrack(Cart cart)
            {
                if (CartOnTrack != null)
                {
                    CartOnTrack.OnTrack = null;
                    CartOnTrack = null;
                    cart.OnTrack.CartOnTrack = null;
                    cart.OnTrack = null;
                    return Loc;
                }

                CartOnTrack = cart;
                cart.OnTrack.CartOnTrack = null;
                cart.OnTrack = this;
                return null;
            }
        }

        private class TwoWay : Track
        {
            public TwoWay(Vec2 loc) : base(loc)
            {
            }

            public Path[] Paths { get; } = new Path[2];

            public override Vec2? Process()
            {
                if (CartOnTrack == null) return null;
                var path = Paths.First(x => x.From == CartOnTrack.Direction);
                CartOnTrack.Direction = path.To;

                return path.PathTrack.SetCartOnTrack(CartOnTrack);
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

            public override Vec2? Process()
            {
                if (CartOnTrack == null) return null;
                var newDirection = CartOnTrack.Direction;

                switch (CartOnTrack.NextIntersection)
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

                CartOnTrack.Direction = newDirection;
                CartOnTrack.NextIntersection = NextIntersection(CartOnTrack.NextIntersection);

                return Tracks[(int) newDirection].SetCartOnTrack(CartOnTrack);
            }
        }
    }
}