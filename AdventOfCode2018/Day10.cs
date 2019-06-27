using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day10 : ISolution
    {
        private static Regex ParsingRegex { get; } =
            new Regex(@"position=< *(?<PosX>-?\d+), *(?<PosY>-?\d+)> velocity=< *(?<VelX>-?\d+), *(?<VelY>-?\d+)>");

        public int DayN => 10;

        public (string, string) GetAns(string[] input)
        {
            var stars = new Star[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                var match = ParsingRegex.Match(input[i]);
                var pos = new Vec2(int.Parse(match.Groups["PosX"].Value), int.Parse(match.Groups["PosY"].Value));
                var vel = new Vec2(int.Parse(match.Groups["VelX"].Value), int.Parse(match.Groups["VelY"].Value));
                stars[i].Pos = pos;
                stars[i].Vel = vel;
            }

            long smallestArea;
            var currentArea = long.MaxValue;
            var wait = 0;
            do
            {
                smallestArea = currentArea;
                for (var i = 0; i < stars.Length; i++) stars[i].Pos += stars[i].Vel;

                var currentBound = Star.GetStarBounds(stars);
                currentArea = (long) currentBound.X * currentBound.Y;
                wait++;
            } while (currentArea < smallestArea);

            for (var i = 0; i < stars.Length; i++) stars[i].Pos -= stars[i].Vel;

            wait--;

            var smallestBound = Star.GetStarBounds(stars);
            var sky = new char[smallestBound.X + 1, smallestBound.Y + 1];
            for (var x = 0; x <= smallestBound.X; x++)
            for (var y = 0; y <= smallestBound.Y; y++)
                sky[x, y] = '.';
            var lowest = Star.GetLowest(stars);
            foreach (var star in stars) sky[star.Pos.X - lowest.X, star.Pos.Y - lowest.Y] = '#';

            var outSky = new StringBuilder();
            for (var y = 0; y <= smallestBound.Y; y++)
            {
                outSky.AppendLine();

                for (var x = 0; x <= smallestBound.X; x++) outSky.Append(sky[x, y]);
            }

            return (outSky.ToString(), wait.ToString());
        }

        private struct Star
        {
            public Vec2 Pos { get; set; }
            public Vec2 Vel { get; set; }

            public static Vec2 GetLowest(IEnumerable<Star> stars)
            {
                var lowestX = int.MaxValue;
                var lowestY = int.MaxValue;

                foreach (var star in stars)
                {
                    var pos = star.Pos;

                    if (pos.X < lowestX) lowestX = pos.X;

                    if (pos.Y < lowestY) lowestY = pos.Y;
                }

                return new Vec2(lowestX, lowestY);
            }

            public static Vec2 GetStarBounds(Star[] stars)
            {
                var highestX = int.MinValue;
                var highestY = int.MinValue;
                var lowest = GetLowest(stars);

                foreach (var star in stars)
                {
                    var pos = star.Pos;

                    if (pos.X > highestX) highestX = pos.X;

                    if (pos.Y > highestY) highestY = pos.Y;
                }

                return new Vec2(highestX - lowest.X, highestY - lowest.Y);
            }
        }

        private struct Vec2
        {
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

            public int X { get; }
            public int Y { get; }
        }
    }
}