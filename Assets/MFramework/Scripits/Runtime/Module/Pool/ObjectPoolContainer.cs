namespace MFramework
{
    public class ObjectPoolContainer<T>
    {
        private T item;//实际存放物体
        public T Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        public bool Used { get; private set; }

        /// <summary>
        /// 将物体置为"不可使用状态(也就是Used)"
        /// </summary>
        public void Consume()
        {
            Used = true;
        }

        /// <summary>
        /// 将物体置为"可使用状态(也就是Not Used)"
        /// </summary>
        public void Release()
        {
            Used = false;
        }
    }
}