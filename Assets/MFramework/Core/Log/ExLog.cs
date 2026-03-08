using System;
using System.Collections.Generic;
using System.Reflection;
using LogLevel = MFramework.Core.MLog.LogLevel;

namespace MFramework.Core
{
    public enum LogException
    {
        // ===常见Ex===
        [ExLog(MLog.LogLevel.Warning, "参数为空")]
        NullArgument,
        
        [ExLog(MLog.LogLevel.Error, "参数不合法")]
        InvalidArgument,
        
        [ExLog(MLog.LogLevel.Error, "索引越界")]
        IndexOutOfRange,
        
        [ExLog(MLog.LogLevel.Error, "不支持的操作")]
        InvalidOperation,
        
        [ExLog(MLog.LogLevel.Error, "功能尚未实现")]
        NotImplemented,
        // ===常见Ex===
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ExLogAttribute : Attribute
    {
        public readonly MLog.LogLevel Level;
        public readonly string Message;

        public ExLogAttribute(MLog.LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
    }
    
    internal static class ExLogCache
    {
        private static readonly Dictionary<LogException, ExLogAttribute> _cache;

        static ExLogCache()
        {
            _cache = new Dictionary<LogException, ExLogAttribute>();

            var type = typeof(LogException);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var value = (LogException)field.GetValue(null);
                var attr = field.GetCustomAttribute<ExLogAttribute>();

                _cache[value] = attr;
            }
        }

        public static ExLogAttribute Get(LogException ex)
        {
            return _cache.GetValueOrDefault(ex, Default);
        }

        private static readonly ExLogAttribute Default =
            new ExLogAttribute(MLog.LogLevel.Error, "发生未知错误");
    }
}