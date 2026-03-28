using System.Collections.Generic;

namespace MFrameworkCSTests.Sort
{
    public static class SortUtils
    {
        public static void Swap<T>(IList<T> list, int i, int j)
        {
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
