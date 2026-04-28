using MFramework.Core.CoreEx;
using MFramework.Coroutines;

namespace MFramework.Tween
{
    public class TweenModule : IModule
    {
        public IModule[] ConfigureDependencies()
        {
            return new IModule[]
            {
                new CoroutineModule(),
            };
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new TweenInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return System.Array.Empty<IRuntimeService>();
        }
    }
}
