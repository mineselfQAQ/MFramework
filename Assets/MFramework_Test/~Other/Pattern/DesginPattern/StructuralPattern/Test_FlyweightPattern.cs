using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_FlyweightPattern : MonoBehaviour
{
    private void Start()
    {
        Circle redCircle = (Circle)ShapeFactory.GetCircle(Color.red);
        redCircle.SetXY(0, 0);
        redCircle.SetRadius(1);
        redCircle.Draw();
        Circle greenCircle = (Circle)ShapeFactory.GetCircle(Color.green);
        greenCircle.SetXY(2, 3);
        greenCircle.SetRadius(5);
        greenCircle.Draw();
        Circle greenCircle2 = (Circle)ShapeFactory.GetCircle(Color.green);
        greenCircle2.Draw();//与greenCircle一致，说明是同一对象
    }

    public class ShapeFactory
    {
        private static Dictionary<Color, Shape> shapeDic = new Dictionary<Color, Shape>();

        public static Shape GetCircle(Color color)
        {
            if (shapeDic.ContainsKey(color))
            {
                return shapeDic[color];
            }
            else
            {
                Shape shape = new Circle(color);
                shapeDic.Add(color, shape);
                return shape;
            }
        }
    }

    public interface Shape
    {
        void Draw();
    }
    public class Circle : Shape
    {
        private int x;
        private int y;
        private int radius;
        private Color color;

        public Circle(Color color)
        {
            this.color = color;
        }

        public void SetXY(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void SetRadius(int radius)
        {
            this.radius = radius;
        }

        public void Draw()
        {
            MLog.Print($"{color}Cirlce: Radius{radius}, [{x},{y}]");
        }
    }
}
