using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Coroutines;
using NUnit.Framework;

namespace MFramework.Tests.Features.Coroutine
{
    public class CoroutineModuleFeatureTests
    {
        [Test]
        public void FeatureCoroutineModule_AfterBootstrap_RegistersManagerInDI()
        {
            var core = new MCore();
            var module = new CoroutineModule();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { module });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            MCoroutineManager manager = core.Container.Resolve<MCoroutineManager>();

            Assert.IsNotNull(manager);

            core.Initialize();
            core.Shutdown();
        }

        [Test]
        public void FeatureCoroutineInstaller_AfterUninstall_RemovesManagerFromDI()
        {
            var core = new MCore();
            var module = new CoroutineModule();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { module });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            core.Initialize();
            core.Shutdown();

            Assert.Throws<UnityFrameworkException>(() => core.Container.Resolve<MCoroutineManager>());
        }
    }
}
