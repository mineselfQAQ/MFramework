using MFramework.Core.CoreEx;
using UnityEngine;

namespace MFramework.Pool
{
    public class PoolInstaller : IModuleInstaller
    {
        private readonly Transform _defaultParent;
        private readonly int _autoWarmSize;

        public PoolInstaller(Transform defaultParent = null, int autoWarmSize = MPoolManager.DefaultAutoWarmSize)
        {
            _defaultParent = defaultParent;
            _autoWarmSize = autoWarmSize;
        }

        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton<MPoolManager>(new MPoolManager(_defaultParent, _autoWarmSize));
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MPoolManager>();
        }
    }
}
