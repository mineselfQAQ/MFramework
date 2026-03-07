using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MFramework.Core.Internal;

namespace MFramework.Core
{
    public interface ILog
    {
        // 原则：
        // 输出提示使用D()
        // 业务错误使用W()/E()
        // EX()只是一种便捷包装
        
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