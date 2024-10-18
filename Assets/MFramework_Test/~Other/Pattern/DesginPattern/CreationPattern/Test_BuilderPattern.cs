using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_BuilderPattern : MonoBehaviour
{
    private void Start()
    {
        MealBuilder builder = new MealBuilder();
        Meal vegMeal = builder.PrepareVegMeal();
        vegMeal.ShowItems();
        vegMeal.GetCost();

        Meal nonVegMeal = builder.PrepareNonVegMeal();
        nonVegMeal.ShowItems();
        nonVegMeal.GetCost();

        //MealBuilder能使用各个配件组合出更多不同的套餐
    }

    public class MealBuilder
    {
        public Meal PrepareVegMeal()
        {
            Meal meal = new Meal();
            meal.AddItem(new VegBurger());
            meal.AddItem(new Coke());
            return meal;
        }
        public Meal PrepareNonVegMeal()
        {
            Meal meal = new Meal();
            meal.AddItem(new ChickenBurger());
            meal.AddItem(new Pepsi());
            return meal;
        }
    }

    public class Meal
    {
        private List<Item> items = new List<Item>();
        
        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public float GetCost()
        {
            float cost = 0.0f;
            foreach (var item in items)
            {
                cost += item.Price();
            }
            return cost;
        }

        public void ShowItems()
        {
            foreach (var item in items)
            {
                MLog.Print($"Item: {item.Name()}, Packing: {item.Packing()}, Price: {item.Price()}");
            }
        }
    }

    public interface Item
    {
        string Name();
        Packing Packing();
        float Price();
    }

    public abstract class Burger : Item
    {
        public abstract string Name();

        public Packing Packing()
        {
            return new Wrapper();
        }

        public abstract float Price();
    }
    public class VegBurger : Burger
    {
        public override string Name()
        {
            return "Veg Burger";
        }

        public override float Price()
        {
            return 30.0f;
        }
    }
    public class ChickenBurger : Burger
    {
        public override string Name()
        {
            return "Chicken Burger";
        }

        public override float Price()
        {
            return 50.0f;
        }
    }

    public abstract class ColdDrink : Item
    {
        public abstract string Name();

        public Packing Packing()
        {
            return new Bottle();
        }

        public abstract float Price();
    }
    public class Coke : ColdDrink
    {
        public override string Name()
        {
            return "Coke";
        }

        public override float Price()
        {
            return 8.0f;
        }
    }
    public class Pepsi : ColdDrink
    {
        public override string Name()
        {
            return "Pepsi";
        }

        public override float Price()
        {
            return 9.0f;
        }
    }

    public interface Packing
    {
        string Pack();
    }
    public class Wrapper : Packing
    {
        public string Pack()
        {
            return "Wrapper";
        }
    }
    public class Bottle : Packing
    {
        public string Pack()
        {
            return "Bottle";
        }
    }
}

public class Test_BuilderPattern2 : MonoBehaviour
{
    private void Start()
    {
        // 使用建造者模式创建角色
        var knight = new CharacterBuilder()
                        .SetName("Knight")
                        .SetHealth(150)
                        .SetAttackPower(20)
                        .SetWeapon("Great Sword")
                        .Build();
    }

    public class Character
    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int AttackPower { get; private set; }
        public string Weapon { get; private set; }

        // 使用 Builder 进行初始化
        public Character(CharacterBuilder builder)
        {
            Name = builder.Name;
            Health = builder.Health;
            AttackPower = builder.AttackPower;
            Weapon = builder.Weapon;
        }
    }

    public class CharacterBuilder
    {
        public string Name { get; private set; }
        public int Health { get; private set; } = 100;
        public int AttackPower { get; private set; } = 10;
        public string Weapon { get; private set; } = "Sword";

        public CharacterBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public CharacterBuilder SetHealth(int health)
        {
            Health = health;
            return this;
        }

        public CharacterBuilder SetAttackPower(int attackPower)
        {
            AttackPower = attackPower;
            return this;
        }

        public CharacterBuilder SetWeapon(string weapon)
        {
            Weapon = weapon;
            return this;
        }

        public Character Build()
        {
            return new Character(this);
        }
    }
}