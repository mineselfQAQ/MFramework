using MFramework.Core.CoreEx;

namespace MFramework.Text
{
    public sealed class MTextModule : IModule
    {
        public IModule[] ConfigureDependencies()
        {
            return System.Array.Empty<IModule>();
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new MTextInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new MTextRuntimeService(),
            };
        }
    }
}
