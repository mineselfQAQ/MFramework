using UnityEngine;

namespace MFrameworkCSTests.Reflection1
{
    public class Test_Reflection_1 : MonoBehaviour
    {
        void Start()
        {
            var constructors = typeof(Test).GetConstructors();
            foreach (var constructor in constructors)
            {
                // 测试结果：在Unity环境中，顺序取决于代码编写顺序(3->1->0->2)
                Debug.Log(constructor.GetParameters().Length);
            }
        }

        public class Test
        {
            public int a;
            public float b;
            public string c;

            public Test(int a, float b, string c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public Test(int a)
            {
                this.a = a;
            }

            public Test()
            {
                
            }
            
            public Test(int a, float b)
            {
                this.a = a;
                this.b = b;
            }
        }
    }
}