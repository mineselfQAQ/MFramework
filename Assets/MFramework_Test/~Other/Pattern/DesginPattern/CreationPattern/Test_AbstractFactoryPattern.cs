using MFramework;
using UnityEngine;

public class Test_AbstractFactoryPattern : MonoBehaviour
{
    private void Start()
    {
        AnimalFactory femaleFactory = new FemaleAnimalFactory();
        Animal femaleCat = femaleFactory.CreateCat();
        femaleCat.Eat();
        femaleCat.Gender();

        AnimalFactory maleFactory = new MaleAnimalFactory();
        Animal maleDog = maleFactory.CreateDog();
        maleDog.Eat();
        maleDog.Gender();
    }

    public abstract class Animal
    {
        public abstract void Eat();

        public abstract void Gender();
    }

    public abstract class Cat : Animal
    {
        public override void Eat()
        {
            MLog.Print("Cat Eat");
        }
    }
    public class FemaleCat : Cat
    {
        public override void Gender()
        {
            MLog.Print("Female");
        }
    }
    public class MaleCat : Cat
    {
        public override void Gender()
        {
            MLog.Print("Male");
        }
    }

    public abstract class Dog : Animal
    {
        public override void Eat()
        {
            MLog.Print("Dog Eat");
        }
    }
    public class FemaleDog : Cat
    {
        public override void Gender()
        {
            MLog.Print("Female");
        }
    }
    public class MaleDog : Cat
    {
        public override void Gender()
        {
            MLog.Print("Male");
        }
    }

    public interface AnimalFactory
    {
        Animal CreateCat();
        Animal CreateDog();
    }
    public class FemaleAnimalFactory : AnimalFactory
    {
        public Animal CreateCat()
        {
            return new FemaleCat();
        }

        public Animal CreateDog()
        {
            return new FemaleDog();
        }
    }
    public class MaleAnimalFactory : AnimalFactory
    {
        public Animal CreateCat()
        {
            return new MaleCat();
        }

        public Animal CreateDog()
        {
            return new MaleDog();
        }
    }
}
