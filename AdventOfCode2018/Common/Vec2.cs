using System;

namespace AdventOfCode2018.Common
{
    public struct Vec2 : IComparable<Vec2>, IComparable, IEquatable<Vec2>
    {
        public bool Equals(Vec2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vec2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (X, Y).GetHashCode();
        }

        public static bool operator ==(Vec2 left, Vec2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec2 left, Vec2 right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is Vec2 other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(Vec2)}");
        }

        public static bool operator <(Vec2 left, Vec2 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Vec2 left, Vec2 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Vec2 left, Vec2 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Vec2 left, Vec2 right)
        {
            return left.CompareTo(right) >= 0;
        }

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

        public int X { get; set; }
        public int Y { get; set; }
    }
}