using MFramework.Core.CoreEx;
using MFramework.Coroutines;

namespace MFramework
{
    public sealed class HotUpdateInstaller : IModuleInstaller
    {
        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton(c => new MHotUpdateManager(c.Resolve<MCoroutineManager>(), c.Resolve<ABRuntimeState>()));
            context.Container.RegisterSingleton(c => new MLuaInterpreter(c.Resolve<ABRuntimeState>(), c.Resolve<MResourceManager>()));
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MLuaInterpreter>();
            context.Container.UnRegister<MHotUpdateManager>();
        }
    }
}
