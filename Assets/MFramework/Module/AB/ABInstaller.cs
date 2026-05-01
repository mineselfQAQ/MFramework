using MFramework.Core.CoreEx;

namespace MFramework
{
    public sealed class ABInstaller : IModuleInstaller
    {
        private readonly ABRuntimeOptions _options;

        public ABInstaller(ABRuntimeOptions options)
        {
            _options = options;
        }

        public void Install(IModuleContext context)
        {
            _options.EnsureDefaults();
            context.Container.RegisterSingleton<ABRuntimeOptions>(_options);
            context.Container.RegisterSingleton(c => new ABRuntimeState(c.Resolve<ABRuntimeOptions>()));
            context.Container.RegisterSingleton(c => new MBundleManager(c.Resolve<ABRuntimeState>()));
            context.Container.RegisterSingleton(c => new MResourceManager(c.Resolve<MBundleManager>(), c.Resolve<ABRuntimeState>()));
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MResourceManager>();
            context.Container.UnRegister<MBundleManager>();
            context.Container.UnRegister<ABRuntimeState>();
            context.Container.UnRegister<ABRuntimeOptions>();
        }
    }
}

