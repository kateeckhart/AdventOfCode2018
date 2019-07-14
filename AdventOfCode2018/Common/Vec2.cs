using System;

namespace AdventOfCode2018.Common
{
    public struct Vec2 : IComparable<Vec2>
    {
        public int CompareTo(Vec2 other)
        {
            var yComparison = Y.CompareTo(other.Y);
            return yComparison != 0 ? yComparison : X.CompareTo(other.X);
        }

        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }

        public int DistanceTo(Vec2 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public int X { get; }
        public int Y { get; }
    }
}