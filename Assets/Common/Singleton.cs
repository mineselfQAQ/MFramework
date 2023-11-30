namespace MFramework
{
    public abstract class Singleton<T> where T : class, new()
    {
        //与abstract功能相似
        //protected Singleton() { }

        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        public static void Clear()
        {
            instance = null;
        }
    }
}
