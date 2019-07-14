using System.Collections;
using System.Collections.Generic;

namespace AdventOfCode2018.Common
{
    public struct TwoDIterItem<T>
    {
        private TwoDIterItem(TwoDIterEnumerator inner)
        {
            Inner = inner;
        }

        private TwoDIterEnumerator Inner { get; }

        public ref T Item => ref Inner.Array[Inner.X, Inner.Y];
        public Vec2 Loc => new Vec2(Inner.X, Inner.Y);

        public static IEnumerable<TwoDIterItem<T>> TwoDIter(T[,] array)
        {
            return new TwoDIterEnumerable(array);
        }

        private struct TwoDIterEnumerator : IEnumerator<TwoDIterItem<T>>
        {
            public TwoDIterEnumerator(T[,] array) : this()
            {
                Array = array;
            }

            public T[,] Array { get; }
            public int X { get; private set; }
            public int Y { get; private set; }
            private bool Init { get; set; }

            public bool MoveNext()
            {
                if (!Init)
                {
                    Init = true;
                    return Array.Length != 0;
                }

                X++;
                if (X >= Array.GetLength(0))
                {
                    X = 0;
                    Y++;
                }

                if (Y < Array.GetLength(1)) return true;
                X = Array.GetLength(0) - 1;
                Y = Array.GetLength(1) - 1;
                return false;
            }

            public void Reset()
            {
                Init = false;
                X = 0;
                Y = 0;
            }

            public TwoDIterItem<T> Current => new TwoDIterItem<T>(this);

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }

        private struct TwoDIterEnumerable : IEnumerable<TwoDIterItem<T>>
        {
            public TwoDIterEnumerable(T[,] array)
            {
                Array = array;
            }

            private T[,] Array { get; }

            public IEnumerator<TwoDIterItem<T>> GetEnumerator()
            {
                return new TwoDIterEnumerator(Array);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}