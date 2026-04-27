using System.Collections.Generic;
using MFramework.Core.CoreEx;

namespace MFramework.Core
{
    public class ModuleBootstrap : IBootstrap, IShutdown
    {
        private readonly MFrameworkCore _core;
        private readonly IModule[] _modules;
        private readonly List<IModuleInstaller> _installedInstallers = new();

        public ModuleBootstrap(MFrameworkCore core, IModule[] modules)
        {
            _core = core;
            _modules = modules;
        }

        public void Bootstrap()
        {
            if (_modules == null) return;

            foreach (var module in _modules)
            {
                if (module == null) continue;

                Install(module.ConfigureInstallers());
                RegisterRuntimeServices(module.ConfigureRuntimeServices());
            }
        }

        public void Shutdown()
        {
            for (int i = _installedInstallers.Count - 1; i >= 0; i--)
            {
                _installedInstallers[i].Uninstall();
            }

            _installedInstallers.Clear();
        }

        private void Install(IModuleInstaller[] installers)
        {
            if (installers == null) return;

            foreach (var installer in installers)
            {
                if (installer == null) continue;

                installer.Install();
                _installedInstallers.Add(installer);
            }
        }

        private void RegisterRuntimeServices(IRuntimeService[] runtimeServices)
        {
            if (runtimeServices == null) return;

            foreach (var runtimeService in runtimeServices)
            {
                if (runtimeService == null) continue;

                _core.Register(runtimeService);
            }
        }
    }
}
