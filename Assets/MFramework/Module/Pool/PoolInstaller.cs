using MFramework.Core.CoreEx;

namespace MFramework.Pool
{
    public class PoolInstaller : IModuleInstaller
    {
        private readonly MPoolManager _manager;

        public PoolInstaller(MPoolManager manager)
        {
            _manager = manager;
        }

        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton<MPoolManager>(_manager);
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MPoolManager>();
        }
    }
}
