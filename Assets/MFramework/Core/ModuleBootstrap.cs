using System.Collections.Generic;
using MFramework.Core.CoreEx;

namespace MFramework.Core
{
    public class ModuleBootstrap : IBootstrap, IShutdown
    {
        private readonly MFrameworkCore _core;
        private readonly IModule[] _modules;
        private readonly List<IModuleInstaller> _installedInstallers = new();
        private readonly HashSet<System.Type> _installedModuleTypes = new();

        public ModuleBootstrap(MFrameworkCore core, IModule[] modules)
        {
            _core = core;
            _modules = modules;
        }

        public void Bootstrap()
        {
            InstallModules(_modules);
        }

        public void Shutdown()
        {
            for (int i = _installedInstallers.Count - 1; i >= 0; i--)
            {
                _installedInstallers[i].Uninstall(_core);
            }

            _installedInstallers.Clear();
            _installedModuleTypes.Clear();
        }

        private void InstallModules(IModule[] modules)
        {
            if (modules == null) return;

            foreach (var module in modules)
            {
                InstallModule(module);
            }
        }

        private void InstallModule(IModule module)
        {
            if (module == null) return;

            var moduleType = module.GetType();
            if (!_installedModuleTypes.Add(moduleType)) return;

            InstallModules(module.ConfigureDependencies());
            Install(module.ConfigureInstallers());
            RegisterRuntimeServices(module.ConfigureRuntimeServices());
        }

        private void Install(IModuleInstaller[] installers)
        {
            if (installers == null) return;

            foreach (var installer in installers)
            {
                if (installer == null) continue;

                installer.Install(_core);
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
