using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MArrayList : IEnumerable
    {
        //默认初始容量
        private const int _defaultCapacity = 4;

        //用来存放的数组
        private object[] _items;
        //当前元素个数(内部使用)
        private int _size;

        //容量(最大可存放数量)
        public int Capacity
        {
            get
            {
                return _items.Length;//通过_items来确定容量
            }
            set
            {
                //错误情况---输入的值小于当前元素个数
                //这意味着现在假如有5个元素，将Capacity设为3，
                //那么就得删除两个存在的元素，这是不合理的
                if (value < _size)
                {
                    throw new Exception("Capacity：输入的值小于当前元素个数");
                }

                //排除情况---容量与最大容量相同，那么根本不需要修改
                if (value == _items.Length)
                {
                    return;
                }

                //一般情况---输入值大于0，
                //由于上方排除了value < _size，
                //所以综合起来这里只有value >= _size，也就是输入的值大于当前元素个数，
                //此时排除了等于最大容量的情况，
                //那么还会有小于最大容量(缩容)与大于最大容量(扩容)的情况
                if (value > 0)
                {
                    //创建并迁移到新数组中
                    object[] array = new object[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, 0, array, 0, _size);
                    }
                    _items = array;
                }
                else//一般来说应该不会进入这里，以防万一这里创建了一个初始数组
                {
                    _items = new object[4];
                }
            }
        }

        //当前元素个数(公开出来的，等价于_size)
        public int Count => _size;

        //this索引器
        public object this[int index]
        {
            get
            {
                //错误情况
                if (index < 0 || index >= _size)
                {
                    throw new Exception("this索引器：index超出范围");
                }

                return _items[index];
            }
            set
            {
                //错误情况
                if (index < 0 || index >= _size)
                {
                    throw new Exception("this索引器：index超出范围");
                }

                _items[index] = value;
            }
        }

        //无参构造函数
        public MArrayList()
        {
            _items = new object[0];
        }
        //有参构造函数(传入容量)
        public MArrayList(int capacity)
        {
            //错误情况
            if (capacity < 0)
            {
                throw new Exception("构造函数：传入容量小于0");
            }

            if (capacity == 0)//等价于无参构造函数
            {
                _items = new object[0];
            }
            else//一般情况
            {
                _items = new object[capacity];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                yield return _items[i];
            }
        }

        //*************
        //***方法部分***
        //*************
        private void EnsureCapacity()
        {
            //当元素满的情况下发生扩容操作
            if (_size == _items.Length)
            {
                //扩容操作，一般来说就是翻倍，只有当容量为0也就是没有存放内容的时候会先进行初始化(4个空间)
                //要注意的是：
                //这里仅仅是创建了一个int数，
                //因为扩容不是直接就能进行的，需要通过创建新数组进行创建，这就需要通过Capacity的Set方法完成
                int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));

                //限制数组上限
                if ((uint)num > 2146435071u)
                {
                    num = 2146435071;
                }

                //真正的扩容操作
                Capacity = num;
            }
        }

        public int Add(object value)
        {
            //特殊情况，当当前元素个数等于总容量的时候，
            //此时再进行Add其实就需要进行扩容操作，也就是EnsureCapacity()了
            if (_size == _items.Length)
            {
                EnsureCapacity();
            }

            //正常来说，就是很普通的添加一个元素，
            //那么会在下标为_size的位置添加该元素
            _items[_size] = value;

            return _size++;//注意元素个数增加了
        }

        public void Insert(int index, object obj)
        {
            //错误情况
            if (index < 0 || index >= _size)
            {
                throw new Exception("Insert()：index超出范围");
            }

            //与Add()相同，既然是插入，那么就有可能会超出上限
            if (_size == _items.Length)
            {
                EnsureCapacity();
            }

            //正常情况下，只需要将index后面的元素向后移即可
            //在此之前，使用了EnsureCapacity()确保有空间进行操作
            Array.Copy(_items, index, _items, index + 1, _size - index);

            //在index位置插入
            _items[index] = obj;
            _size++;//注意元素个数增加了
        }

        public void RemoveAt(int index)
        {
            //错误情况
            if (index < 0 || index >= _size)
            {
                throw new Exception("RemoveAt()：index超出范围");
            }

            _size--;
            //此处的if是有作用的，
            //该操作限制的是移除最后一个元素的情况，此时index=_size，
            //而_size已经向前移动一位，最后一个元素被"移除"了
            if (index < _size)
            {
                //其实就是Insert()的反向，这里复制数组进行缩容操作
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }

            //将最后一个元素设为null
            _items[_size] = null;
        }

        public void Clear()
        {
            //只要数组中还存在元素，就需要进行全部清空
            if (_size > 0)
            {
                //所有元素置空，但此时不算完全情况
                Array.Clear(_items, 0, _size);
                _size = 0;//只要_size为0，就是真正意义上的清空
            }
        }

        public bool Contains(object obj)
        {
            //特殊情况---需要搜寻的值为null
            if (obj == null)
            {
                //遍历整个数组，查看是否有元素为null
                for (int i = 0; i < _size; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }

            //一般情况
            for (int j = 0; j < _size; j++)
            {
                //遍历整个数组，
                //先看看该元素是否为null(应该是为了节省性能，为null就不用继续判断了)
                //如果满足，就真正判断两者是否相等
                if (_items[j] != null && _items[j].Equals(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(object value)
        {
            //直接使用Array中的IndexOf()进行取索引
            return Array.IndexOf((Array)_items, value, 0, _size);
        }

        public void Reverse()
        {
            //正常来说会翻转全部
            Reverse(0, Count);
        }
        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                throw new Exception("Reverse()：index必定不小于0");
            }
            if (count < 0)
            {
                //虽然0也是不太正确的，但是至少是可行的
                throw new Exception("Reverse()：count必定不小于0");
            }

            if (_size - index < count)
            {
                //这种情况是数量超过范围了，比如说：
                //当前元素有9个，此时设置的index=8,count=2，也就意味着想要从索引为8的位置和后一个位置进行翻转
                //但是其实此时已经超过元素个数了
                throw new Exception("Reverse()：数量不正确");
            }

            //直接使用Array中的Reverse()进行翻转
            Array.Reverse(_items, index, count);
        }

        public void Sort()
        {
            //正常来说会排序所有
            Sort(0, Count, Comparer.Default);
        }
        public void Sort(int index, int count, IComparer comparer)
        {
            if (index < 0)
            {
                throw new Exception("Sort()：index必定不小于0");
            }
            if (count < 0)
            {
                //虽然0也是不太正确的，但是至少是可行的
                throw new Exception("Sort()：count必定不小于0");
            }

            if (_size - index < count)
            {
                //同Reverse()
                throw new Exception("Sort()：数量不正确");
            }

            //直接使用Array中的Sort()进行翻转
            Array.Sort(_items, index, count, comparer);
        }

        public void TrimToSize()
        {
            //只要将容量设为当前元素个数，其实就是裁剪操作
            Capacity = _size;
        }
    }

    public static class MArrayListExtension
    {
        public static void Print(this MArrayList list)
        {
            MLog.Print("输出: ");

            if (list.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            string outputStr = "";
            foreach (var item in list)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}