namespace MFramework.Core.CoreEx
{
    public interface IManagedServiceContext
    {
        CoreState State { get; }
        bool IsApplicationPaused { get; }
        bool HasApplicationFocus { get; }
    }
}
