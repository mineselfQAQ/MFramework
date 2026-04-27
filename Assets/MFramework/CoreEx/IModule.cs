namespace MFramework.Core.CoreEx
{
    /// <summary>
    /// Feature composition entry.
    /// </summary>
    public interface IModule
    {
        IModule[] ConfigureDependencies();

        IModuleInstaller[] ConfigureInstallers();

        IRuntimeService[] ConfigureRuntimeServices();
    }
}
