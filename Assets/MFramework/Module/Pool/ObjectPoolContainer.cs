namespace MFramework.Pool
{
    /// <summary>
    /// 对象池句柄，负责记录实例与占用状态。
    /// </summary>
    public class ObjectPoolContainer<T>
    {
        public ObjectPoolContainer(T item)
        {
            Item = item;
        }

        public T Item { get; }

        public bool Used { get; private set; }

        public void Consume()
        {
            Used = true;
        }

        public void Release()
        {
            Used = false;
        }
    }
}
