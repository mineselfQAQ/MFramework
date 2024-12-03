using MFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Test_VisitorPattern2 : MonoBehaviour
{
    private void Start()
    {
        ObjectStructure gameWorld = new ObjectStructure();
        gameWorld.AddElement(new Enemy("Goblin", 100));
        gameWorld.AddElement(new Item("Health Potion", "+50 HP"));
        gameWorld.AddElement(new NPC("Villager", "Hello, traveler!"));

        // 打印信息
        PrintVisitor printVisitor = new PrintVisitor();
        gameWorld.Accept(printVisitor);

        // 统计数量
        CountVisitor countVisitor = new CountVisitor();
        gameWorld.Accept(countVisitor);
        MLog.Print($"敌人数量：{countVisitor.EnemyCount}");
        MLog.Print($"道具数量：{countVisitor.ItemCount}");
        MLog.Print($"NPC数量：{countVisitor.NPCCount}");
    }

    // 访问者接口
    public interface IVisitor
    {
        void VisitEnemy(Enemy enemy);
        void VisitItem(Item item);
        void VisitNPC(NPC npc);
    }

    // 具体访问者：打印信息
    public class PrintVisitor : IVisitor
    {
        public void VisitEnemy(Enemy enemy)
        {
            Console.WriteLine($"敌人：{enemy.Name}, 血量：{enemy.Health}");
        }

        public void VisitItem(Item item)
        {
            Console.WriteLine($"道具：{item.Name}, 效果：{item.Effect}");
        }

        public void VisitNPC(NPC npc)
        {
            Console.WriteLine($"NPC：{npc.Name}, 对话：{npc.Dialogue}");
        }
    }

    // 具体访问者：统计数量
    public class CountVisitor : IVisitor
    {
        public int EnemyCount { get; private set; } = 0;
        public int ItemCount { get; private set; } = 0;
        public int NPCCount { get; private set; } = 0;

        public void VisitEnemy(Enemy enemy)
        {
            EnemyCount++;
        }

        public void VisitItem(Item item)
        {
            ItemCount++;
        }

        public void VisitNPC(NPC npc)
        {
            NPCCount++;
        }
    }

    // 元素接口
    public interface IElement
    {
        void Accept(IVisitor visitor);
    }

    // 具体元素：敌人
    public class Enemy : IElement
    {
        public string Name { get; }
        public int Health { get; }

        public Enemy(string name, int health)
        {
            Name = name;
            Health = health;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitEnemy(this);
        }
    }

    // 具体元素：道具
    public class Item : IElement
    {
        public string Name { get; }
        public string Effect { get; }

        public Item(string name, string effect)
        {
            Name = name;
            Effect = effect;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitItem(this);
        }
    }

    // 具体元素：NPC
    public class NPC : IElement
    {
        public string Name { get; }
        public string Dialogue { get; }

        public NPC(string name, string dialogue)
        {
            Name = name;
            Dialogue = dialogue;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitNPC(this);
        }
    }

    // 对象结构
    public class ObjectStructure
    {
        private readonly List<IElement> _elements = new();

        public void AddElement(IElement element)
        {
            _elements.Add(element);
        }

        public void Accept(IVisitor visitor)
        {
            foreach (var element in _elements)
            {
                element.Accept(visitor);
            }
        }
    }
}
