using System.IO;

namespace MFramework.Core.Internal
{
    /// <summary>
    /// 内置功能(仅供框架使用)
    /// </summary>
    internal static partial class IntUtil
    {
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
        private readonly string _file;
        private readonly int _line;
        private readonly string _member;

        private CallerLocation(string file, int line, string member)
        {
            _file = file;
            _line = line;
            _member = member;
        }

        public static CallerLocation From(string file, int line, string member)
            => new CallerLocation(Path.GetFileName(file), line, member);

        public override string ToString()
            => $"{_member} ({_file}:{_line})";
    }
}