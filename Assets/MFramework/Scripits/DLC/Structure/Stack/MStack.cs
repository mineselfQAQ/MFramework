using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MStack : IEnumerable
    {
        private const int _defaultCapacity = 10;//默认总容量

        private object[] _array;//Stack本体

        private int _size;//当前元素个数

        public int Count => _size;//用于公开的当前元素个数

        public MStack()
        {
            _array = new object[_defaultCapacity];
            _size = 0;
        }

        public MStack(int capacity)
        {
            if (capacity < 0)
            {
                throw new Exception();
            }

            if (capacity < 10)
            {
                capacity = 10;
            }

            _array = new object[capacity];
            _size = 0;
        }

        public bool IsEmpty => _size == 0;

        public void Push(object o)
        {
            //元素已满，需要扩容
            if (_size == _array.Length)
            {
                object[] temp = new object[_array.Length * 2];
                Array.Copy(_array, 0, temp, 0, _size);
                _array = temp;
            }

            _array[_size] = o;
            _size++;
        }

        public object Pop()
        {
            //没有元素不能弹出
            if (IsEmpty)
            {
                throw new Exception();
            }

            _size--;
            object result = _array[_size];
            _array[_size] = null;//将最后一个元素置为空(可以不用，因为访问不到)
            return result;
        }

        public object Peek()
        {
            //没有元素不能查看
            if (IsEmpty)
            {
                throw new Exception();
            }

            return _array[_size - 1];
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
        }

        public bool Contains(object o)
        {
            int size = _size;
            //这样思考：
            //当size=0的时候while循环退出，这是对的
            //当size=1时，内部访问的是_array[0]，这就是最后一个元素，这也是对的
            while (size-- > 0)
            {
                if (o == null)
                {
                    if (_array[size] == null)
                    {
                        return true;
                    }
                }
                else if (_array[size] != null && _array[size].Equals(o))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = _size - 1; i >= 0; i--)
            {
                yield return _array[i];
            }
        }
    }
    public static class MStackExtension
    {
        public static void Print(this MStack stack)
        {
            MLog.Print("输出: ");

            if (stack.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            string outputStr = "";
            foreach (var item in stack)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}