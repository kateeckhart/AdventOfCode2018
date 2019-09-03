using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018.Common
{
    public class PriorityQueue<T> : ICollection<T>
    {
        private readonly IComparer<T> _comparer;

        private readonly List<Node> _inner;

        public PriorityQueue()
        {
            _inner = new List<Node>();
            _comparer = Comparer<T>.Default;
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            _inner = new List<Node>();
            _comparer = comparer;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.Select(x => x.Item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            AddGetNode(item);
        }

        public void Clear()
        {
            foreach (var node in _inner) node.Queue = null;
            _inner.Clear();
        }

        public bool Contains(T item)
        {
            return _inner.Any(node => _comparer.Compare(node.Item, item) == 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array too small.", nameof(array));

            for (var i = 0; i < Count; i++) array[arrayIndex + i] = _inner[i].Item;
        }

        public bool Remove(T item)
        {
            var itemIndex = _inner.FindIndex(x => _comparer.Compare(x.Item, item) == 0);

            if (itemIndex == -1) return false;

            _inner[itemIndex].Remove();
            return true;
        }

        public int Count => _inner.Count;

        public bool IsReadOnly => false;

        public Node AddGetNode(T item)
        {
            var newNode = new Node(this, Count, item);
            _inner.Add(newNode);
            newNode.ShiftUp();

            return newNode;
        }

        public T Pop()
        {
            switch (Count)
            {
                case 0:
                    throw new InvalidOperationException();
                case 1:
                {
                    var ret = _inner[0].Item;
                    _inner.RemoveAt(0);
                    return ret;
                }
            }

            var greatest = _inner[0];
            var replacement = _inner[_inner.Count - 1];
            greatest.Swap(replacement);
            _inner.RemoveAt(_inner.Count - 1);
            greatest.Queue = null;
            replacement.ShiftDown();
            return greatest.Item;
        }

        public class Node
        {
            private int _index;

            private T _item;

            internal PriorityQueue<T> Queue;

            internal Node(PriorityQueue<T> queue, int index, T item)
            {
                Queue = queue;
                _index = index;
                _item = item;
            }

            public T Item
            {
                get => _item;
                set
                {
                    _item = value;
                    Update();
                }
            }

            internal void Swap(Node other)
            {
                Queue._inner[_index] = other;
                Queue._inner[other._index] = this;

                var swapIndex = _index;
                _index = other._index;
                other._index = swapIndex;
            }

            public void Remove()
            {
                while (_index != 0) Swap(Queue._inner[(_index - 1) / 2]);

                Queue.Pop();
            }

            internal void ShiftUp()
            {
                while (true)
                {
                    if (_index == 0) return;
                    var parentNode = Queue._inner[(_index - 1) / 2];
                    if (Queue._comparer.Compare(_item, parentNode._item) >= 0) return;

                    Swap(parentNode);
                }
            }

            internal void ShiftDown()
            {
                while (true)
                {
                    var child1Index = _index * 2 + 1;
                    var child2Index = child1Index + 1;

                    if (child1Index >= Queue.Count) return;
                    var child1 = Queue._inner[child1Index];
                    var smallerChild = child1;
                    if (child2Index < Queue.Count)
                    {
                        var child2 = Queue._inner[child2Index];
                        if (Queue._comparer.Compare(child2._item, child1._item) < 0) smallerChild = child2;
                    }

                    if (Queue._comparer.Compare(_item, smallerChild.Item) >= 0) Swap(smallerChild);
                    else return;
                }
            }

            public void Update()
            {
                ShiftUp();
                ShiftDown();
            }
        }
    }
}