using System.Collections.Generic;

namespace MFrameworkCSTests.Sort
{
    public interface ISort<T>
    {
        void Sort(IList<T> list, IComparer<T> comparer);
    }
}
