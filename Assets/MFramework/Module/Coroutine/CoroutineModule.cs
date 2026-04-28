using MFramework.Core.CoreEx;

namespace MFramework.Coroutines
{
    public class CoroutineModule : IModule
    {
        public IModule[] ConfigureDependencies()
        {
            return System.Array.Empty<IModule>();
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new CoroutineInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new CoroutineRuntimeService(),
            };
        }
    }
}
