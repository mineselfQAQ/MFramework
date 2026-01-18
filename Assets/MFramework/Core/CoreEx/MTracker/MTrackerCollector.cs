using System.Collections.Generic;
using System.Text;

namespace MFramework.Core
{
    public class TrackerCollector : ITrackerCollector
    {
        private readonly List<string> _logs = new List<string>();

        public void Collect(string log)
        {
            _logs.Add(log);
        }

        public string GetLog()
        {
            return ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int len = _logs.Count;
            for (int i = 0; i < len; i++)
            {
                if (i != len - 1)
                {
                    sb.Append(_logs[i] + "\n");
                }
                else
                {
                    sb.Append(_logs[i]);
                }
            }
            return sb.ToString();
        }
    }
    
    public interface ITrackerCollector
    {
        void Collect(string log);

        string GetLog();
    }
}