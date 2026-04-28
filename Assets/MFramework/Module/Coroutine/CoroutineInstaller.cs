using MFramework.Core.CoreEx;

namespace MFramework.Coroutines
{
    public class CoroutineInstaller : IModuleInstaller
    {
        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton<MCoroutineManager>(new MCoroutineManager());
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MCoroutineManager>();
        }
    }
}
