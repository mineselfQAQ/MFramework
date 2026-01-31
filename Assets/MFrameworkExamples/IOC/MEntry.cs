using MFramework.Core;

namespace MFrameworkExamples.IOC
{
    public class MEntry : MEntryBase
    {
        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            MSLContainer container = MIOCContainer.Default;
            
            container.RegisterTransient<A>(() => new A(1));
            A a1 = container.Resolve<A>();
            a1.I = 2;
            a1.Print();
            A a2 = container.Resolve<A>();
            a2.Print();
            
            container.RegisterInstance<B>(() => new B("A"));
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
    }
}