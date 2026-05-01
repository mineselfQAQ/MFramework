using MFramework.Core.CoreEx;

namespace MFramework
{
    public sealed class ABModule : IModule
    {
        private readonly ABRuntimeOptions _options;

        public ABModule(ABRuntimeOptions options = null)
        {
            _options = options ?? ABRuntimeOptions.CreateDefault();
        }

        public IModule[] ConfigureDependencies()
        {
            return System.Array.Empty<IModule>();
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new ABInstaller(_options),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new ABRuntimeService(),
            };
        }
    }
}

