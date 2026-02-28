using MFramework.Core;

namespace MFrameworkExamples.IOC.DI
{
    public class MEntry : MEntryBase
    {
        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            var container = MIOCContainer.CreateDI();
            
            A a1 = new A(88);
            A a2 = new A(22);
            container.RegisterSingleton<IA>("A1", a1);
            container.RegisterSingleton<IA>("A2", a2);

            using (var sub1 = container.CreateScope())
            {
                sub1.Resolve<IA>("A1").Print();
                sub1.Resolve<IA>("A2").Print();
            }
        }

        public interface IA
        {
            void Print();
        }
        
        public class A : IA
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