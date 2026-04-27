using MFramework.Core.CoreEx;
using MFramework.Core.IOC;

namespace MFramework.Pool
{
    public class PoolInstaller : IModuleInstaller
    {
        private readonly MPoolManager _manager;

        public PoolInstaller(MPoolManager manager)
        {
            _manager = manager;
        }

        public void Install()
        {
            MIOCContainer.Default.RegisterSingleton<MPoolManager>(_manager);
        }

        public void Uninstall()
        {
            MIOCContainer.Default.UnRegister<MPoolManager>();
        }
    }
}
