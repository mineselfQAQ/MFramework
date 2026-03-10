using System.Runtime.CompilerServices;
using MFramework.Core.Internal;

namespace MFramework.Core.Event
{
    public class CSharpFrameworkException : FrameworkException
    {
        public CSharpFrameworkException(
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

            return $"[E][I][{location}] {message}";
        }
    }
}