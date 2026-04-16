using MFramework.Core.CoreEx;
using MFramework.Core.IOC;

using UnityEngine;

namespace MFramework.Pool
{
    /// <summary>
    /// Pool 模块服务注册器。
    /// </summary>
    public class PoolServiceProvider : IServiceProvider
    {
        private readonly Transform _defaultParent;
        private readonly int _autoWarmSize;
        private MPoolManager _manager;

        public PoolServiceProvider(Transform defaultParent = null, int autoWarmSize = MPoolManager.DefaultAutoWarmSize)
        {
            _defaultParent = defaultParent;
            _autoWarmSize = autoWarmSize;
        }

        public void Register()
        {
            _manager = new MPoolManager(_defaultParent, _autoWarmSize);
            MIOCContainer.Default.RegisterSingleton<MPoolManager>(_manager);
        }

        public void Initialize()
        {
        }

        public void Unregister()
        {
        }

        public void Shutdown()
        {
            _manager?.Shutdown();
        }
    }
}
