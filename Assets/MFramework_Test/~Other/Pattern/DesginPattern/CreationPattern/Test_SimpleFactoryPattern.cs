using MFramework;
using UnityEngine;

public class Test_SimpleFactoryPattern : MonoBehaviour
{
    private void Start()
    {
        Animal dog = AnimalFactory.CreateAnimal("Dog");
        dog.Eat();

        Animal cat = AnimalFactory.CreateAnimal("Cat");
        cat.Eat();

        //�����Ҫ�µĶ����Ҫ����AnimalFactory.CreateAnimal()���ⲻ���Ͽ���ԭ��
    }

    public abstract class Animal
    {
        public abstract void Eat();
    }
    public class Cat : Animal
    {
        public override void Eat()

        {
            MLog.Print("Cat Eat");
        }
    }
    public class Dog : Animal
    {
        public override void Eat()
        {
            MLog.Print("Dog Eat");
        }
    }

    public class AnimalFactory
    {
        public static Animal CreateAnimal(string type)
        {
            switch (type.ToLower())
            {
                case "cat":
                    return new Cat();
                case "dog":
                    return new Dog();
                default:
                    throw new System.Exception();
            }
        }
    }
}
