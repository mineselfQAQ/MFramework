using UnityEngine;

public class Test_OCP : MonoBehaviour
{
    //private void Start()
    //{
    //    Computer computer1 = Factory.ProduceComputer("Macbook");
    //    Computer computer2 = Factory.ProduceComputer("Surface");
    //    //������ָ����ComputerƷ�֣����Ǿ�ֻ�ܲ��ϵ��޸�ProduceComputer()
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
        //��ʹ�������µ�Computer������ֻ�������µĹ���ȥ������������ȥ�������д���
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
