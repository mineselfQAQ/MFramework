using System;
using System.Collections.Generic;

namespace MFramework
{
    public class MDictionary<TKey, TValue>
    {
        //类似于Hashtable中的Bucket
        private struct Entry
        {
            public int hashCode;
            public int next;
            public TKey key;
            public TValue value;
        }

        public sealed class KeyCollection { }
        public sealed class ValueCollection { }



        private int[] buckets;//entries索引

        private Entry[] entries;//实际存储元素的数组

        private int count;

        private int freeList;

        private int freeCount;

        private IEqualityComparer<TKey> comparer;//用于进行Equals()和GetHashCode()操作

        private KeyCollection keys;

        private ValueCollection values;

        public MDictionary() : this(0, null) { }//默认容量为0，这意味着不报错也不Initialize()
        public MDictionary(int capacity) : this(capacity, null){ }
        public MDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
            {
                throw new Exception();
            }

            if (capacity > 0)
            {
                Initialize(capacity);
            }

            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, add: true);
        }

        /// <summary>
        /// 构造函数中的初始化方法
        /// </summary>
        private void Initialize(int capacity)
        {
            //大小---设置容量向上取最小质数
            int prime = MHashHelpers.GetPrime(capacity);
            buckets = new int[prime];
            //初始化为-1
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }
            entries = new Entry[prime];
            freeList = -1;
        }

        /// <summary>
        /// 加入键值对的内部实现
        /// </summary>
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
            {
                throw new Exception();
            }

            //桶还未初始化，需要Initialize()，对应无参构造函数情况
            if (buckets == null)
            {
                Initialize(0);//意味着是最小质数，为3
            }

            int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//取低31位hashcode
            int num2 = num % buckets.Length;//index(将hashcode映射至桶范围)
            int num3 = 0;//很像Hashtable中的count，但是是Insert()中即时计算的
            //流程：
            //在buckets中检查index处的int值，如果该处已被赋值，说明有指向，可以进行循环
            //每次都会检查entries中num4处的key与hashcode：
            //·如果匹配，就说明找到对应位置
            //·如果不匹配，就说明没有找对位置，需要通过该Entry中的next找到下一位置
            for (int num4 = buckets[num2]; num4 >= 0; num4 = entries[num4].next)
            {
                //数组中的hashcode/key与当前传入的相同---找到了相同的key
                if (entries[num4].hashCode == num && comparer.Equals(entries[num4].key, key))
                {
                    if (add)//此时添加元素是不正确的，只能更改
                    {
                        throw new Exception();
                    }
                    //更改元素
                    entries[num4].value = value;
                    return;
                }

                num3++;//统计这一组链表存储元素数量
            }

            int num5;
            if (freeCount > 0)//如果有空余位置
            {
                num5 = freeList;//取出其中一个位置
                freeList = entries[num5].next;//保存下一个空余位置
                freeCount--;
            }
            else//如果没有空余位置，那么只能自己去找了
            {
                if (count == entries.Length)//数组已满，需要扩容
                {
                    Resize();//扩容
                    num2 = num % buckets.Length;//buckets长度有所变化，重新计算index
                }

                num5 = count;//存放元素在entries数组中的位置
                count++;//提前放置下一位置
            }

            entries[num5].hashCode = num;
            //如果没有发生碰撞，next值会为默认值-1
            //如果发生碰撞，会更改指向，如：
            //第一次index为4，那么在entries[0]处会存放该数据，此时buckets[4]=0
            //第二次index还为4，那么发生了哈希碰撞，在entries[1]处存放了数据，除此以外next会被设为0，同时buckets[4]也被更改为1
            //那么现在其实有一个链表关系：
            //buckets[4]--->entries[1]--->entries[0]
            entries[num5].next = buckets[num2];
            entries[num5].key = key;
            entries[num5].value = value;
            buckets[num2] = num5;//使buckets[nums2]处指向entreis的nums5处
            //num3过大，意味着出现了极大量的哈希冲突(每次都找到同一个index)
            if (num3 > 100 /*&& MHashHelpers.IsWellKnownEqualityComparer(comparer)*/)
            {
                //Tip：原本会对不够优秀的comparer进行重新创建
                //comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
                Resize(entries.Length, forceNewHashCodes: true);
            }
        }

        private void Resize()
        {
            Resize(MHashHelpers.ExpandPrime(count), forceNewHashCodes: false);
        }
        private void Resize(int newSize, bool forceNewHashCodes)
        {
            //初始化新buckets
            int[] array = new int[newSize];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
            //初始化新entries
            Entry[] array2 = new Entry[newSize];
            Array.Copy(entries, 0, array2, 0, count);
            if (forceNewHashCodes)
            {
                for (int j = 0; j < count; j++)
                {
                    if (array2[j].hashCode != -1)
                    {
                        array2[j].hashCode = comparer.GetHashCode(array2[j].key) & 0x7FFFFFFF;
                    }
                }
            }

            for (int k = 0; k < count; k++)
            {
                if (array2[k].hashCode >= 0)
                {
                    int num = array2[k].hashCode % newSize;
                    array2[k].next = array[num];
                    array[num] = k;
                }
            }

            buckets = array;
            entries = array2;
        }
    }
}