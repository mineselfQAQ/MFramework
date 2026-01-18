using MFramework.Core;
using UnityEngine;

public class TestServiceProvider : IServiceProvider
{
    public void Register()
    {
        var container = MIOCContainer.Default;
        container.RegisterInstance(()=>new B("B"));
        container.RegisterInstance(()=>new C("C"));
        container.RegisterInstance(()=>new A(container.Resolve<B>(), container.Resolve<C>()));
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