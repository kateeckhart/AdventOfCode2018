using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day12 : ISolution
    {
        private static Regex InitialStateRegex { get; } = new Regex(@"initial state: (?<state>(?:#|\.)*)");
        private static Regex TransformRuleRegex { get; } = new Regex(@"(?<from>(?:#|\.){5}) => (?<to>#|\.)");
        public int DayN => 12;

        public (string, string) GetAns(string[] input)
        {
            var plots = new Plots();
            const int numberOfTransformRules = 1 << 6;
            var transformer = new bool[numberOfTransformRules];

            var initialStateMatch = InitialStateRegex.Match(input[0]).Groups["state"].Value;
            for (var i = 0; i < initialStateMatch.Length; i++) plots[i] = initialStateMatch[i] == '#';

            foreach (var line in input.Skip(2))
            {
                var transformMatch = TransformRuleRegex.Match(line);
                var rule = 0;
                foreach (var plot in transformMatch.Groups["from"].Value)
                {
                    rule <<= 1;
                    var bit = 0;
                    if (plot == '#') bit = 1;
                    rule |= bit;
                }

                transformer[rule] = transformMatch.Groups["to"].Value == "#";
            }

            var sum = 0;
            int? part1OrNull = null;
            var x = 0;
            int lastDiff;
            var diff = 0;

            do
            {
                var lastSum = sum;
                lastDiff = diff;
                var newPlots = new Plots(plots);
                for (var i = plots.LowerBound - 2; i < plots.UpperBound + 2; i++)
                {
                    var rule = 0;
                    for (var j = i - 2; j <= i + 2; j++)
                    {
                        rule <<= 1;
                        var bit = 0;
                        if (plots[j]) bit = 1;
                        rule |= bit;
                    }

                    newPlots[i] = transformer[rule];
                }

                plots = newPlots;

                sum = 0;
                for (var i = plots.LowerBound; i < plots.UpperBound; i++)
                    if (plots[i])
                        sum += i;

                x++;
                if (x == 20) part1OrNull = sum;

                diff = sum - lastSum;
            } while (diff != lastDiff);

            var part1 = part1OrNull ?? diff * (20 - x) + sum;

            return (part1.ToString(), ((50000000000 - x) * diff + sum).ToString());
        }

        private class Plots
        {
            public Plots()
            {
                Positive = new List<bool>();
                Negative = new List<bool>();
            }

            public Plots(Plots plots)
            {
                Positive = new List<bool>(plots.Positive);
                Negative = new List<bool>(plots.Negative);
            }

            private List<bool> Positive { get; }
            private List<bool> Negative { get; }

            public int LowerBound => -Negative.Count;
            public int UpperBound => Positive.Count;

            public bool this[int i]
            {
                get
                {
                    var negI = -(i + 1);
                    if (i >= 0 && i < Positive.Count) return Positive[i];

                    return negI >= 0 && negI < Negative.Count && Negative[negI];
                }
                set
                {
                    var negI = -(i + 1);
                    if (i >= 0)
                        SetList(Positive, i, value);
                    else
                        SetList(Negative, negI, value);
                }
            }

            private static void ExpandList<T>(IList<T> list, int newSize) where T : new()
            {
                while (list.Count < newSize) list.Add(new T());
            }

            private static void SetList(IList<bool> list, int index, bool value)
            {
                if (index >= list.Count && !value) return;

                ExpandList(list, index + 1);
                list[index] = value;
            }
        }
    }
}