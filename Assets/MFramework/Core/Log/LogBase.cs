using MFramework.Core.Internal;
using UnityEngine;

namespace MFramework.Core
{
    public abstract class LogBase : ILog
    {
        private const string DEFAULT_LEVEL_NAME = "[DEFAULT]";
        private const string DEBUG_LEVEL_NAME = "[DEBUG]";
        private const string WARNING_LEVEL_NAME = "[WARNING]";
        private const string ERROR_LEVEL_NAME = "[ERROR]";
        
        protected abstract string SrcName { get; }
        
        private readonly MLog.MLogLevel? _logLevel;
        private readonly string _name;
        
        protected LogBase(string name)
        {
            _name = name;
            _logLevel = MLog.GetDefaultLogLevel();
        }
        
        protected LogBase(string name, MLog.MLogLevel logLevel)
        {
            _name = name;
            _logLevel = logLevel;
        }

        private string GetPrefix(MLog.MLogLevel logLevel)
        {
            string logLevelName;
            if(logLevel == MLog.MLogLevel.Debug) logLevelName = DEBUG_LEVEL_NAME;
            else if (logLevel == MLog.MLogLevel.Warning) logLevelName = WARNING_LEVEL_NAME;
            else if (logLevel == MLog.MLogLevel.Error) logLevelName = ERROR_LEVEL_NAME;
            else logLevelName = DEFAULT_LEVEL_NAME;
#if UNITY_EDITOR
            Color color;
            if(logLevel == MLog.MLogLevel.Debug) color = Color.green;
            else if(logLevel == MLog.MLogLevel.Warning) color = Color.yellow;
            else if(logLevel == MLog.MLogLevel.Error) color = Color.red;
            else color = Color.white;
            
            logLevelName = IntUtil.Col(logLevelName, color);
#endif
            
            return $"{logLevelName}[{SrcName}][{_name}]";
        }

        private string GetStackTrace()
        {
#if !UNITY_EDITOR
            var stackTrace = new System.Diagnostics.StackTrace(2, true);
            return stackTrace.ToString();
#else
            return null;
#endif
        }
        
        public void D(object message)
        {
            if (_logLevel < MLog.MLogLevel.Debug) return;

            string prefix = GetPrefix(MLog.MLogLevel.Debug);
            Debug.Log($"{prefix} {message}");
        }
        
        public void W(object message)
        {
            if (_logLevel < MLog.MLogLevel.Warning) return;
            
            string prefix = GetPrefix(MLog.MLogLevel.Warning);
            Debug.LogWarning($"{prefix} {message}");
        }
        
        public void E(object message)
        {
            if (_logLevel < MLog.MLogLevel.Error) return;
            
            string prefix = GetPrefix(MLog.MLogLevel.Error);
            string stackTrace = GetStackTrace();
            if (stackTrace != null)
            {
                Debug.LogError($"{prefix} {message}\n{stackTrace}");
            }
            else
            {
                Debug.LogError($"{prefix} {message}");
            }
        }
    }
}