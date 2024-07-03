using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MQueue : IEnumerable
    {
        private object[] _array;//循环数组，通过_head/_tail实现

        private int _head;//一开始在0处，删除元素后+1
        private int _tail;//在_size处(末元素位置的后一个位置)

        private int _size;

        private int _growFactor;

        public int Count => _size;

        public MQueue() : this(32, 2.0f) { }
        public MQueue(int capacity) : this(capacity, 2.0f) { }
        public MQueue(int capacity, float growFactor)
        {
            //容量不可能小于0
            if (capacity < 0)
            {
                throw new Exception();
            }

            //限制增长因子GrowFactor范围---[1,10]
            if (!((double)growFactor >= 1.0) || !((double)growFactor <= 10.0))
            {
                throw new Exception();
            }

            _array = new object[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
            _growFactor = (int)(growFactor * 100f);//本质上是保留2位小数，2.11得211，2.1111得211

        }

        private void SetCapacity(int capacity)
        {
            object[] temp = new object[capacity];

            //有元素就搬迁
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, temp, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, temp, 0, _array.Length - _head);
                    Array.Copy(_array, 0, temp, _array.Length - _head, _tail);
                }
            }

            _array = temp;
            //搬迁完后同时进行了整理，头指针和尾指针的位置重置了
            _head = 0;
            _tail = ((_size != capacity) ? _size : 0);//元素满了会指向首元素(下一个可以存放的位置就是0)
        }

        public void Enqueue(object o)
        {
            //数组需要扩容
            if (_size == _array.Length)
            {
                //扩容大小---数组原长度*构造函数中的growFactor
                //注意：运算顺序为左到右，不会/100后先向下取整，所以精度会更高一点
                //注意：先long再int是需要的，因为可能发生_array.Length*_growFactor正好比int大一点，这样精度不会损失
                int num = (int)((long)_array.Length * (long)_growFactor / 100);
                //特殊情况：扩容量连4都没有，那么就使用最低限度扩容4个元素
                if (num < _array.Length + 4)
                {
                    num = _array.Length + 4;
                }

                SetCapacity(num);
            }

            _array[_tail] = o;
            _tail = (_tail + 1) % _array.Length;//循环数组
            _size++;
        }

        public object Dequeue()
        {
            //没有元素不能出队
            if (_size == 0)
            {
                throw new Exception();
            }

            //FIFO，取出头指针的元素
            object result = _array[_head];
            //后处理
            _array[_head] = null;
            _head = (_head + 1) % _array.Length;//头指针后移(更改首元素)
            _size--;

            return result;
        }

        public object Peek()
        {
            //没有元素不能查看
            if (_size == 0)
            {
                throw new Exception();
            }

            return _array[_head];
        }

        public void Clear()
        {
            if (_head < _tail)//正常情况，如：null 1(头) 2 3 4 5 null(尾)
            {
                Array.Clear(_array, _head, _size);
            }
            else//颠倒情况，如：4 5 null(尾) null 1(头) 2 3
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _size = 0;
        }


        public bool Contains(object o)
        {
            int num = _head;//检查的对象(从队首开始)
            int size = _size;//需要检查的个数

            while (size-- > 0)
            {
                if (o == null)
                {
                    if (_array[num] == null)
                    {
                        return true;
                    }
                }
                else if (_array[num] != null && _array[num].Equals(o))
                {
                    return true;
                }

                num = (num + 1) % _array.Length;//检查下一个对象
            }
            return false;
        }

        /// <summary>
        /// 整理数组(第一个元素归位至0)
        /// </summary>
        public void TrimToSize()
        {
            SetCapacity(_size);
        }

        public IEnumerator GetEnumerator()
        {
            int num = _head;
            int size = _size;

            while (size-- > 0)
            {
                yield return _array[num];

                num = (num + 1) % _array.Length;
            }
        }
    }

    public static class MQueueExtension
    {
        public static void Print(this MQueue queue)
        {
            MLog.Print("输出: ");

            if (queue.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            string outputStr = "";
            foreach (var item in queue)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}