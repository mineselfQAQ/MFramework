using MFramework.Core;
using MFramework.Core.CoreEx;
using NUnit.Framework;

namespace MFramework.Tests.Features.HotUpdate
{
    public class HotUpdateModuleFeatureTests
    {
        [Test]
        public void FeatureHotUpdateModule_AfterBootstrap_RegistersLuaInterpreterInDI()
        {
            var core = new MCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { new HotUpdateModule() });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            MLuaInterpreter interpreter = core.Container.Resolve<MLuaInterpreter>();

            Assert.IsNotNull(interpreter);

            bootstrap.Shutdown();
        }

        [Test]
        public void FeatureHotUpdateInstaller_AfterUninstall_RemovesLuaInterpreterFromDI()
        {
            var core = new MCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { new HotUpdateModule() });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            MLuaInterpreter interpreter = core.Container.Resolve<MLuaInterpreter>();
            bootstrap.Shutdown();

            Assert.Throws<UnityFrameworkException>(() => core.Container.Resolve<MLuaInterpreter>());
        }
    }
}
