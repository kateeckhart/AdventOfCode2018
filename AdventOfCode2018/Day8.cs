using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
    public class Day8 : ISolution
    {
        public int DayN => 8;

        public (string, string) GetAns(string[] input)
        {
            var tree = Tree.ParseTree(StringToIntEnumerable(input[0]));

            return (tree.MetaSum.ToString(), tree.Value.ToString());
        }

        private static IEnumerable<int> StringToIntEnumerable(string input)
        {
            var spacedString = input.Split(new[] {' '});
            return spacedString.Select(int.Parse);
        }

        private struct Tree
        {
            private List<int> ChildValues { get; set; }
            public int Value { get; private set; }
            public int MetaSum { get; private set; }

            public static Tree ParseTree(IEnumerable<int> input)
            {
                using (var enumerator = input.GetEnumerator())
                {
                    return ParseTreeInternal(enumerator);
                }
            }

            private static Tree ParseTreeInternal(IEnumerator<int> input)
            {
                if (!input.MoveNext()) throw new ArgumentException();
                var childCount = input.Current;
                if (!input.MoveNext()) throw new ArgumentException();
                var metaDataCount = input.Current;
                var ret = new Tree {ChildValues = new List<int>(childCount)};

                for (var i = 0; i < childCount; i++)
                {
                    var child = ParseTreeInternal(input);
                    ret.MetaSum += child.MetaSum;
                    ret.ChildValues.Add(child.Value);
                }

                for (var i = 0; i < metaDataCount; i++)
                {
                    if (!input.MoveNext()) throw new ArgumentException();
                    var num = input.Current;
                    ret.MetaSum += num;
                    if (childCount == 0)
                    {
                        ret.Value += num;
                    }
                    else
                    {
                        if (num == 0 || num > childCount) continue;
                        ret.Value += ret.ChildValues[num - 1];
                    }
                }

                return ret;
            }
        }
    }
}