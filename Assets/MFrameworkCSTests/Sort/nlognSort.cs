using System.Collections.Generic;

namespace MFrameworkCSTests.Sort
{
    public class ShellSort<T> : ISort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            // 优化：Knuth序列
            int gap = 1;
            while (gap < list.Count / 3)
            {
                gap = gap * 3 + 1;
            }
            while (gap >= 1)
            {
                for (int i = 0; i < gap; i++)
                {
                    for (int j = i + gap; j < list.Count; j += gap)
                    {
                        T current = list[j];
                        int k = j - gap;

                        while (k >= i && comparer.Compare(current, list[k]) < 0)
                        {
                            list[k + gap] = list[k];
                            k -= gap;
                        }
                        list[k + gap] = current;
                    }
                }

                gap = (gap - 1) / 3;
            }
        }
    }

    public class QuickSort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null, int way = 0)
        {
            switch (way)
            {
                case 0:
                    SortHoareLeftPivot(list, comparer);
                    break;
                case 1:
                    SortHoare(list, comparer);
                    break;
                case 2:
                    SortLomuto(list, comparer);
                    break;
                case 3:
                    SortHole(list, comparer);
                    break;
                case 4:
                    SortThreeWay(list, comparer);
                    break;
            }
        }

        #region 1) HoareLeftPivot

        private void SortHoareLeftPivot(IList<T> list, IComparer<T> comparer)
        {
            if (list.Count < 2) return;
            comparer ??= Comparer<T>.Default;

            SortHoareLeftPivot(list, comparer, 0, list.Count - 1);
        }

        private void SortHoareLeftPivot(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 通用边界考虑：
            // left < right，即至少还有2个元素，需要排序，当left == right时，只有1个元素了，无需排序
            while (left < right)
            {
                int pivot = PartitionHoareLeftPivot(list, comparer, left, right);

                // 优化：只递归较短的一边，控制栈深
                // 此时pivot为标准语义即分割点
                if (pivot - left < right - pivot)
                {
                    SortHoareLeftPivot(list, comparer, left, pivot - 1);
                    left = pivot + 1;
                }
                else
                {
                    SortHoareLeftPivot(list, comparer, pivot + 1, right);
                    right = pivot - 1;
                }
            }
        }

        private int PartitionHoareLeftPivot(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 将中间数放到左侧后执行
            int median = MedianThree(list, comparer, left, left + ((right - left) / 2), right);
            SortUtils.Swap(list, left, median);

            // 边界考虑：
            // 从端点开始考虑，不断交换不符合元素，当l==r时，可以认为是pivot落点
            // 比较时附带=，是因为r--/l++都是发生在该元素无需交换的情况，同时这也意味着与pivot相等的数随意摆放（pivot具有唯一性）
            // pivot会跳过（带=自然会跳过）
            T pivot = list[left];
            int l = left;
            int r = right;

            while (l < r)
            {
                // 重点：先右再左，因为起点（pivot）设置在左
                // 极端例子：
                // 顺序情况先动左，l/r重合在right处，left与l（实际是right）交换导致错误
                // 顺序情况先动右，l/r重合在left处，left与l（实际还是left）交换等于没换
                while (l < r && comparer.Compare(list[r], pivot) >= 0)
                {
                    r--;
                }

                while (l < r && comparer.Compare(list[l], pivot) <= 0)
                {
                    l++;
                }

                if (l < r)
                {
                    SortUtils.Swap(list, l, r);
                }
            }

            // 使用l或r交换都是可以的，可能l语义上更顺一点
            SortUtils.Swap(list, left, l);
            return l;
        }

        #endregion

        #region 2) Hoare

        public void SortHoare(IList<T> list, IComparer<T> comparer = null)
        {
            if (list.Count < 2) return;
            comparer ??= Comparer<T>.Default;

            SortHoare(list, comparer, 0, list.Count - 1);
        }

        private void SortHoare(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 通用边界考虑：
            // left < right，即至少还有2个元素，需要排序，当left == right时，只有1个元素了，无需排序
            while (left < right)
            {
                // 注意用词：mid（并非真正意义上的pivot）
                int mid = PartitionHoare(list, comparer, left, right);

                // 优化：只递归较短的一边，控制栈深
                // 重点：此时的mid语义：返回左侧最后一个元素（根据返回的是i还是j决定）
                if (mid - left < right - mid)
                {
                    SortHoare(list, comparer, left, mid);
                    left = mid + 1;
                }
                else
                {
                    SortHoare(list, comparer, mid + 1, right);
                    right = mid;
                }
            }
        }

        private int PartitionHoare(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            int median = MedianThree(list, comparer, left, left + ((right - left) / 2), right);
            T pivot = list[median];

            // 使用dowhile需要提前向外一格
            // 边界考虑：
            // 从端点开始考虑，相等或不符合都交换
            int i = left - 1;
            int j = right + 1;

            while (true)
            {
                // 重点：相比LeftPivot，=也会交换
                // 空间复杂度不容易退化（考虑只有一种值，Hoare分割点在中间，HoareLeftPivot在最左侧）
                // 重点2：dowhile具有自动推进功能，while也是也可以的
                // 重点3：[i,j]可以理解为未处理区间，当i>=j时说明已全部完成处理
                do
                {
                    i++;
                }
                while (comparer.Compare(list[i], pivot) < 0);

                do
                {
                    j--;
                }
                while (comparer.Compare(list[j], pivot) > 0);

                if (i >= j)
                {
                    return j; // 重点4：返回j对应SortHoare递归选择
                }

                SortUtils.Swap(list, i, j);
            }
        }
        #endregion

        #region 3) Lomuto

        public void SortLomuto(IList<T> list, IComparer<T> comparer = null)
        {
            if (list.Count < 2) return;
            comparer ??= Comparer<T>.Default;

            SortLomuto(list, comparer, 0, list.Count - 1);
        }

        private void SortLomuto(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 通用边界考虑：
            // left < right，即至少还有2个元素，需要排序，当left == right时，只有1个元素了，无需排序
            while (left < right)
            {
                int pivot = PartitionLomuto(list, comparer, left, right);

                // 优化：只递归较短的一边，控制栈深
                // 此时pivot为标准语义即分割点
                if (pivot - left < right - pivot)
                {
                    SortLomuto(list, comparer, left, pivot - 1);
                    left = pivot + 1;
                }
                else
                {
                    SortLomuto(list, comparer, pivot + 1, right);
                    right = pivot - 1;
                }
            }
        }

        private int PartitionLomuto(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 将中间数放到右侧后执行
            int median = MedianThree(list, comparer, left, left + ((right - left) / 2), right);
            SortUtils.Swap(list, median, right);

            // i：未排序头（i前面的都是小于pivot的值）
            // j：搜寻者，只要小于pivot就扔到i处
            // 所以搜寻完成后，i就会是pivot应该在的位置
            T pivot = list[right];
            int i = left;

            for (int j = left; j < right; j++)
            {
                // 左：<pivot    右：>=pivot
                if (comparer.Compare(list[j], pivot) < 0)
                {
                    if (i != j)
                    {
                        SortUtils.Swap(list, i, j);
                    }
                    i++;
                }
            }

            if (i != right)
            {
                SortUtils.Swap(list, i, right);
            }

            return i;
        }

        #endregion

        #region 4) Hole（挖坑法）

        public void SortHole(IList<T> list, IComparer<T> comparer = null)
        {
            if (list.Count < 2) return;
            comparer ??= Comparer<T>.Default;

            SortHole(list, comparer, 0, list.Count - 1);
        }

        private void SortHole(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 通用边界考虑：
            // left < right，即至少还有2个元素，需要排序，当left == right时，只有1个元素了，无需排序
            while (left < right)
            {
                int pivot = PartitionHole(list, comparer, left, right);

                // 优化：只递归较短的一边，控制栈深
                // 此时pivot为标准语义即分割点
                if (pivot - left < right - pivot)
                {
                    SortHole(list, comparer, left, pivot - 1);
                    left = pivot + 1;
                }
                else
                {
                    SortHole(list, comparer, pivot + 1, right);
                    right = pivot - 1;
                }
            }
        }

        private int PartitionHole(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 将中间数放到左侧后执行
            int median = MedianThree(list, comparer, left, left + ((right - left) / 2), right);
            SortUtils.Swap(list, left, median);

            // 重点：坑交换存在
            // 一开始坑在left处，即取出的pivot，当list[l]=list[r]时，r处就是新坑，以此往复
            T pivot = list[left];
            int l = left;
            int r = right;

            while (l < r)
            {
                while (l < r && comparer.Compare(list[r], pivot) >= 0)
                {
                    r--;
                }
                if (l < r)
                {
                    list[l] = list[r];
                    l++;
                }

                while (l < r && comparer.Compare(list[l], pivot) <= 0)
                {
                    l++;
                }
                if (l < r)
                {
                    list[r] = list[l];
                    r--;
                }
            }

            list[l] = pivot;
            return l;
        }

        #endregion

        #region 5) ThreeWay（三路快排）

        public void SortThreeWay(IList<T> list, IComparer<T> comparer = null)
        {
            if (list.Count < 2) return;
            comparer ??= Comparer<T>.Default;

            SortThreeWay(list, comparer, 0, list.Count - 1);
        }

        private void SortThreeWay(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 通用边界考虑：
            // left < right，即至少还有2个元素，需要排序，当left == right时，只有1个元素了，无需排序
            while (left < right)
            {
                // lt: less than    gt: greater than
                (int lt, int gt) = PartitionThreeWay(list, comparer, left, right);

                // 两边里递归较短的一边，中间 == pivot 的区间直接跳过
                // 本质上和二路快排没啥区别，分区更加精准了而已
                int leftSize = lt - left;
                int rightSize = right - gt;

                if (leftSize < rightSize)
                {
                    SortThreeWay(list, comparer, left, lt - 1);
                    left = gt + 1;
                }
                else
                {
                    SortThreeWay(list, comparer, gt + 1, right);
                    right = lt - 1;
                }
            }
        }

        private (int lt, int gt) PartitionThreeWay(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            // 将中间数放到左侧后执行
            int median = MedianThree(list, comparer, left, left + ((right - left) / 2), right);
            SortUtils.Swap(list, left, median);

            // lt：左界  i：判断值  gt：右界
            //[left ... lt-1]   < pivot
            //[lt ... i-1]      == pivot
            //[i ... gt]        未处理
            //[gt+1 ... right]  > pivot
            T pivot = list[left];
            int lt = left;
            int i = left + 1;
            int gt = right;

            // 必须添加等于
            // 设想：5 5 5 5 1，i一路到1处，如果i<gt那么最后一次就不会判断就换不过去了
            // 本质：[i,gt]是未处理区间，当i==gt时该位置还没有处理，自然需要处理
            while (i <= gt)
            {
                int cmp = comparer.Compare(list[i], pivot);

                if (cmp < 0)
                {
                    SortUtils.Swap(list, lt, i);
                    lt++;
                    i++;
                }
                else if (cmp > 0)
                {
                    SortUtils.Swap(list, i, gt);
                    gt--;
                    // 关键：没有i++，因为换过来的元素是未知的，需要再判断一次
                }
                else
                {
                    i++;
                }
            }

            return (lt, gt);
        }

        #endregion

        #region MedianOfThree

        private int MedianThree(IList<T> list, IComparer<T> comparer, int left, int mid, int right)
        {
            // 直接把三个位置局部排好：left <= mid <= right
            if (comparer.Compare(list[left], list[mid]) > 0)
                SortUtils.Swap(list, left, mid);

            if (comparer.Compare(list[left], list[right]) > 0)
                SortUtils.Swap(list, left, right);

            if (comparer.Compare(list[mid], list[right]) > 0)
                SortUtils.Swap(list, mid, right);

            return mid;
        }

        #endregion
    }

    public class MergeSort<T> : ISort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null)
        {
            Sort(list, comparer, 0, list.Count - 1);
        }

        private void Sort(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            comparer ??= Comparer<T>.Default;

            if (left >= right) return;

            int mid = (left + right) / 2;
            Sort(list, comparer, left, mid);
            Sort(list, comparer, mid + 1, right);

            // 优化：左最大<=右最小，说明已有序，无需Merge
            if (comparer.Compare(list[mid], list[mid + 1]) <= 0) return;

            Merge(list, comparer, left, mid, right);
        }

        private void Merge(IList<T> list, IComparer<T> comparer, int left, int mid, int right)
        {
            T[] temp = new T[right - left + 1];
            // i---左最左，j---右最左，k---初始索引
            int i = left, j = mid + 1, k = 0;

            // 排序，注意：
            // 由于是从最短数组开始合并的，所以两侧数组一直都是有序的，所以说可以通过从左至右依次选取的方式进行排序
            while (i <= mid && j <= right)
            {
                if (comparer.Compare(list[i], list[j]) <= 0)
                {
                    temp[k++] = list[i++];
                }
                else
                {
                    temp[k++] = list[j++];
                }
            }
            // 上述操作可能出现一侧已全部转移，另一侧还剩下多个元素，需要全部依次放入
            while (i <= mid)
            {
                temp[k++] = list[i++];
            }
            while (j <= right)
            {
                temp[k++] = list[j++];
            }

            // 重新转移回原数组中
            for (k = 0; k < temp.Length; k++)
            {
                list[left + k] = temp[k];
            }
        }
    }

    public class HeapSort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null, int way = 0)
        {
            switch (way)
            {
                case 0:
                    Sort1(list, comparer);
                    break;
                case 1:
                    Sort2(list, comparer);
                    break;
            }
        }

        public void Sort1(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            int n = list.Count;

            // 倒序遍历堆(从非叶子节点开始)，进行堆化操作
            // 堆化：简单可以理解为冒泡，小数向下沉大数向上浮
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                SiftDown1(list, comparer, n, i);
            }

            for (int i = n - 1; i > 0; i--)
            {
                SortUtils.Swap(list, 0, i);// 首尾交换
                SiftDown1(list, comparer, i, 0);// 对未完成排序序列堆化
            }
        }

        public void Sort2(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            int n = list.Count;

            // 倒序遍历堆(从非叶子节点开始)，进行堆化操作
            // 堆化：简单可以理解为冒泡，小数向下沉大数向上浮
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                SiftDown2(list, comparer, n, i);
            }

            for (int i = n - 1; i > 0; i--)
            {
                SortUtils.Swap(list, 0, i);// 首尾交换
                SiftDown2(list, comparer, i, 0);// 对未完成排序序列堆化
            }
        }

        private void SiftDown1(IList<T> list, IComparer<T> comparer, int n, int i)
        {
            while (true)
            {
                // max---该节点索引，l---左节点索引，r---右节点索引
                int max = i;
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                // 子节点未出界 且 父节点并非最大，此时更改max
                if (l < n && comparer.Compare(list[l], list[max]) > 0)
                {
                    max = l;
                }

                if (r < n && comparer.Compare(list[r], list[max]) > 0)
                {
                    max = r;
                }

                if (max == i) // 条件满足，不再执行(父节点为最大值)
                {
                    break;
                }

                SortUtils.Swap(list, i, max); // 交换节点中的值
                i = max; // 向下一层
            }
        }

        private void SiftDown2(IList<T> list, IComparer<T> comparer, int n, int i)
        {
            T value = list[i];

            while (true)
            {
                int left = i * 2 + 1;
                if (left >= n) break;

                int right = left + 1;

                // 找到更大的子节点
                int child = right < n && comparer.Compare(list[right], list[left]) > 0
                    ? right
                    : left;

                // 父节点已经不小于最大子节点，结束
                if (comparer.Compare(value, list[child]) >= 0)
                    break;

                // 子节点上移
                list[i] = list[child];
                i = child;
            }

            list[i] = value;
        }
    }
}
