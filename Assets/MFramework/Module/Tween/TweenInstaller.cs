using MFramework.Core.CoreEx;

namespace MFramework.Tween
{
    public class TweenInstaller : IModuleInstaller
    {
        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton(c => new MTweenManager(c.Resolve<Coroutines.MCoroutineManager>()));
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MTweenManager>();
        }
    }
}
