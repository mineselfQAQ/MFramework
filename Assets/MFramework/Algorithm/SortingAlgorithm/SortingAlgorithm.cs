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
        /// <para>快速排序---分治处理，每次都将数组满足左侧<当前<右侧，递归至一个元素完成排序</para>
        /// 自适应排序，时间复杂度O(nlogn) | 原地排序，空间复杂度O(n) | 非稳定排序
        /// </summary>
        public static void QuickSort(int[] arr)
        {
            QuickSort(arr, 0, arr.Length - 1);
        }
        private static void QuickSort(int[] arr, int left, int right)
        {
            if (left >= right) return;

            int pivot = Partition(arr, left, right);
            QuickSort(arr, left, pivot - 1);
            QuickSort(arr, pivot + 1, right);
        }
        private static int Partition(int[] arr, int left, int right)
        {
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
                Swap(ref arr[l], ref arr[r]);//将两个不符合的元素交换位置(就符合了)
            }
            Swap(ref arr[l], ref arr[left]);//将基准值调整至分割位置
            return l;
        }


        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
