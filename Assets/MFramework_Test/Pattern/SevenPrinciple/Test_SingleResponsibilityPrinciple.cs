using MFramework;
using UnityEngine;

public class Test_SingleResponsibilityPrinciple : MonoBehaviour
{
    //private void Start()
    //{
    //    Animal dog = new Animal("Dog");
    //    dog.Eat();
    //    Animal cat = new Animal("Cat");
    //    cat.Eat(); 
    //    Animal lion = new Animal("Lion");
    //    lion.Eat();

    //    //出现了不吃肉物种，Eat()出现问题
    //    Animal rabbit = new Animal("Rabbit");
    //    rabbit.Eat();
    //}

    //private class Animal
    //{
    //    public string species;

    //    public Animal(string species)
    //    {
    //        this.species = species;
    //    }

    //    public void Eat()
    //    {
    //        MLog.Print($"{species}吃肉");
    //    }
    //}

    private void Start()
    {
        MeetAnimal dog = new MeetAnimal("Dog");
        dog.Eat();
        MeetAnimal cat = new MeetAnimal("Cat");
        cat.Eat();
        MeetAnimal lion = new MeetAnimal("Lion");
        lion.Eat();

        GrassAnimal rabbit = new GrassAnimal("Rabbit");
        rabbit.Eat();
    }

    private class MeetAnimal
    {
        public string species;

        public MeetAnimal(string species)
        {
            this.species = species;
        }

        public void Eat()
        {
            MLog.Print($"{species}吃肉");
        }
    }

    private class GrassAnimal
    {
        public string species;

        public GrassAnimal(string species)
        {
            this.species = species;
        }

        public void Eat()
        {
            MLog.Print($"{species}吃草");
        }
    }
}
