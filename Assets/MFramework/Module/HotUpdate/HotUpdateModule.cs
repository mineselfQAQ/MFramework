using MFramework.Core.CoreEx;
using MFramework.Coroutines;

namespace MFramework
{
    public sealed class HotUpdateModule : IModule
    {
        private readonly ABRuntimeOptions _options;
        private readonly bool _startOnInitialize;

        public HotUpdateModule(ABRuntimeOptions options = null, bool startOnInitialize = false)
        {
            _options = options ?? ABRuntimeOptions.CreateDefault();
            _startOnInitialize = startOnInitialize;
        }

        public IModule[] ConfigureDependencies()
        {
            return new IModule[]
            {
                new ABModule(_options),
                new CoroutineModule(),
            };
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new HotUpdateInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new HotUpdateRuntimeService(_startOnInitialize),
            };
        }
    }
}
