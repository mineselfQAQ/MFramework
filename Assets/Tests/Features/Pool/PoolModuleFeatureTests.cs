using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Pool;
using NUnit.Framework;

namespace MFramework.Tests.Features.Pool
{
    public class PoolModuleFeatureTests
    {
        [Test]
        public void FeaturePoolModule_AfterBootstrap_RegistersManagerInDI()
        {
            var core = new MCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { new PoolModule() });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();

            Assert.IsNotNull(core.Container.Resolve<MPoolManager>());

            core.Initialize();
            core.Shutdown();
        }

        [Test]
        public void FeaturePoolInstaller_AfterUninstall_RemovesManagerFromDI()
        {
            var core = new MCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { new PoolModule() });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            core.Initialize();
            core.Shutdown();

            Assert.Throws<UnityFrameworkException>(() => core.Container.Resolve<MPoolManager>());
        }
    }
}
