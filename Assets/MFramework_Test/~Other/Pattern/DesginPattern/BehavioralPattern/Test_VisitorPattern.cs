using MFramework;
using UnityEngine;

public class Test_VisitorPattern : MonoBehaviour
{
    private void Start()
    {
        ComputerPart computer = new Computer();
        computer.Accept(new ComputerPartDisplayVisitor());
        computer.Accept(new ComputerPartDisplayVisitor2());
    }

    public interface ComputerPartVisitor
    {
        public void Visit(Mouse mouse);
        public void Visit(Keyboard keyboard);
        public void Visit(Monitor monitor);
        public void Visit(Computer computer);
    }
    public class ComputerPartDisplayVisitor : ComputerPartVisitor
    {
        public void Visit(Mouse mouse)
        {
            MLog.Print("Mouse1");
        }

        public void Visit(Keyboard keyboard)
        {
            MLog.Print("Keyboard1");
        }

        public void Visit(Monitor monitor)
        {
            MLog.Print("Monitor1");
        }

        public void Visit(Computer computer)
        {
            MLog.Print("Computer1");
        }
    }
    public class ComputerPartDisplayVisitor2 : ComputerPartVisitor
    {
        public void Visit(Mouse mouse)
        {
            MLog.Print("Mouse2");
        }

        public void Visit(Keyboard keyboard)
        {
            MLog.Print("Keyboard2");
        }

        public void Visit(Monitor monitor)
        {
            MLog.Print("Monitor2");
        }

        public void Visit(Computer computer)
        {
            MLog.Print("Computer2");
        }
    }

    public interface ComputerPart
    {
        public void Accept(ComputerPartVisitor computerPartVisitor);
    }
    public class Mouse : ComputerPart
    {
        public void Accept(ComputerPartVisitor computerPartVisitor)
        {
            computerPartVisitor.Visit(this);
        }
    }
    public class Keyboard : ComputerPart
    {
        public void Accept(ComputerPartVisitor computerPartVisitor)
        {
            computerPartVisitor.Visit(this);
        }
    }
    public class Monitor : ComputerPart
    {
        public void Accept(ComputerPartVisitor computerPartVisitor)
        {
            computerPartVisitor.Visit(this);
        }
    }
    public class Computer : ComputerPart
    {
        private ComputerPart[] parts;

        public Computer()
        {
            parts = new ComputerPart[] { new Mouse(), new Keyboard(), new Monitor() };
        }

        public void Accept(ComputerPartVisitor computerPartVisitor)
        {
            foreach (var part in parts)
            {
                part.Accept(computerPartVisitor);
            }
            computerPartVisitor.Visit(this);
        }
    }
}
