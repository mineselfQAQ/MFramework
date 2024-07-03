using System;
using System.Collections;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public class MDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        //类似于Hashtable中的Bucket
        private struct Entry
        {
            public int hashCode;
            public int next;
            public TKey key;
            public TValue value;
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private MDictionary<TKey, TValue> dictionary;

            private KeyValuePair<TKey, TValue> current;

            private int index;

            private int getEnumeratorRetType;//模式，一般为2，也就是键值对模式(泛型)

            internal const int DictEntry = 1;

            internal const int KeyValuePair = 2;

            internal Enumerator(MDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = default(KeyValuePair<TKey, TValue>);
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        throw new Exception();
                    }

                    //模式1，也就是非泛型模式
                    if (getEnumeratorRetType == 1)
                    {
                        return new DictionaryEntry(current.Key, current.Value);
                    }

                    //模式2，键值对模式，也就是泛型模式
                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entries[index].hashCode >= 0)
                    {
                        current = new KeyValuePair<TKey, TValue>(dictionary.entries[index].key, dictionary.entries[index].value);
                        index++;
                        return true;
                    }

                    index++;
                }

                index = dictionary.count + 1;
                current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default(KeyValuePair<TKey, TValue>);
            }
        }

        public sealed class KeyCollection : IEnumerable<TKey>
        {
            public struct Enumerator : IEnumerator<TKey>
            {
                private MDictionary<TKey, TValue> dictionary;

                private int index;

                private TKey currentKey;

                public TKey Current//IEnumerator<T>的Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

                object IEnumerator.Current//IEnuemrator的Current
                {
                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            throw new Exception();
                        }

                        return currentKey;
                    }
                }

                internal Enumerator(MDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    index = 0;
                    currentKey = default(TKey);
                }

                public void Dispose(){ }

                public bool MoveNext()
                {
                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentKey = dictionary.entries[index].key;
                            index++;
                            return true;
                        }

                        index++;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(TKey);
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    currentKey = default(TKey);
                }
            }

            private MDictionary<TKey, TValue> dictionary;

            public int Count
            {
                get
                {
                    return dictionary.Count;
                }
            }

            public KeyCollection(MDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception();
                }

                this.dictionary = dictionary;
            }

            //看起来是想要隐藏，目前来看，优先会用1，然后用3，然后用2
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator IEnumerable.GetEnumerator()//IEnumerable的GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()//IEnumerable<T>的GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
        }
        public sealed class ValueCollection : IEnumerable<TValue>
        {
            public struct Enumerator : IEnumerator<TValue>
            {
                private MDictionary<TKey, TValue> dictionary;

                private int index;

                private TValue currentValue;

                public TValue Current//IEnumerator<T>的Current
                {
                    get
                    {
                        return currentValue;
                    }
                }

                object IEnumerator.Current//IEnuemrator的Current
                {
                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            throw new Exception();
                        }

                        return currentValue;
                    }
                }

                internal Enumerator(MDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    index = 0;
                    currentValue = default(TValue);
                }

                public void Dispose() { }

                public bool MoveNext()
                {
                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }

                        index++;
                    }

                    index = dictionary.count + 1;
                    currentValue = default(TValue);
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    currentValue = default(TValue);
                }
            }

            private MDictionary<TKey, TValue> dictionary;

            public int Count
            {
                get
                {
                    return dictionary.Count;
                }
            }

            public ValueCollection(MDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception();
                }

                this.dictionary = dictionary;
            }

            //看起来是想要隐藏，目前来看，优先会用1，然后用3，然后用2
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator IEnumerable.GetEnumerator()//IEnumerable的GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()//IEnumerable<T>的GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
        }



        private int[] buckets;//entries索引

        private Entry[] entries;//实际存储元素的数组

        private int count;//当前所在的entries的位置  Tip：可以认为是元素数量，但是如果Remove()过数量会发生变化

        private int freeList;

        private int freeCount;

        private IEqualityComparer<TKey> comparer;//用于进行Equals()和GetHashCode()操作

        private KeyCollection keys;

        private ValueCollection values;

        //准确的元素数量
        public int Count
        {
            get
            {
                return count - freeCount;
            }
        }

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

        public TValue this[TKey key]
        {
            get
            {
                int num = FindEntry(key);//寻找Entry
                if (num >= 0)//如果存在Entry就返回
                {
                    return entries[num].value;
                }

                throw new Exception();
            }
            set
            {
                Insert(key, value, add: false);//改值
            }
        }

        public KeyCollection Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }
        public ValueCollection Values
        {
            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, add: true);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new Exception();
            }

            if (buckets != null)
            {
                int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//hashcode
                int num2 = num % buckets.Length;//index
                int num3 = -1;
                //同Insert()
                for (int num4 = buckets[num2]; num4 >= 0; num4 = entries[num4].next)
                {
                    //找到了要删除的Entry
                    if (entries[num4].hashCode == num && comparer.Equals(entries[num4].key, key))
                    {
                        //Tip：只有这一种可能，
                        //如果途中发生entries[num4].next < 0的话，下一轮循环就会退出了，也就没有机会得到num3 < 0了
                        if (num3 < 0)//第一个就中了，跳过它直接改链(头删)
                        {
                            buckets[num2] = entries[num4].next;
                        }
                        else//一般情况的链表删除方式(A->B->C变为A->C)
                        {
                            entries[num3].next = entries[num4].next;//Tip:此时的num3就是上一个元素
                        }

                        entries[num4].hashCode = -1;
                        entries[num4].next = freeList;//链接1
                        entries[num4].key = default(TKey);
                        entries[num4].value = default(TValue);
                        freeList = num4;//链接2
                        freeCount++;
                        return true;
                    }

                    num3 = num4;//num3存放着上一个index
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
                for (int j = 0; j < count; j++)
                {
                    if (entries[j].hashCode >= 0 && @default.Equals(entries[j].value, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i] = -1;
                }

                Array.Clear(entries, 0, count);
                count = 0;
                freeList = -1;
                freeCount = 0;
            }
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

            //之所以取低31位，是因为需要保证数是正数，这样才能做接下来的取余操作
            int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//取低31位hashcode
            int num2 = num % buckets.Length;//index(将hashcode映射至桶范围)
            int num3 = 0;//很像Hashtable中的count，但是是Insert()中即时计算的

            //=====更改情况=====
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

            //=====获取存放新元素的entries索引=====
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

            //=====添加情况=====
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

            //强制更新Hashcode情况
            if (forceNewHashCodes)
            {
                for (int j = 0; j < count; j++)
                {
                    if (array2[j].hashCode != -1)
                    {
                        array2[j].hashCode = comparer.GetHashCode(array2[j].key) & 0x7FFFFFFF;//去除最高位
                    }
                }
            }

            //重新链接，大致流程：
            //此时我们的buckets已经清空了，我们需要将所有的entries归入buckets
            //从元素数组的第一个开始，只要其中有hashCode，就说明该处有存在元素
            //然后依次将entry进行头插法链接即可
            for (int k = 0; k < count; k++)
            {
                if (array2[k].hashCode >= 0)//最高位为0时
                {
                    //大致就是：新元素指向上一个元素，buckets[newIndex]指向新元素
                    int num = array2[k].hashCode % newSize;//newIndex
                    array2[k].next = array[num];
                    array[num] = k;
                }
            }

            buckets = array;
            entries = array2;
        }

        private int FindEntry(TKey key)
        {
            if (key == null)
            {
                throw new Exception();
            }

            if (buckets != null)
            {
                int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//hashcode
                //依次查询桶链表中的每一个Entry
                for (int num2 = buckets[num % buckets.Length]; num2 >= 0; num2 = entries[num2].next)
                {
                    //找到匹配Entry就返回
                    if (entries[num2].hashCode == num && comparer.Equals(entries[num2].key, key))
                    {
                        return num2;
                    }
                }
            }

            return -1;
        }

        //看起来是想要隐藏，优先会用1
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, 2);
        }
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }
    }
}