using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day3 : ISolution
    {
        private const int WidthOfFabric = 1000;
        private const int HeightOfFabric = 1000;

        int ISolution.DayN => 3;

        string ISolution.Part1(string[] input)
        {
            return GetAns(input).Item1.ToString();
        }

        string ISolution.Part2(string[] input)
        {
            return GetAns(input).Item2.ToString();
        }

        private static IEnumerable<Claim> ParseInput(IEnumerable<string> input)
        {
            return input.Select(str => new Claim(new ClaimDescription(str)));
        }

        private static (int, int) GetAns(string[] input)
        {
            var fabric = new Square[WidthOfFabric, HeightOfFabric];
            var overlapping = 0;
            var claims = ParseInput(input).ToArray();

            foreach (var claim in claims)
                for (var h = claim.Description.LeftDistance;
                    h < claim.Description.LeftDistance + claim.Description.Width;
                    h++)
                for (var v = claim.Description.TopDistance;
                    v < claim.Description.TopDistance + claim.Description.Height;
                    v++)
                    if (fabric[h, v].InnerClaim == null)
                    {
                        fabric[h, v].InnerClaim = claim;
                    }
                    else
                    {
                        if (!fabric[h, v].Overlaps)
                        {
                            overlapping++;
                            fabric[h, v].Overlaps = true;
                        }

                        fabric[h, v].InnerClaim.Overlaps = true;
                        claim.Overlaps = true;
                    }

            var firstOverlap = claims.First(claim => !claim.Overlaps).Description.Id;

            return (overlapping, firstOverlap);
        }

        private struct ClaimDescription
        {
            public ClaimDescription(string input)
            {
                var match = ParsingRegex.Match(input);
                Id = int.Parse(match.Groups["id"].Captures[0].Value);
                LeftDistance = int.Parse(match.Groups["leftDistance"].Captures[0].Value);
                TopDistance = int.Parse(match.Groups["topDistance"].Captures[0].Value);
                Width = int.Parse(match.Groups["width"].Captures[0].Value);
                Height = int.Parse(match.Groups["height"].Captures[0].Value);
            }

            private static Regex ParsingRegex { get; } =
                new Regex(@"#(?<id>\d+) @ (?<leftDistance>\d+),(?<topDistance>\d+): (?<width>\d+)x(?<height>\d+)");

            public int Id { get; }
            public int LeftDistance { get; }
            public int TopDistance { get; }
            public int Width { get; }
            public int Height { get; }
        }

        private class Claim
        {
            public Claim(ClaimDescription description)
            {
                Description = description;
            }

            public ClaimDescription Description { get; }
            public bool Overlaps { get; set; }
        }

        private struct Square
        {
            public Claim InnerClaim { get; set; }
            public bool Overlaps { get; set; }
        }
    }
}