using System;

namespace MFramework
{
    public static class SortingAlgorithm
    {
        /// <summary>
        /// <para>选择排序---对于每一轮循环，选择一个最小元素，将其放置到已排序区间的后面</para>
        /// 非自适应排序，时间复杂度O(n^2) | 原地排序，空间复杂度O(1) | 不稳定排序
        /// </summary>
        public static void SelectionSort(int[] arr)
        {
            //需要排序(长度-1)次，因为只剩一个元素时，无需排序
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int min = i;
                //寻找最小元素
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] < arr[min])
                    {
                        min = j;
                    }
                }
                //交换
                Swap(ref arr[i], ref arr[min]);
            }
        }

        /// <summary>
        /// <para>冒泡排序---连续地比较与交换相邻元素实现排序</para>
        /// 自适应排序，时间复杂度O(n^2) | 原地排序，空间复杂度O(1) | 稳定排序
        /// </summary>
        public static void BubbleSort(int[] arr)
        {
            //每轮进行对一个元素的"冒泡"操作
            for(int i = 0; i < arr.Length - 1; i++)
            {
                bool flag = false;

                //从前至后每两个元素进行交换从而"冒泡"
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(ref arr[j], ref arr[j + 1]);
                        flag = true;
                    }
                }

                if (!flag) break;//如果一轮循环中没有进行交换操作，说明排序已完成，退出即可
            }
        }

        /// <summary>
        /// <para>插入排序---每次将一个未排序元素按大小插入排序元素中，直至整理完毕</para>
        /// 自适应排序，时间复杂度O(n^2) | 原地排序，空间复杂度O(1) | 稳定排序
        /// </summary>
        public static void InsertionSort(int[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int temp = arr[i];//暂存值
                int j = i - 1;//index

                //如果待存值更小，为其"腾空间"(后移)
                while (j >= 0 && arr[j] > temp)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                //插入值
                arr[j + 1] = temp;
            }
        }

        /// <summary>
        /// <para>快速排序---分治处理，每次都将数组满足|左侧<当前<右侧|，递归至一个元素完成排序</para>
        /// 自适应排序，时间复杂度O(nlogn) | 原地排序，空间复杂度O(n) | 非稳定排序
        /// </summary>
        public static void QuickSort(int[] arr)
        {
            QuickSort(arr, 0, arr.Length - 1);
        }
        private static void QuickSort(int[] arr, int left, int right)
        {
            while (left < right)
            {
                int pivot = Partition(arr, left, right);

                //只对短的那个进行递归
                if (pivot - left < right - pivot)
                {
                    QuickSort(arr, left, pivot - 1);//左短，动right
                    left = pivot + 1;//另一半，由while语句处理
                }
                else
                {
                    QuickSort(arr, pivot + 1, right);//右短，动left
                    right = pivot - 1;//另一半，由while语句处理
                }
            }
        }
        private static int Partition(int[] arr, int left, int right)
        {
            int median = MedianThree(arr, left, (left + right) / 2, right);
            Swap(ref arr[left], ref arr[median]);

            int l = left, r = right;
            while (l < r)//如果只有一个元素不需要操作
            {
                //寻找不符合元素索引(左侧的大元素，右侧的小元素)
                while (l < r && arr[r] >= arr[left])
                {
                    r--;
                }
                while (l < r && arr[l] <= arr[left])
                {
                    l++;
                }
                Swap(ref arr[l], ref arr[r]);//将两个不符合的元素交换位置(指针重合处为分界点，交换后符合)
            }
            Swap(ref arr[l], ref arr[left]);//将基准值调整至分割位置
            return l;
        }
        private static int MedianThree(int[] arr, int left, int mid, int right)
        {
            if ((arr[left] < arr[mid] && arr[right] > arr[mid]) || (arr[right] < arr[mid] && arr[left] > arr[mid]))
            {
                return mid;
            }
            else if ((arr[mid] < arr[left] && arr[right] > arr[left]) || (arr[right] < arr[left] && arr[mid] > arr[left]))
            {
                return left;
            }
            else
            {
                return right;
            }
        }

        /// <summary>
        /// <para>归并排序---分治处理，将数组递归拆分成最短数组，再不断合并成有序数组</para>
        /// 非自适应排序，时间复杂度O(nlogn) | 非原地排序，空间复杂度O(n) | 稳定排序
        /// </summary>
        public static void MergeSort(int[] arr)
        {
            MergeSort(arr, 0, arr.Length - 1);
        }
        private static void MergeSort(int[] arr, int left, int right)
        {
            //划分到一个元素时退出
            if (left >= right) return;

            int mid = (left + right) / 2;
            MergeSort(arr, left, mid);
            MergeSort(arr, mid + 1, right);
            Merge(arr, left, mid, right);
        }
        private static void Merge(int[] arr, int left, int mid, int right)
        {
            int[] tempArr = new int[right - left + 1];//创建临时数组，大小就是当前轮次的大小

            //i---左最左，j---右最左，k---初始索引
            int i = left, j = mid + 1, k = 0;
            //排序，注意：
            //由于是从最短数组开始合并的，所以两侧数组一直都是有序的，所以说可以通过从左至右依次选取的方式进行排序
            while (i <= mid && j <= right)
            {
                if (arr[i] <= arr[j])
                {
                    tempArr[k++] = arr[i++];
                }
                else
                {
                    tempArr[k++] = arr[j++];
                }
            }
            //上述操作可能出现一侧已全部转移，另一侧还剩下多个元素，需要全部依次放入
            while (i <= mid)
            {
                tempArr[k++] = arr[i++];
            } 
            while (j <= right)
            {
                tempArr[k++] = arr[j++];
            }

            //重新转移回原数组中
            for (k = 0; k < tempArr.Length; k++)
            {
                arr[left + k] = tempArr[k];
            }
        }

        /// <summary>
        /// <para>堆排序---通过大顶堆，每次将最大元素放至堆尾，并对其余元素堆化保证大顶堆性质</para>
        /// 非自适应排序，时间复杂度O(nlogn) | 原地排序，空间复杂度O(1) | 非稳定排序
        /// </summary>
        public static void HeapSort(int[] arr)
        {
            //倒序遍历堆(从非叶子节点开始)，进行堆化操作
            for (int i = arr.Length / 2 - 1; i >= 0; i--)
            {
                SiftDown(arr, arr.Length, i);
            }

            for (int i = arr.Length - 1; i > 0; i--)
            {
                Swap(ref arr[0], ref arr[i]);//首尾交换
                SiftDown(arr, i, 0);//对未完成排序序列堆化
            }
        }
        private static void SiftDown(int[] arr, int n, int i)
        {
            while (true)
            {
                //max---该节点索引，l---左节点索引，r---右节点索引
                int max = i;
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                //子节点未出界 且 父节点并非最大，此时更改max
                if (l < n && arr[l] > arr[max])
                {
                    max = l;
                }
                if (r < n && arr[r] > arr[max])
                {
                    max = r;
                }

                if (max == i)//条件满足，不再执行(父节点为最大值)
                {
                    break;
                }

                Swap(ref arr[i], ref arr[max]);//交换节点中的值
                i = max;//向下一层
            }
        }

        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
