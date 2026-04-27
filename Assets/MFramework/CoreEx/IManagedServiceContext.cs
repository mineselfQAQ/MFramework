namespace MFramework.Core.CoreEx
{
    public interface IRuntimeServiceContext
    {
        IDIContainer Container { get; }
        CoreState State { get; }
        bool IsApplicationPaused { get; }
        bool HasApplicationFocus { get; }
    }

    public interface IModuleContext
    {
        IDIContainer Container { get; }
        CoreState State { get; }
    }
}
