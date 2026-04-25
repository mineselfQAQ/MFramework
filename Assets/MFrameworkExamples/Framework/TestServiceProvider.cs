using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.IOC;

namespace MFrameworkExamples.Framework
{
    public class TestServiceProvider : IManagedService
    {
        public void Register()
        {
            var container = MIOCContainer.Default;
            var b = new B("B");
            var c = new C("C");

            container.RegisterSingleton<A>((_) => new A(container.Resolve<B>(), container.Resolve<C>()));
            container.RegisterSingleton<B>(b);
            container.RegisterSingleton<C>(c);
        }

        public void Initialize()
        {

        }

        public void Unregister()
        {

        }

        public void Shutdown()
        {

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
