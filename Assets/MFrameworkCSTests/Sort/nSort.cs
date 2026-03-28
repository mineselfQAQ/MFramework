using System;
using System.Collections.Generic;

namespace MFrameworkCSTests.Sort
{
    /// <summary>
    /// 桶排序
    /// <para>尽量均匀分配到不同桶中，在桶内排序后再合并回原序列</para>
    /// <para>需要外部提供桶映射规则：元素 -> 桶索引</para>
    /// <para>理想情况下时间复杂度 O(n)，空间复杂度 O(n)，稳定性取决于桶内排序算法</para>
    /// </summary>
    public sealed class BucketSort<T>
    {
        private readonly Func<T, int, int> bucketSelector;
        private readonly IComparer<T> comparer;

        public BucketSort(Func<T, int, int> bucketSelector, IComparer<T> comparer = null)
        {
            this.bucketSelector = bucketSelector ?? throw new ArgumentNullException(nameof(bucketSelector));
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        public void Sort(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count < 2) return;

            int bucketCount = Math.Max(1, list.Count / 2);
            List<List<T>> buckets = new List<List<T>>(bucketCount);

            for (int i = 0; i < bucketCount; i++)
            {
                buckets.Add(new List<T>());
            }

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                int bucketIndex = bucketSelector(item, bucketCount);

                if (bucketIndex < 0 || bucketIndex >= bucketCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(bucketSelector), $"桶索引超出范围：{bucketIndex}");
                }

                buckets[bucketIndex].Add(item);
            }

            int writeIndex = 0;
            foreach (List<T> bucket in buckets)
            {
                bucket.Sort(comparer);

                foreach (T item in bucket)
                {
                    list[writeIndex++] = item;
                }
            }
        }
    }

    /// <summary>
    /// 计数排序（基础版）
    /// <para>将键值作为索引统计出现次数，再按计数结果回写</para>
    /// <para>需要外部提供非负整数键：元素 -> key</para>
    /// <para>时间复杂度 O(n + k)，空间复杂度 O(n + k)，非稳定排序</para>
    /// </summary>
    public sealed class CountingSortNaive<T>
    {
        private readonly Func<T, int> keySelector;

        public CountingSortNaive(Func<T, int> keySelector)
        {
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public void Sort(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count < 2) return;

            int max = 0;
            for (int i = 0; i < list.Count; i++)
            {
                int key = keySelector(list[i]);
                if (key < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(keySelector), "CountingSortNaive 仅支持非负整数键。");
                }

                if (key > max)
                {
                    max = key;
                }
            }

            int[] counter = new int[max + 1];
            List<T>[] groups = new List<T>[max + 1];

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                int key = keySelector(item);

                counter[key]++;

                groups[key] ??= new List<T>();
                groups[key].Add(item);
            }

            int writeIndex = 0;
            for (int key = 0; key <= max; key++)
            {
                if (counter[key] == 0) continue;

                List<T> group = groups[key];
                for (int i = 0; i < group.Count; i++)
                {
                    list[writeIndex++] = group[i];
                }
            }
        }
    }

    /// <summary>
    /// 稳定计数排序
    /// <para>将计数数组转为前缀和数组，再倒序回填结果数组</para>
    /// <para>需要外部提供非负整数键：元素 -> key</para>
    /// <para>时间复杂度 O(n + k)，空间复杂度 O(n + k)，稳定排序</para>
    /// </summary>
    public sealed class CountingSort<T>
    {
        private readonly Func<T, int> keySelector;

        public CountingSort(Func<T, int> keySelector)
        {
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public void Sort(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count < 2) return;

            int max = 0;
            for (int i = 0; i < list.Count; i++)
            {
                int key = keySelector(list[i]);
                if (key < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(keySelector), "CountingSort 仅支持非负整数键。");
                }

                if (key > max)
                {
                    max = key;
                }
            }

            int[] counter = new int[max + 1];
            for (int i = 0; i < list.Count; i++)
            {
                counter[keySelector(list[i])]++;
            }

            for (int i = 0; i < max; i++)
            {
                counter[i + 1] += counter[i];
            }

            T[] result = new T[list.Count];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T item = list[i];
                int key = keySelector(item);
                result[counter[key] - 1] = item;
                counter[key]--;
            }

            for (int i = 0; i < result.Length; i++)
            {
                list[i] = result[i];
            }
        }
    }
}
