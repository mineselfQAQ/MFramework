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

        [Test]
        public void Bootstrap_WithDuplicateDependencyModule_InstallsDependencyOnceBeforeOwner()
        {
            var events = new List<string>();
            var sharedDependencyA = new SharedDependencyModule(events);
            var sharedDependencyB = new SharedDependencyModule(events);
            var featureA = new RecordingModule(
                new RecordingInstaller(events, "feature-a-install", "feature-a-uninstall"),
                new RecordingRuntimeService(events, "feature-a-initialize", "feature-a-shutdown"),
                new IModule[] { sharedDependencyA });
            var featureB = new RecordingModule(
                new RecordingInstaller(events, "feature-b-install", "feature-b-uninstall"),
                new RecordingRuntimeService(events, "feature-b-initialize", "feature-b-shutdown"),
                new IModule[] { sharedDependencyB });
            var core = new MFrameworkCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { featureA, featureB });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();
            core.Initialize();
            core.Shutdown();

            CollectionAssert.AreEqual(
                new[]
                {
                    "dependency-install",
                    "feature-a-install",
                    "feature-b-install",
                    "dependency-initialize",
                    "feature-a-initialize",
                    "feature-b-initialize",
                    "dependency-shutdown",
                    "feature-a-shutdown",
                    "feature-b-shutdown",
                    "feature-b-uninstall",
                    "feature-a-uninstall",
                    "dependency-uninstall",
                },
                events);
        }

        private sealed class RecordingModule : IModule
        {
            private readonly IModule[] _dependencies;
            private readonly IModuleInstaller _installer;
            private readonly IRuntimeService _runtimeService;

            public RecordingModule(IModuleInstaller installer, IRuntimeService runtimeService, IModule[] dependencies = null)
            {
                _dependencies = dependencies;
                _installer = installer;
                _runtimeService = runtimeService;
            }

            public IModule[] ConfigureDependencies()
            {
                return _dependencies;
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
            private readonly string _installEvent;
            private readonly string _uninstallEvent;

            public RecordingInstaller(List<string> events, string installEvent = "install", string uninstallEvent = "uninstall")
            {
                _events = events;
                _installEvent = installEvent;
                _uninstallEvent = uninstallEvent;
            }

            public void Install()
            {
                _events.Add(_installEvent);
            }

            public void Uninstall()
            {
                _events.Add(_uninstallEvent);
            }
        }

        private sealed class RecordingRuntimeService : IRuntimeService
        {
            private readonly List<string> _events;
            private readonly string _initializeEvent;
            private readonly string _shutdownEvent;

            public RecordingRuntimeService(List<string> events, string initializeEvent = "initialize", string shutdownEvent = "runtime-shutdown")
            {
                _events = events;
                _initializeEvent = initializeEvent;
                _shutdownEvent = shutdownEvent;
            }

            public void Initialize()
            {
                _events.Add(_initializeEvent);
            }

            public void Shutdown()
            {
                _events.Add(_shutdownEvent);
            }
        }

        private sealed class SharedDependencyModule : IModule
        {
            private readonly List<string> _events;

            public SharedDependencyModule(List<string> events)
            {
                _events = events;
            }

            public IModule[] ConfigureDependencies()
            {
                return null;
            }

            public IModuleInstaller[] ConfigureInstallers()
            {
                return new IModuleInstaller[]
                {
                    new RecordingInstaller(_events, "dependency-install", "dependency-uninstall"),
                };
            }

            public IRuntimeService[] ConfigureRuntimeServices()
            {
                return new IRuntimeService[]
                {
                    new RecordingRuntimeService(_events, "dependency-initialize", "dependency-shutdown"),
                };
            }
        }
    }
}
