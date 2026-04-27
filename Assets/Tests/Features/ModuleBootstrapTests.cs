using System.Collections.Generic;
using MFramework.Core;
using MFramework.Core.CoreEx;
using NUnit.Framework;

namespace MFramework.Tests.Features
{
    public class ModuleBootstrapTests
    {
        [Test]
        public void CoreLifecycle_WithModule_RunsInstallerAndRuntimeInOrder()
        {
            var events = new List<string>();
            var installer = new RecordingInstaller(events);
            var runtime = new RecordingRuntimeService(events);
            var module = new RecordingModule(installer, runtime);
            var core = new MFrameworkCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { module });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            core.Initialize();
            core.Shutdown();

            CollectionAssert.AreEqual(
                new[] { "install", "initialize", "runtime-shutdown", "uninstall" },
                events);
        }

        private sealed class RecordingModule : IModule
        {
            private readonly IModuleInstaller _installer;
            private readonly IRuntimeService _runtimeService;

            public RecordingModule(IModuleInstaller installer, IRuntimeService runtimeService)
            {
                _installer = installer;
                _runtimeService = runtimeService;
            }

            public IModuleInstaller[] ConfigureInstallers()
            {
                return new[] { _installer };
            }

            public IRuntimeService[] ConfigureRuntimeServices()
            {
                return new[] { _runtimeService };
            }
        }

        private sealed class RecordingInstaller : IModuleInstaller
        {
            private readonly List<string> _events;

            public RecordingInstaller(List<string> events)
            {
                _events = events;
            }

            public void Install()
            {
                _events.Add("install");
            }

            public void Uninstall()
            {
                _events.Add("uninstall");
            }
        }

        private sealed class RecordingRuntimeService : IRuntimeService
        {
            private readonly List<string> _events;

            public RecordingRuntimeService(List<string> events)
            {
                _events = events;
            }

            public void Initialize()
            {
                _events.Add("initialize");
            }

            public void Shutdown()
            {
                _events.Add("runtime-shutdown");
            }
        }
    }
}
