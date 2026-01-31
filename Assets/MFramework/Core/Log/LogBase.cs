using System;
using System.Runtime.CompilerServices;
using MFramework.Core.Internal;
using UnityEngine;

namespace MFramework.Core
{
    public abstract class LogBase : ILog
    {
        private const string DEBUG_LEVEL_NAME = "D";
        private const string WARNING_LEVEL_NAME = "W";
        private const string ERROR_LEVEL_NAME = "E";
        
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
            DInternal(message, CallerLocation.From(file, line, member));
        }
        
        public void W(object message, 
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            WInternal(message, CallerLocation.From(file, line, member));
        }
        
        public void E(object message, 
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            EInternal(message, CallerLocation.From(file, line, member));
        }
        
        public void EX(LogException ex, MLog.LogLevel? overrideLevel = null,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            var location = CallerLocation.From(file, line, member);
            var info = ExLogCache.Get(ex);
            
            var level = overrideLevel ?? info.Level;
            
            switch (level)
            {
                case MLog.LogLevel.Warning:
                    WInternal(info.Message, location);
                    break;
                case MLog.LogLevel.Error:
                    EInternal(info.Message, location);
                    break;
                default:
                    DInternal(info.Message, location);
                    break;
            }
        }
        
        private void DInternal(object message, CallerLocation location)
        {
            if (GetLogFilter() > MLog.LogFilter.Debug) return;

            string info = GetLogInfo(MLog.LogLevel.Debug, location);
            
            Debug.Log($"{info} {message}");
        }

        private void WInternal(object message, CallerLocation location)
        {
            if (GetLogFilter() > MLog.LogFilter.Warning) return;
            
            string info = GetLogInfo(MLog.LogLevel.Warning, location);
            
            Debug.LogWarning($"{info} {message}");
        }

        private void EInternal(object message, CallerLocation location)
        {
            if (GetLogFilter() > MLog.LogFilter.Error) return;
            
            string info = GetLogInfo(MLog.LogLevel.Error, location);
            
            string stackTrace = GetStackTrace(3);
            if (stackTrace != null)
            {
                Debug.LogError($"{info} {message}\n{stackTrace}");
            }
            else
            {
                Debug.LogError($"{info} {message}\n未获取到堆栈信息.");
            }
        }



        private MLog.LogFilter GetLogFilter()
        {
            if (_logFilter != null) return _logFilter.Value;
            return GlobalLogFilter;
        }
        
        private string GetLogInfo(MLog.LogLevel logLevel, CallerLocation callerLocation)
        {
            string level = GetLogLevel(logLevel);
            string source = GetSource();
            string location = GetCallerLocation(callerLocation);

            return $"{level}{source}{location}";
        }
        
        private string GetLogLevel(MLog.LogLevel logLevel)
        {
            string logLevelName;
            if (logLevel == MLog.LogLevel.Warning) logLevelName = WARNING_LEVEL_NAME;
            else if (logLevel == MLog.LogLevel.Error) logLevelName = ERROR_LEVEL_NAME;
            else logLevelName = DEBUG_LEVEL_NAME;
            
            logLevelName = $"[{logLevelName}]";
            
#if UNITY_EDITOR
            Color color;
            if(logLevel == MLog.LogLevel.Warning) color = Color.yellow;
            else if(logLevel == MLog.LogLevel.Error) color = Color.red;
            else color = Color.green;
            
            logLevelName = IntUtil.Col(logLevelName, color);
#endif
            
            return logLevelName;
        }

        private string GetSource() => $"[{SrcName}]";

        private string GetCallerLocation(CallerLocation location) => $"[{location}]";
        
        private string GetStackTrace(int skipFrames)
        {
#if !UNITY_EDITOR
            var stackTrace = new System.Diagnostics.StackTrace(skipFrames, true);
            return stackTrace.ToString();
#else
            return null;
#endif
        }
    }
}