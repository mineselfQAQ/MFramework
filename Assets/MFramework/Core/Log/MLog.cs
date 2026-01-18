using System;
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
        public enum MLogLevel : byte
        {
            /// <summary>
            /// 不记录
            /// </summary>
            Off,
            
            /// <summary>
            /// 记录：Error
            /// </summary>
            Error,
            
            /// <summary>
            /// 记录：Error Warning
            /// </summary>
            Warning,
            
            /// <summary>
            /// 记录：Error Warning Debug
            /// </summary>
            Debug,
        }

        public static ILog Default;
        
        private static MLogLevel? _logLevel;
        
        private static FileStream _stream;
        private static StreamWriter _writer;
        
        private static bool _isInitialized;
        private static LogType? _lastLogType;
        
        private const string FILE_NAME = "log.txt";
        
        public static void Bootstrap()
        {
            _logLevel ??= MLogLevel.Debug; // 默认全开
            Default = new UserLog("Default");
            
            string path = GetRootPath();
            _stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(_stream, Encoding.UTF8);
            _writer.WriteLine("==================================");
            _writer.WriteLine($"===日志开始[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]===");
            _writer.WriteLine("==================================");
            
            // 日志回调
#if UNITY_EDITOR
            Application.logMessageReceived += OnLogCallBack;
#endif
        }

        public static void Shutdown()
        {
            // 日志回调
#if UNITY_EDITOR
            Application.logMessageReceived -= OnLogCallBack;
#endif
            // TODO：如何在Exception报错时输出这段
            _writer.WriteLine("==================================");
            _writer.WriteLine($"===日志结束[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]===");
            _writer.WriteLine("==================================");
            _writer.Flush();
            _writer.Close();
            _writer = null;
            _stream = null;
        }
        
        /// <summary>
        /// 设置默认Log等级(如果不传Log等级则会使用)
        /// <para>如需调用，请在OnBootstrapping事件调用</para>
        /// </summary>
        public static void SetDefaultLogLevel(MLogLevel logLevel) => _logLevel = logLevel;

        public static MLogLevel? GetDefaultLogLevel() => _logLevel;

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
                // 报错用Unity流程
                _writer.WriteLine($"[{time}] {logString}\n{stackTrace.TrimEnd()}");
            }
            else
            {
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
