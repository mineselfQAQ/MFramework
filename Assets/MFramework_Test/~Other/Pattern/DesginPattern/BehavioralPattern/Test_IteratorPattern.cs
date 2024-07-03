using MFramework;
using UnityEngine;

public class Test_IteratorPattern : MonoBehaviour
{
    private void Start()
    {
        NameRepository nameRepository = new NameRepository();
        for (Iterator iterator = nameRepository.GetIterator(); iterator.HasNext();)
        {
            MLog.Print(iterator.Next().ToString());
        }
    }

    public class NameRepository : Container
    {
        public string[] names = { "A", "E", "B", "D", "C" };

        public Iterator GetIterator()
        {
            return new NameIterator(names);
        }

        private class NameIterator : Iterator
        {
            private int index;

            private string[] names;

            public NameIterator(string[] names)
            {
                this.names = names;
            }

            public bool HasNext()
            {
                if (index < names.Length)
                {
                    return true;
                }
                return false;
            }

            public object Next()
            {
                if (this.HasNext())
                {
                    return names[index++];
                }
                return null;
            }
        }
    }

    public interface Iterator
    {
        public bool HasNext();
        public object Next();
    }
    public interface Container
    {
        public Iterator GetIterator();
    }
}
