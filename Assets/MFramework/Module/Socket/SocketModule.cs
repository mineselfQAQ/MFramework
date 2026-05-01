using MFramework.Core.CoreEx;

namespace MFramework.Socket
{
    public sealed class SocketModule : IModule
    {
        public IModule[] ConfigureDependencies()
        {
            return System.Array.Empty<IModule>();
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new SocketInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new SocketRuntimeService(),
            };
        }
    }

    public sealed class SocketInstaller : IModuleInstaller
    {
        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton<ISocketMainThreadDispatcher>(new SocketMainThreadDispatcher());
            context.Container.RegisterSingleton<IMSocketFactory>(container =>
                new MSocketFactory(container.Resolve<ISocketMainThreadDispatcher>()));
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<IMSocketFactory>();
            context.Container.UnRegister<ISocketMainThreadDispatcher>();
        }
    }

    public sealed class SocketRuntimeService : IRuntimeService, IRuntimeServiceWithContext
    {
        private IRuntimeServiceContext _context;

        public void BindContext(IRuntimeServiceContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Container.Resolve<ISocketMainThreadDispatcher>().Capture();
        }

        public void Shutdown()
        {
        }
    }
}
