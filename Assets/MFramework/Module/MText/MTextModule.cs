using MFramework.Core.CoreEx;
using MFramework.Coroutines;

namespace MFramework.Text
{
    public sealed class MTextModule : IModule
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
                new MTextInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new MTextRuntimeService(),
            };
        }
    }
}
