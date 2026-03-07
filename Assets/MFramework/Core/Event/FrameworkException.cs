using System;
using System.IO;
using System.Runtime.CompilerServices;
using MFramework.Core.Internal;

namespace MFramework.Core.Event
{
    public class FrameworkException : Exception
    {
        public FrameworkException(
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
            var location = CallerLocation.From(file, line, member);

            return LogFormatter.Build(
                MLog.LogLevel.Error,
                Consts.InternalName,
                location,
                message);
        }
    }
}