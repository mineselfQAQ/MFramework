namespace MFramework.Core.CoreEx
{
    /// <summary>
    /// Feature composition entry.
    /// </summary>
    public interface IModule
    {
        IModuleInstaller[] ConfigureInstallers();

        IRuntimeService[] ConfigureRuntimeServices();
    }
}
