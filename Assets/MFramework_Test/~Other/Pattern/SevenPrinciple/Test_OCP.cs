using UnityEngine;

public class Test_OCP : MonoBehaviour
{
    //private void Start()
    //{
    //    Computer computer1 = Factory.ProduceComputer("Macbook");
    //    Computer computer2 = Factory.ProduceComputer("Surface");
    //    //如果出现更多的Computer品种，我们就只能不断地修改ProduceComputer()
    //}

    //private class Factory
    //{
    //    public static Computer ProduceComputer(string type)
    //    {
    //        Computer c = null;
    //        if (type == "Macbook")
    //        {
    //            c = new Macbook();
    //        }
    //        else if (type == "Surface")
    //        {
    //            c = new Surface();
    //        }
    //        return c;
    //    }
    //}

    //private interface Computer { }
    //private class Macbook : Computer { }
    //private class Surface : Computer { }

    private void Start()
    {
        AppleFactory appleFactory = new AppleFactory();
        Computer computer1 = appleFactory.ProduceComputer();

        MSFactory msFactory = new MSFactory();
        Computer computer2 = msFactory.ProduceComputer();
        //即使出现了新的Computer，我们只会增加新的工厂去创建，但不会去更改已有代码
    }

    private interface IFactory
    {
        Computer ProduceComputer();
    }
    private class AppleFactory : IFactory
    {
        public Computer ProduceComputer()
        {
            return new Macbook();
        }
    }
    private class MSFactory : IFactory
    {
        public Computer ProduceComputer()
        {
            return new Surface();
        }
    }

    private interface Computer { }
    private class Macbook : Computer { }
    private class Surface : Computer { }
}
