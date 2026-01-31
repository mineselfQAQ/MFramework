using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MFramework.Core.Internal
{
    /// <summary>
    /// 内置功能(仅供框架使用)
    /// </summary>
    internal static class IntUtil
    {
        # region 颜色
        internal static string Col(object message, Color color)
        {
            string htmlColor = ColorUtility.ToHtmlStringRGBA(color);
            string resultStr = $"<color=#{htmlColor}>{message}</color>";

            return resultStr;
        }
        # endregion
        
        # region Debug
        /// <summary>
        /// 获取调用处信息
        /// 问题：只能用于Editor，无法用于打包
        /// </summary>
        internal static string GetCallerLocation(int skipFrames)
        {
            var stackTrace = new System.Diagnostics.StackTrace(skipFrames, true);
            var frame = stackTrace.GetFrame(0);
    
            var method = frame?.GetMethod();
            var sourceFilePath = frame?.GetFileName() ?? "";
            var sourceLineNumber = frame?.GetFileLineNumber() ?? 0;
            var callerMemberName = method?.Name ?? "unknown";
                    
            return $"{callerMemberName} ({Path.GetFileName(sourceFilePath)}:{sourceLineNumber})";
        }
        # endregion
    }
    
    public readonly struct CallerLocation
    {
        public readonly string File;
        public readonly int Line;
        public readonly string Member;

        private CallerLocation(string file, int line, string member)
        {
            File = file;
            Line = line;
            Member = member;
        }

        public static CallerLocation From(string file, int line, string member)
            => new CallerLocation(Path.GetFileName(file), line, member);

        public override string ToString()
            => $"{Member} ({File}:{Line})";
    }
}