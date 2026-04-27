namespace MFramework.Core.CoreEx
{
    public interface IRuntimeServiceContext
    {
        CoreState State { get; }
        bool IsApplicationPaused { get; }
        bool HasApplicationFocus { get; }
    }
}
