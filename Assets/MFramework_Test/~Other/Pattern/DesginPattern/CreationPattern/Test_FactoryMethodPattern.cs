using MFramework;
using UnityEngine;

public class Test_FactoryMethodPattern : MonoBehaviour
{
    private void Start()
    {
        AnimalFactory dogFactory = new DogFactory();
        Animal dog = dogFactory.CreateAnimal();
        dog.Eat();

        AnimalFactory catFactory = new CatFactory();
        Animal cat = catFactory.CreateAnimal();
        cat.Eat();

        //�����Ҫ�µĶ����ô�ʹ����µĶ����Լ��乤������
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

    public interface AnimalFactory
    {
        Animal CreateAnimal();
    }
    public class CatFactory : AnimalFactory
    {
        public Animal CreateAnimal()
        {
            return new Cat();
        }
    }
    public class DogFactory : AnimalFactory
    {
        public Animal CreateAnimal()
        {
            return new Dog();
        }
    }
}
