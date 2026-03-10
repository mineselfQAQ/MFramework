using System.Runtime.CompilerServices;
using MFramework.Core.Internal;
using UnityEngine;

namespace MFramework.Core
{
    public abstract class LogBase : ILog
    {
        protected abstract string SrcName { get; }
        
        private readonly MLog.LogFilter? _logFilter;
        private readonly string _name; // 无用，已通过[CallerFilePath]特性获取文件名，此处仅收集供ToString()使用

        private MLog.LogFilter GlobalLogFilter => MLog.GetDefaultLogFilter();
        
        protected LogBase(string name)
        {
            _name = name;
        }
        
        protected LogBase(string name, MLog.LogFilter logFilter)
        {
            _name = name;
            _logFilter = logFilter;
        }

        public override string ToString() => _name;

        public void D(object message, 
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            DInternal(message, BuildCallerLocation(file, line, member));
        }
        
        public void W(object message, 
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            WInternal(message, BuildCallerLocation(file, line, member));
        }
        
        public void E(object message, 
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            EInternal(message, BuildCallerLocation(file, line, member));
        }
        
        public void EX(LogException ex, MLog.LogLevel? overrideLevel = null,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            var callerLocation = BuildCallerLocation(file, line, member);
            var info = ExLogCache.Get(ex);
            
            var level = overrideLevel ?? info.Level;
            
            switch (level)
            {
                case MLog.LogLevel.Warning:
                    WInternal(info.Message, callerLocation);
                    break;
                case MLog.LogLevel.Error:
                    EInternal(info.Message, callerLocation);
                    break;
                default:
                    DInternal(info.Message, callerLocation);
                    break;
            }
        }
        
        protected virtual void DInternal(object message, string callerLocation)
        {
            if (GetLogFilter() > MLog.LogFilter.Debug) return;

            string info = LogFormatter.Build(MLog.LogLevel.Debug, SrcName, callerLocation, message);
            
            Debug.Log(info);
        }

        protected virtual void WInternal(object message, string callerLocation)
        {
            if (GetLogFilter() > MLog.LogFilter.Warning) return;
            
            string info = LogFormatter.Build(MLog.LogLevel.Warning, SrcName, callerLocation, message);
            
            Debug.LogWarning($"{info} {message}");
        }

        protected virtual void EInternal(object message, string callerLocation)
        {
            if (GetLogFilter() > MLog.LogFilter.Error) return;
            
            string info = LogFormatter.Build(MLog.LogLevel.Error, SrcName, callerLocation, message);
            
            string stackTrace = GetStackTrace(3);
            if (stackTrace != null)
            {
                Debug.LogError($"{info} {message}\n{stackTrace}");
            }
            else
            {
                Debug.LogError($"{info} {message}");
            }
        }

        

        private MLog.LogFilter GetLogFilter()
        {
            if (_logFilter != null) return _logFilter.Value;
            return GlobalLogFilter;
        }

        private string GetSource() => $"[{SrcName}]";
        
        private string GetStackTrace(int skipFrames)
        {
#if !UNITY_EDITOR
            var stackTrace = new System.Diagnostics.StackTrace(skipFrames, true);
            return stackTrace.ToString();
#else
            return null;
#endif
        }
        
        private static string BuildCallerLocation(string file, int line, string member)
        {
            var location = IntUtilEx.GetCallerLocation(file, line, member);
            return location.ToString();
        }
    }
}
