using System;
using System.Collections;

namespace MFramework.DLC
{
    public struct MDictionaryEntry
    {
        private object _key;

        private object _value;

        public object Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public MDictionaryEntry(object key, object value)
        {
            _key = key;
            _value = value;
        }
    }

    public interface IDictionaryEnumerator : IEnumerator
    {
        MDictionaryEntry Entry { get; }
        object Key { get; }
        object Value { get; }
    }

    public class MHashtable : IEnumerable
    {
        private class KeyCollection : IEnumerable
        {
            private MHashtable _hashtable;

            internal KeyCollection(MHashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public IEnumerator GetEnumerator()
            {
                return new MHashtableEnumerator(_hashtable, 1);
            }
        }

        private class ValueCollection : IEnumerable
        {
            private MHashtable _hashtable;

            internal ValueCollection(MHashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public IEnumerator GetEnumerator()
            {
                return new MHashtableEnumerator(_hashtable, 2);
            }
        }

        private class MHashtableEnumerator : IDictionaryEnumerator, IEnumerator
        {
            private MHashtable hashtable;

            private int bucket;//桶容量

            private bool current;//当前位置是否有内容

            private int getObjectRetType;//输出模式

            private object currentKey;

            private object currentValue;

            public object Key
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    return currentKey;
                }
            }

            public object Value
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    return currentValue;
                }
            }

