using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MFramework.Core
{
    /// <summary>
    /// Log系统(控制台输出+输出日志)
    /// </summary>
    public static class MLog
    {
        public enum LogFilter : byte
        {
            /// <summary>
            /// 记录：Error Warning Debug
            /// </summary>
            Debug, // 默认值
            
            /// <summary>
            /// 记录：Error Warning
            /// </summary>
            Warning,
            
            /// <summary>
            /// 记录：Error
            /// </summary>
            Error,
            
            /// <summary>
            /// 不记录
            /// </summary>
            Off,
        }
        
        
        public enum LogLevel : byte
        {
            Error, // 默认值
            Warning,
            Debug,
        }
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal const LogFilter BUILD_FILTER = LogFilter.Debug;
#else
        internal const LogFilter BUILD_FILTER = LogFilter.Off;
#endif

        public static ILog Default;
        private static ILog _selfLog;
        
        private static readonly Dictionary<string, UserLog> _ULogDic = new Dictionary<string, UserLog>();
        
        private static LogFilter _logFilter;
        
        private static FileStream _stream;
        private static StreamWriter _writer;
        
        private static LogType? _lastLogType;
        
        private const string FILE_NAME = "log.txt";
        
        public static void Bootstrap()
        {
            Default = new UserLog("Default");
            _selfLog = new InternalLog(nameof(MLog));
            
            _selfLog.D("Log模块：开启");
            
#if !UNITY_EDITOR
            // TODO：这里的文件读写需要改进吗？
            string path = GetRootPath();
            _stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(_stream, Encoding.UTF8);
            _writer.WriteLine("==================================");
            _writer.WriteLine($"===日志开始[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]===");
            _writer.WriteLine("==================================");
            
            // 日志回调
            Application.logMessageReceived += OnLogCallBack;
#endif
        }

        public static void Shutdown(MFrameworkCore core)
        {
            // 日志回调
#if !UNITY_EDITOR
            Application.logMessageReceived -= OnLogCallBack;
            _writer.WriteLine("==================================");
            _writer.WriteLine($"===日志结束[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]===");
            _writer.WriteLine("==================================");
            string log = core.GetLog();
            _writer.WriteLine(core.GetLog());
            _selfLog.D($"程序退出  {typeof(TrackerCollector)}统计情况：\n{log}");
            _writer.Flush();
            _writer.Close();
            _writer = null;
            _stream = null;
#endif
        }
        
        public static UserLog Create<T>()
        {
            string name = typeof(T).FullName;
            return GetOrCreateULog(name);
        }
        
        public static UserLog Create<T>(LogFilter filter)
        {
            string name = typeof(T).FullName;
            return GetOrCreateULog(name, filter);
        }
        
        private static UserLog GetOrCreateULog(string name, LogFilter? filter = null)
        {
            if (_ULogDic.TryGetValue(name, out UserLog log))
            {
                return log;
            }

            if (filter == null) _ULogDic.Add(name, new UserLog(name));
            else _ULogDic.Add(name, new UserLog(name, filter.Value));
            
            return _ULogDic[name];
        }

        // 仅应该调用一次
        internal static void SetDefaultLogFilter(LogFilter logFilter) => _logFilter = logFilter;

        public static LogFilter GetDefaultLogFilter() => _logFilter;

        private static void OnLogCallBack(string logString, string stackTrace, LogType type)
        {
            string time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            
            bool isError = type == LogType.Exception || type == LogType.Error || type == LogType.Assert;
            bool lastIsError = _lastLogType == LogType.Exception || 
                                 _lastLogType == LogType.Error || 
                                 _lastLogType == LogType.Assert;

            if (isError && !lastIsError) _writer.WriteLine();
            if (type == LogType.Exception)
            {
                // Exception用Unity流程(自己throw/UnityAPI)
                _writer.WriteLine($"[{time}] {logString}\n{stackTrace.TrimEnd()}");
            }
            else
            {
                // ScriptOnly无法获取到stackTrace，所以实现在logString中
                _writer.WriteLine($"[{time}] {logString}");
            }
            if (isError) _writer.WriteLine();

            _lastLogType = type;
            
            _writer.Flush();
            _stream.Flush(true);
        }

        private static string GetRootPath()
        {
            // TODO：仅测试了PC，Ios和Android应该不能用
            return $"{MPathCache.PC_ROOT_PATH}/{FILE_NAME}";
        }
        
        #if UNITY_EDITOR
            [UnityEditor.Callbacks.OnOpenAsset(0)]
            public static bool OnOpenAsset(int instanceID, int line)
            {
                string stackTrace = GetStackTrace();
                string fileName = nameof(LogBase);
                
                if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains($"{fileName}.cs"))
                {
                    var matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                    string pathLine = "";
                    while (matches.Success)
                    {
                        pathLine = matches.Groups[1].Value;

                        if (!pathLine.Contains($"{fileName}.cs"))
                        {
                            int splitIndex = pathLine.LastIndexOf(":");
                            string path = pathLine.Substring(0, splitIndex);
                            line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                            string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                            fullPath = fullPath + path;
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                            break;
                        }
                        matches = matches.NextMatch();
                    }
                    return true;
                }
                return false;
            }

            private static string GetStackTrace()
            {
                System.Type ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
                FieldInfo fieldInfo = ConsoleWindowType.GetField
                    ("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
                object consoleInstance = fieldInfo.GetValue(null);
                if (consoleInstance != null)
                {
                    if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
                    {
                        fieldInfo = ConsoleWindowType.GetField
                            ("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                        string activeText = fieldInfo.GetValue(consoleInstance).ToString();

                        return activeText;
                    }
                }
                return null;
            }
    #endif
    }
}
