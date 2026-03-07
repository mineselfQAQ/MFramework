using System.Collections.Generic;
using MFramework.Core;
using UnityEngine;

namespace MFrameworkExamples.IOC.DI
{
    public class MEntry : MEntryBase
    {
        protected override MLog.LogFilter SetLogFilter()
        {
            return MLog.LogFilter.Debug;
        }
        
        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            var container = MIOCContainer.CreateDI();
            
            // 1.一般注入测试
            container.RegisterTransient<IA, A>();
            container.RegisterTransient<IB, B>();
            container.RegisterTransient<IC, C>();
            
            // 2.一对多注入测试
            container.RegisterTransient<ArrContainer>();
            container.RegisterTransient<IArr, Arr1>("Arr1");
            container.RegisterTransient<IArr, Arr2>("Arr2");
            
            // 3.生命周期测试
            container.RegisterSingleton<D>(new D(1));
            container.RegisterScoped<E>((c) => new E(1));
            container.RegisterTransient<F>((c) => new F(1));

            var ee = container.Resolve<E>();
            Debug.Log(ee.Value);
            
            // // 1-1-1-1-1
            // MLog.Default.D("1.一般注入测试");
            //
            // container.Resolve<IA>().Print();
            // // 1-1-1-1-1
            //
            // // 2-2-2-2-2
            // MLog.Default.D("2.一对多注入测试");
            //
            // container.Resolve<ArrContainer>().Print();
            // // 2-2-2-2-2
            //
            // // 3-3-3-3-3
            // MLog.Default.D("3.生命周期测试");
            // // Tip：
            // // 1.根容器禁止Scoped解析(如果在根容器解析那么就会等价于Singleton注册，语义不明确)
            //
            // MLog.Default.D("Root域-Start");
            //
            // var d1 = container.Resolve<D>();
            // d1.Value = 2;
            // d1 = container.Resolve<D>(); // 只可能取同一个实例
            // MLog.Default.D($"D1：{d1.Value}");
            //
            // var f1 = container.Resolve<F>();
            // f1.Value = 2;
            // var f2 = container.Resolve<F>();
            // MLog.Default.D($"F1：{f1.Value}");
            // MLog.Default.D($"F2：{f2.Value}");
            //
            // using (var sub1 = container.CreateScope())
            // {
            //     MLog.Default.D("Sub1域-Start");
            //
            //     var e1 = sub1.Resolve<E>();
            //     e1.Value = 2;
            //     MLog.Default.D($"E1：{e1.Value}");
            //     e1 = sub1.Resolve<E>(); // 同一域下为同一实例
            //     MLog.Default.D($"E1：{e1.Value}");
            //     
            //     // 嵌套结构并非嵌套关系，sub1/sub2为平级关系
            //     using (var sub2 = container.CreateScope())
            //     {
            //         MLog.Default.D("Sub2域-Start");
            //     
            //         var e2 = sub2.Resolve<E>(); // 不同域下为不同实例
            //         MLog.Default.D($"E1：{e1.Value}"); // 内部能直接取到，无需Resolve
            //         MLog.Default.D($"E2：{e2.Value}");
            //     
            //         MLog.Default.D("Sub2域-End");
            //     }
            //     
            //     MLog.Default.D("Sub1域-End");
            // }
            //
            // MLog.Default.D("Root域-End");
            // // 3-3-3-3-3
        }

        public interface IA
        {
            void Print();
        }
        
        public class A : IA
        {
            public IB B { get; }
            public IC C { get; }

            public A(IB b, IC c)
            {
                B = b;
                C = c;
            }
        
            public void Print()
            {
                B.Print();
                C.Print();
            }
        }
        
        public interface IB
        {
            void Print();
        }
    
        public class B : IB
        {
            public void Print()
            {
                MLog.Default.D("B");
            }
        }
        
        public interface IC
        {
            void Print();
        }

        public class C : IC
        {
            public void Print()
            {
                MLog.Default.D("C");
            }
        }

        public class D
        {
            public int Value { get; set; }

            public D(int value)
            {
                Value = value;
            }
        }
        
        public class E
        {
            public int Value { get; set; }

            public E(int value)
            {
                Value = value;
            }
        }
        
        public class F
        {
            public int Value { get; set; }

            public F(int value)
            {
                Value = value;
            }
        }
            
        public interface IArr
        {
            void Print();
        }

        public class Arr1 : IArr
        {
            public void Print()
            {
                MLog.Default.D("Arr1");
            }
        }
        
        public class Arr2 : IArr
        {
            public void Print()
            {
                MLog.Default.D("Arr2");
            }
        }

        public class ArrContainer
        {
            // 注意：
            // DI仅支持IEnumerable<T>与T[]，如想使用List<T>则可自行转换
            
            private readonly IEnumerable<IArr> _arrs;

            public ArrContainer(IEnumerable<IArr> arrs)
            {
                _arrs = arrs;
                
                // _list = arrs.ToList(); // List<T>转换
            }

            public void Print()
            {
                foreach (var arr in _arrs)
                {
                    arr.Print();
                }
            }
        }
    }
}