using MFramework.Core;

namespace MFrameworkExamples.IOC.SL
{
    public class MEntry : MEntryBase
    {
        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            var container = MIOCContainer.CreateSL();
            
            // 因为是延迟执行，顺序无关，只要保证先注册所有后再解析即可
            container.RegisterTransient<C>(() => new C(container.Resolve<A>()));
            container.RegisterTransient<A>(() => new A(1));
            container.RegisterSingleton<B>(() => new B("A"));
            
            C c = container.Resolve<C>();
            c.Print();
            
            A a1 = container.Resolve<A>();
            a1.I = 2;
            a1.Print();
            A a2 = container.Resolve<A>();
            a2.Print();
            
            B b1 = container.Resolve<B>();
            b1.S = "B";
            b1.Print();
            B b2 = container.Resolve<B>();
            b2.Print();
            
        }
        
        public class A
        {
            public int I { get; set; }

            public A(int i)
            {
                I = i;
            }
        
            public void Print()
            {
                MLog.Default.D($"A：{I}");
            }
        }
    
        public class B
        {
            public string S { get; set; }

            public B(string s)
            {
                S = s;
            }

            public void Print()
            {
                MLog.Default.D($"B：{S}");
            }
        }

        public class C
        {
            public A A { get; set; }
            
            public C(A a)
            {
                A = a;
            }
            
            public void Print()
            {
                MLog.Default.D($"C：{A.I}");
            }
        }
    }
}