using System.Runtime.CompilerServices;
using MFramework.Core.CoreEx;
using MFramework.Core.Event;
using MFramework.Core.Internal;

namespace MFramework.Core
{
    public class UnityFrameworkException : FrameworkException
    {
        public UnityFrameworkException(
            string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
            : base(Build(message, file, line, member))
        {
        }

        private static string Build(
            string message,
            string file,
            int line,
            string member)
        {
            var location = IntUtilEx.GetCallerLocation(file, line, member);

            return LogFormatter.Build(
                MLog.LogLevel.Error,
                IntConsts.InternalName,
                location.ToString(),
                message);
        }
    }
}
