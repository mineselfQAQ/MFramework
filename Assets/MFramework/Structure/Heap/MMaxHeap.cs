using System.Collections.Generic;

namespace MFramework
{
    public class MMaxHeap<T>
    {
        private MList<T> _items;

        private IComparer<T> comparer;

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public MMaxHeap()
        {
            _items = new MList<T>();

            this.comparer = Comparer<T>.Default;
        }
        public MMaxHeap(IComparer<T> comparer)
        {
            _items = new MList<T>();

            this.comparer = comparer;
        }
        public MMaxHeap(int capacity, IComparer<T> comparer)
        {
            _items = new MList<T>(capacity);

            this.comparer = comparer;
        }

        public T Peek()
        {
            return _items[0];
        }

        public void Push(T item)
        {
            _items.Add(item);
            SiftUp(Count - 1);
        }

        private void SiftUp(int i)
        {
            while (true)
            {
                int p = GetParent(i);
                if (p < 0 || comparer.Compare(_items[i], _items[p]) <= 0)
                {
                    break;
                }

                Swap(i, p);
                i = p;
            }
        }

        private void Swap(int a, int b)
        {
            T temp = _items[a];
            _items[a] = _items[b];
            _items[b] = temp;
        }

        private int GetLeft(int i)
        {
            return 2 * i + 1;
        }
        private int GetRight(int i)
        {
            return 2 * i + 2;
        }
        private int GetParent(int i)
        {
            return (i - 1) / 2;
        }
    }
}