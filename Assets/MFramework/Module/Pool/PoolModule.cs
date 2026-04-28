using MFramework.Core.CoreEx;
using UnityEngine;

namespace MFramework.Pool
{
    public class PoolModule : IModule
    {
        private readonly Transform _defaultParent;
        private readonly int _autoWarmSize;

        public PoolModule(Transform defaultParent = null, int autoWarmSize = MPoolManager.DefaultAutoWarmSize)
        {
            _defaultParent = defaultParent;
            _autoWarmSize = autoWarmSize;
        }

        public IModule[] ConfigureDependencies()
        {
            return System.Array.Empty<IModule>();
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new PoolInstaller(_defaultParent, _autoWarmSize),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new PoolRuntimeService(),
            };
        }
    }
}
