using System.Runtime.CompilerServices;
using MFramework.Core.Internal;

namespace MFramework.Core
{
    public interface ILog
    {
        void D(object message, 
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = ""); // Debug
        
        void W(object message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = ""); // Warning
        
        void E(object message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = ""); // Error
        
        void EX(LogException ex, MLog.LogLevel? overrideLevel = null,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = ""); // Exception
    }
}