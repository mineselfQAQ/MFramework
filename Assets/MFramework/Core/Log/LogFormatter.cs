using MFramework.Core.CoreEx;
using MFramework.Core.Internal;

namespace MFramework.Core
{
    internal static class LogFormatter
    {
        private const string DEBUG_LEVEL_NAME = "D";
        private const string WARNING_LEVEL_NAME = "W";
        private const string ERROR_LEVEL_NAME = "E";
        
        public static string Build(
            MLog.LogLevel level,
            string srcName,
            string callerLocation,
            object message)
        {
            string levelStr = GetLevel(level);
            string source = $"[{srcName}]";
            string caller = $"[{callerLocation}]";

            return $"{levelStr}{source}{caller} {message}";
        }

        private static string GetLevel(MLog.LogLevel logLevel)
        {
            string name;
            if (logLevel == MLog.LogLevel.Warning) name = WARNING_LEVEL_NAME;
            else if (logLevel == MLog.LogLevel.Error) name = ERROR_LEVEL_NAME;
            else name = DEBUG_LEVEL_NAME;

            name = $"[{name}]";

#if UNITY_EDITOR
            UnityEngine.Color color;
            if (logLevel == MLog.LogLevel.Warning) color = UnityEngine.Color.yellow;
            else if (logLevel == MLog.LogLevel.Error) color = UnityEngine.Color.red;
            else color = UnityEngine.Color.green;

            name = IntUtil.Col(name, color);
#endif

            return name;
        }
    }
}
