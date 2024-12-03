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

        // ��ӡ��Ϣ
        PrintVisitor printVisitor = new PrintVisitor();
        gameWorld.Accept(printVisitor);

        // ͳ������
        CountVisitor countVisitor = new CountVisitor();
        gameWorld.Accept(countVisitor);
        MLog.Print($"����������{countVisitor.EnemyCount}");
        MLog.Print($"����������{countVisitor.ItemCount}");
        MLog.Print($"NPC������{countVisitor.NPCCount}");
    }

    // �����߽ӿ�
    public interface IVisitor
    {
        void VisitEnemy(Enemy enemy);
        void VisitItem(Item item);
        void VisitNPC(NPC npc);
    }

    // ��������ߣ���ӡ��Ϣ
    public class PrintVisitor : IVisitor
    {
        public void VisitEnemy(Enemy enemy)
        {
            Console.WriteLine($"���ˣ�{enemy.Name}, Ѫ����{enemy.Health}");
        }

        public void VisitItem(Item item)
        {
            Console.WriteLine($"���ߣ�{item.Name}, Ч����{item.Effect}");
        }

        public void VisitNPC(NPC npc)
        {
            Console.WriteLine($"NPC��{npc.Name}, �Ի���{npc.Dialogue}");
        }
    }

    // ��������ߣ�ͳ������
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

    // Ԫ�ؽӿ�
    public interface IElement
    {
        void Accept(IVisitor visitor);
    }

    // ����Ԫ�أ�����
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

    // ����Ԫ�أ�����
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

    // ����Ԫ�أ�NPC
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

    // ����ṹ
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
