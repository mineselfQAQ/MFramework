using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.IOC;

using UnityEngine;

namespace MFrameworkExamples.Framework
{
    public class TestFrameworkModule : IModule
    {
        public IModule[] ConfigureDependencies()
        {
            return System.Array.Empty<IModule>();
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new TestFrameworkInstaller(),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new TestFrameworkRuntimeService(),
            };
        }
    }

    public class TestFrameworkInstaller : IModuleInstaller
    {
        public void Install()
        {
            var container = MIOCContainer.Default;
            var b = new B("B");
            var c = new C("C");

            container.RegisterSingleton<A>((_) => new A(container.Resolve<B>(), container.Resolve<C>()));
            container.RegisterSingleton<B>(b);
            container.RegisterSingleton<C>(c);
        }

        public void Uninstall()
        {
            var container = MIOCContainer.Default;
            container.UnRegister<A>();
            container.UnRegister<B>();
            container.UnRegister<C>();
        }
    }

    public class TestFrameworkRuntimeService : IRuntimeService, IRuntimeUpdateService
    {
        public void Initialize()
        {
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                MLog.Default.D("TestFrameworkRuntimeService-UPDATE");
        }
    }

    public class A
    {
        private readonly B _b;
        private readonly C _c;

        public A(B b, C c)
        {
            _b = b;
            _c = c;
        }

        public void Print()
        {
            MLog.Default.D($"{_b.Name}+{_c.Name}");
        }
    }

    public class B
    {
        public string Name { get; }

        public B(string name)
        {
            Name = name;
        }
    }

    public class C
    {
        public string Name { get; }

        public C(string name)
        {
            Name = name;
        }
    }
}
