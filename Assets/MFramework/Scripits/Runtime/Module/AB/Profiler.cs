using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MFramework
{
    /// <summary>
    /// 计时类，记录整体以及每个操作所使用时间
    /// </summary>
    public class Profiler
    {
        private static readonly Stopwatch ms_Stopwatch = Stopwatch.StartNew();//全局计时器
        private static readonly StringBuilder ms_StringBuilder = new StringBuilder();//输出字符串
        private static readonly List<Profiler> ms_Stack = new List<Profiler>();//遍历容器

        private List<Profiler> m_children;//子Profiler(组合模式)
        private string m_Name;//名字
        private int m_Level;//层级
        private long m_Timestamp;//时间戳
        private long m_Time;//总时间(开始到结束的时间戳间隔)

        public Profiler(string name)
        {
            m_children = null;
            m_Name = name;
            m_Level = 0;
            m_Timestamp = -1;
            m_Time = 0;
        }

        private Profiler(string name, int level) : this(name)
        {
            m_Level = level;
        }

        public static Profiler Create(string name)
        {
            return new Profiler(name);
        }

        public Profiler CreateChild(string name)
        {
            if (m_children == null)
            {
                m_children = new List<Profiler>();
            }

            Profiler profiler = new Profiler(name, m_Level + 1);
            m_children.Add(profiler);
            return profiler;
        }

        public void Reset()
        {
            m_Timestamp = -1;
            m_Time = 0;

            if (m_children == null)
            {
                return;
            }

            for (int i = 0; i < m_children.Count; ++i)
            {
                m_children[i].Reset();
            }
        }

        public void Start()
        {
            if (m_Timestamp != -1)
            {
                MLog.Print($"{nameof(Profiler)}.{nameof(Start)}：{m_Name}重复开始，请检查", MLogType.Warning);
            }

            m_Timestamp = ms_Stopwatch.ElapsedTicks;
        }

        public void Restart()
        {
            Reset();
            Start();
        }

        public void Stop()
        {
            if (m_Timestamp == -1)
            {
                MLog.Print($"{nameof(Profiler)}.{nameof(Stop)}：{m_Name}重复结束，请检查", MLogType.Warning);
            }

            m_Time += ms_Stopwatch.ElapsedTicks - m_Timestamp;
            m_Timestamp = -1;
        }

        private void Format()
        {
            ms_StringBuilder.AppendLine();

            for (int i = 0; i < m_Level; ++i)
            {
                //1层：|--
                //2层：|  |--
                //3层：|  |  |--
                ms_StringBuilder.Append(i < m_Level - 1 ? "|  " : "|--");
            }
            
            ms_StringBuilder.Append(m_Name);
            
            //输出信息(时间)
            string totalMillisecond = $"{(float)m_Time / TimeSpan.TicksPerMillisecond:F2}";
            string totalSecond = $"{(float)m_Time / TimeSpan.TicksPerSecond:F2}";
            string totalMinute = $"{(float)m_Time / TimeSpan.TicksPerMinute:F4}";
            ms_StringBuilder.Append
                ($" [Time：{totalMinute}分 {totalSecond}秒 {totalMillisecond}毫秒]");
        }

        /// <summary>
        /// BFS输出Profiler信息
        /// </summary>
        public override string ToString()
        {
            ms_StringBuilder.Clear();
            ms_Stack.Clear();
            ms_Stack.Add(this);

            while (ms_Stack.Count > 0)
            {
                int index = ms_Stack.Count - 1;
                Profiler profiler = ms_Stack[index];
                ms_Stack.RemoveAt(index);

                profiler.Format();//组合ms_StringBuilder

                List<Profiler> children = profiler.m_children;
                if (children == null) continue;
                for (int i = children.Count - 1; i >= 0; --i)
                {
                    ms_Stack.Add(children[i]);
                }
            }

            return ms_StringBuilder.ToString();//输出ms_StringBuilder
        }
    }
}