            public MDictionaryEntry Entry
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    return new MDictionaryEntry(currentKey, currentValue);
                }
            }

            public object Current
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    //根据选择的getObjectRetType确定获取的内容：
                    //1---Key
                    //2---Value
                    //3---KeyValuePair
                    if (getObjectRetType == 1)
                    {
                        return currentKey;
                    }
                    if(getObjectRetType == 2) 
                    {
                        return currentValue;
                    }
                    //getObjectRetType == 3
                    return new DictionaryEntry(currentKey, currentValue);
                }
            }

            internal MHashtableEnumerator(MHashtable hashtable, int getObjRetType)
            {
                this.hashtable = hashtable;
                bucket = hashtable.buckets.Length;
                current = false;
                getObjectRetType = getObjRetType;
            }

            public bool MoveNext()
            {
                while (bucket > 0)
                {
                    bucket--;
                    object key = hashtable.buckets[bucket].key;//取出key
                    //如果有值，就取出所有内容
                    if (key != null && key != hashtable.buckets)
                    {
                        currentKey = key;
                        currentValue = hashtable.buckets[bucket].val;
                        current = true;
                        return true;
                    }
                }

                current = false;
                return false;
            }

            public void Reset()
            {
                current = false;
                bucket = hashtable.buckets.Length;
                currentKey = null;
                currentValue = null;
            }
        }

        private struct bucket
        {
            public object key;

            public object val;

            public int hash_coll;//哈希码，最高位存储哈希冲突标记
        }

        private bucket[] buckets;

        private int count;

        private int occupancy;//哈希冲突次数

        private int loadsize;//负载极点---真正的扩容点

        private float loadFactor;//负载因子---控制扩容时机


        private IEnumerable keys;

        private IEnumerable values;

        public int Count => count;

        public IEnumerable Keys
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
        public IEnumerable Values
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

        public object this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new Exception();
                }

                bucket[] array = buckets;
                uint seed;
                uint incr;
                uint num = InitHash(key, array.Length, out seed, out incr);
                int num2 = 0;
                int num3 = (int)(seed % (uint)array.Length);
                bucket bucket;
                do
                {
                    bucket = array[num3];

                    if (bucket.key == null)
                    {
                        return null;
                    }

                    if ((bucket.hash_coll & 0x7FFFFFFF) == num && KeyEquals(bucket.key, key))
                    {
                        return bucket.val;
                    }

                    num3 = (int)((num3 + incr) % (long)(uint)array.Length);
                }
                while (bucket.hash_coll < 0 && ++num2 < array.Length);

                //没有哈希冲突还得不到值，说明没有值，返回null
                return null;
            }
            set
            {
                Insert(key, value, add: false);
            }
        }

        public MHashtable() : this(0, 1.0f) { }
        public MHashtable(int capacity) : this(capacity, 1.0f) { }
        public MHashtable(int capacity, float loadFactor)
        {
            //capacity---容量，指的是想要开辟的桶空间
            //loadFactor---负载因子，指的是空间的余量控制
            //num---capacity/loadFactor，指的是根据负载因子获取的桶空间
            //num2---根据num获取的一个质数，指的是真正开辟的桶空间
            //那么意思就是：
            //我们可能传入100，这意味着我们想要开辟100个元素的空间，但是事实上会开辟更多的空间，这主要取决于负载因子

            //容量不可能小于0
            if (capacity < 0)
            {
                throw new Exception();
            }

            //负载因子取值范围应该在[0,1]之间，[0,0.1)太小了也排除
            if (loadFactor < 0.1f || loadFactor > 1.0f)
            {
                throw new Exception();
            }

            this.loadFactor = 0.72f * loadFactor;

            double num = (float)capacity / this.loadFactor;
            //由于有负载因子的存在，容量有一定的限制
            if (num > 2147483647.0)
            {
                throw new Exception();
            }

            //计算真正的桶数量(下限为3(最小质数)，正常为比num大的一个规定质数)
            int num2 = ((num > 3.0) ? MHashHelpers.GetPrime((int)num) : 3);
            buckets = new bucket[num2];//创建桶(使用质数，用于减少index的哈希冲突)
            loadsize = (int)(this.loadFactor * (float)num2);//计算负载极点---真正的扩容点
        }

        public void Add(object key, object value)
        {
            Insert(key, value, add: true);
        }

        public void Remove(object key)
        {
            //移除不可能移除null
            if (key == null)
            {
                throw new Exception();
            }

            uint seed;//hashcode1
            uint incr;//indexIncr，没找到都会以此值进行步进
            uint num = InitHash(key, buckets.Length, out seed, out incr);//hashcode1
            int num2 = 0;//迭代次数
            int num3 = (int)(seed % (uint)buckets.Length);//index1
            bucket bucket;

            //循环，直到发现没有哈希冲突的bucket(循环中会完成移除操作)
            do
            {
                bucket = buckets[num3];//找到放入的bucket
                //bucket中存储的hashcode与hashcode1一致且bucket中存储的key与key相同
                //也就是bucket确实是需要移除的那个
                //0x7FFFFFFF---0111...1，进行&操作也就是排除最高位
                if ((bucket.hash_coll & 0x7FFFFFFF) == num && KeyEquals(bucket.key, key))
                {
                    buckets[num3].hash_coll &= int.MinValue;//取出哈希冲突标识(最高位为1/0，其它位都为0)
                    //当存在哈希冲突时
                    if (buckets[num3].hash_coll != 0)
                    {
                        buckets[num3].key = buckets;//将key设为buckets，用来标识
                    }
                    else//正常来说清除key即可
                    {
                        buckets[num3].key = null;
                    }

                    buckets[num3].val = null;
                    count--;
                    break;
                }

                num3 = (int)((num3 + incr) % (long)(uint)buckets.Length);//步进后的新index
            }
            while (bucket.hash_coll < 0 && ++num2 < buckets.Length);//如果找的次数甚至超过了桶的长度，就不找了
        }

        public bool ContainsKey(object key)
        {
            //找key，key必不可能为null
            if (key == null)
            {
                throw new Exception();
            }

            //常规index算法
            bucket[] array = buckets;
            uint seed;
            uint incr;
            uint num = InitHash(key, array.Length, out seed, out incr);
            int num2 = 0;
            int num3 = (int)(seed % (uint)array.Length);
            bucket bucket;

            do
            {
                bucket = array[num3];//取出bucket

                //该位置的key必不可能为null
                if (bucket.key == null)
                {
                    return false;
                }

                //该位置就是key所对应的位置
                if ((bucket.hash_coll & 0x7FFFFFFF) == num && KeyEquals(bucket.key, key))
                {
                    return true;
                }

                num3 = (int)((num3 + incr) % (long)(uint)array.Length);
            }
            while (bucket.hash_coll < 0 && ++num2 < array.Length);
            //条件1说明了：
            //一个位置存在key(情况1不满足)，且key与传入的key不相等(情况2不满足)，
            //这意味着发生了冲突，但没有冲突(条件1)，这是不可能的，返回false

            return false;
        }

        public bool ContainsValue(object value)
        {
            if (value == null)//寻找某个value为null的位置
            {
                int num = buckets.Length;
                while (--num >= 0)
                {
                    //该位置必须存在且value为null
                    if (buckets[num].key != null && buckets[num].key != buckets && buckets[num].val == null)
                    {
                        return true;
                    }
                }
            }
            else//一般寻找
            {
                int num = buckets.Length;
                while (--num >= 0)
                {
                    object val = buckets[num].val;
                    //判断val是否等于value(条件2)，如果val已经是null了是必不可能的(条件1)
                    if (val != null && val.Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Insert(object key, object nvalue, bool add)
        {
            //想要添加的话key不可能为null
            if (key == null)
            {
                throw new Exception();
            }

            //存入数量超过负载限度
            if (count >= loadsize)
            {
                expand();
            }
            //发生大量冲突且容量超过100(核心是发生大量冲突)
            //注意：occupancy一个位置只能设置一次，这意味着统计的是有多少位置发生过冲突
            else if (occupancy > loadsize && count > 100)
            {
                rehash();
            }

            uint seed;//hashcode1
            uint incr;//hashcode2
            uint num = InitHash(key, buckets.Length, out seed, out incr);//hashcode1
            int num2 = 0;//迭代次数
            int num3 = -1;//emptySlotNumber
            int num4 = (int)(seed % (uint)buckets.Length);//index

            do
            {
                //如果该处以前发生过哈希碰撞(条件2)而且目前也存在哈希碰撞(条件3)
                //在没有备用位置的情况下找到了一个不好(发生过哈希碰撞)的空位
                if (num3 == -1 && buckets[num4].key == buckets && buckets[num4].hash_coll < 0)
                {
                    num3 = num4;//如果num3还没有设置过(条件1)，那么就将该index暂存于num3中
                }

                //如果该位置没有使用过(情况1)
                if (buckets[num4].key == null || (buckets[num4].key == buckets && (buckets[num4].hash_coll & 0x80000000u) == 0L))
                {
                    //针对情况2---是一个不太好(发生过哈希碰撞，但rehash()后不再是)的空位
                    //如果已经保存了一个可用index
                    if (num3 != -1)
                    {
                        num4 = num3;//使用这个暂存index
                    }

                    buckets[num4].val = nvalue;
                    buckets[num4].key = key;
                    buckets[num4].hash_coll |= (int)num;
                    count++;

                    return;
                }

                //bucket中存储的hashcode与hashcode1一致且bucket中存储的key与key相同
                //说明这个位置已经存在，需要进行set操作
                if ((buckets[num4].hash_coll & 0x7FFFFFFF) == num && KeyEquals(buckets[num4].key, key))
                {
                    //此处为更改操作而非添加操作，add为true的话是不正确的
                    if (add)
                    {
                        throw new Exception();
                    }

                    buckets[num4].val = nvalue;

                    return;
                }

                //至此，该位置一定发生冲突，
                //因为虽然key中有值，但是有的是另一个已经存入的值，是不匹配的，这不是需要改而是发生冲突(有关改情况)

                //含义为发生碰撞，但是并没有设置(hash_coll最高位为0)
                if (num3 == -1 && buckets[num4].hash_coll >= 0)
                {
                    //设置哈希冲突状态
                    buckets[num4].hash_coll |= int.MinValue;
                    occupancy++;//好像是一个位置只能设置一次
                }

                num4 = (int)((num4 + incr) % (long)(uint)buckets.Length);//步进后的新index
            }
            while (++num2 < buckets.Length);//如果找了整个桶的长度次还没找到，就不找了

            //迭代完毕，只能存储在预存位置了
            if (num3 != -1)
            {
                buckets[num3].val = nvalue;
                buckets[num3].key = key;
                buckets[num3].hash_coll |= (int)num;
                count++;

                //已经全部找过一遍了，并没有一个优质的位置，这意味着buckets的状态非常不好
                //除此以外如果桶还挺长的，有100个以上，这种时候还是发生了，说明是真的不好(比如说就3个位置那么发生非常正常)
                if (buckets.Length > 100)
                {
                    rehash(buckets.Length, forceNewHashCode: true);
                }

                return;
            }

            //函数执行结束了都没有存储成功，这是错误的
            throw new Exception();
        }

        private uint InitHash(object key, int hashsize, out uint seed, out uint incr)
        {
            //result = seed = 哈希冲突标识为0的hashcode
            uint result = (seed = (uint)key.GetHashCode() & 0x7FFFFFFFu);
            incr = 1 + seed * 101 % (uint)(hashsize - 1);//indexIncr，应该可以称为混合哈希
            return result;
        }

        private void expand()
        {
            int newsize = MHashHelpers.ExpandPrime(buckets.Length);
            rehash(newsize, forceNewHashCode: false);
        }

        private void rehash()
        {
            rehash(buckets.Length, forceNewHashCode: false);
        }
        private void rehash(int newsize, bool forceNewHashCode)
        {
            occupancy = 0;
            bucket[] newBuckets = new bucket[newsize];

            for (int i = 0; i < buckets.Length; i++)
            {
                bucket bucket = buckets[i];
                //存在key且该位置没有发生过移除时具有哈希冲突(条件2，简单来说就是已经移除过是空的了)
                if (bucket.key != null && bucket.key != buckets)
                {
                    //forceNewHashCode=1---重新获取哈希码
                    //forceNewHashCode=0---清除哈希冲突标记
                    //因为这是在转移新桶，所以原冲突都不作数了
                    int hashcode = (forceNewHashCode ? bucket.key.GetHashCode() : bucket.hash_coll) & 0x7FFFFFFF;
                    putEntry(newBuckets, bucket.key, bucket.val, hashcode);
                }
            }

            buckets = newBuckets;
            loadsize = (int)(loadFactor * (float)newsize);
        }

        private void putEntry(bucket[] newBuckets, object key, object nvalue, int hashcode)
        {
            uint num = 1 + (uint)(hashcode * 101) % (uint)(newBuckets.Length - 1);//indexIncr，为index增量
            int num2 = (int)((uint)hashcode % (uint)newBuckets.Length);//index

            //判断index处是否有key，占用就一直找到没有占用的key
            while (newBuckets[num2].key != null && newBuckets[num2].key != buckets)
            {
                //哈希冲突标记为0时(没有哈希冲突)
                if (newBuckets[num2].hash_coll >= 0)
                {
                    newBuckets[num2].hash_coll |= int.MinValue;//哈希冲突标记置为1(其实发生了哈希冲突)
                    occupancy++;
                }

                //步进找到一个新的index
                num2 = (int)((num2 + num) % (long)(uint)newBuckets.Length);
            }

            //在没有占用的key处存入
            newBuckets[num2].val = nvalue;
            newBuckets[num2].key = key;
            newBuckets[num2].hash_coll |= hashcode;
        }

        private bool KeyEquals(object item, object key)
        {
            //该处为空必然不可能与key相等
            if (buckets == item)
            {
                return false;
            }

            //==比较
            if (item == key)
            {
                return true;
            }

            //Equals()比较
            return item?.Equals(key) ?? false;
        }

        //可以隐藏，使用foreach语句时无影响
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MHashtableEnumerator(this, 3);
        }
    }
}