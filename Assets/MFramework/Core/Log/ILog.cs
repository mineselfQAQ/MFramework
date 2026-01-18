namespace MFramework.Core
{
    public interface ILog
    {
        void D(object message); // Debug
        void W(object message); // Warning
        void E(object message); // Error
    }
}