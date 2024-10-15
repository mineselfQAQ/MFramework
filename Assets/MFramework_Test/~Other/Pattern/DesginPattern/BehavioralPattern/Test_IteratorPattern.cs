using MFramework;
using System;
using System.Collections;
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

        Student student = new Student();
        student.AddStudent("1");
        student.AddStudent("A");
        student.AddStudent("E");
        student.AddStudent("G");
        foreach (var s in student)
        {
            MLog.Print(s);
        }

        var e = student.GetEnumerator();
        e.Reset();
        while (e.MoveNext())
        {
            string item = (string)e.Current;
            MLog.Print(item);
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





    internal class Student : IEnumerable
    {
        private static string[] ms_Students = new string[100];

        private static int ms_Size = 0;

        public void AddStudent(string name)
        {
            ms_Students[ms_Size] = name;
            ms_Size++;
        }

        public IEnumerator GetEnumerator()
        {
            return new StudentEnumerator(ms_Students);
        }
    }

    internal class StudentEnumerator : IEnumerator
    {
        private string[] m_student;
        private int m_position = -1;

        public StudentEnumerator(string[] student)
        {
            m_student = student;
        }

        public object Current
        {
            get
            {
                if (m_position == -1)
                {
                    throw new Exception();
                }
                if (m_position >= m_student.Length)
                {
                    throw new Exception();
                }

                return m_student[m_position];
            }
        }

        public bool MoveNext()
        {
            //关键：
            //由于使用的是string[]，所以还需要辨别当前存放了几个string
            if (m_position < m_student.Length - 1 && m_student[m_position + 1] != null)
            {
                m_position++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            m_position = -1;
        }
    }
}
