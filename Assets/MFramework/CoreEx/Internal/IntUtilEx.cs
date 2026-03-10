using System.IO;

namespace MFramework.Core.Internal
{
    /// <summary>
    /// 内置功能(仅供框架使用)
    /// </summary>
    internal static class IntUtilEx
    {
        # region Debug
        /// <summary>
        /// 获取调用处信息
        /// 问题：只能用于Editor，无法用于打包
        /// </summary>
        internal static CallerLocation GetCallerLocation(int skipFrames)
        {
            var stackTrace = new System.Diagnostics.StackTrace(skipFrames, true);
            var frame = stackTrace.GetFrame(0);
    
            var method = frame?.GetMethod();
            var sourceFilePath = frame?.GetFileName() ?? "";
            var sourceLineNumber = frame?.GetFileLineNumber() ?? 0;
            var callerMemberName = method?.Name ?? "unknown";
            
            return CallerLocation.From(sourceFilePath, sourceLineNumber, callerMemberName);
        }

        internal static CallerLocation GetCallerLocation(string file, int line, string member)
        {
            return CallerLocation.From(file, line, member);
        }
        # endregion
    }
}