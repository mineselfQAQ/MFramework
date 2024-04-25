using MFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Test_PrototypePattern : MonoBehaviour
{
    private void Start()
    {
        ShapeCache.LoadCache();

        Shape circle = ShapeCache.GetShape(1);
        MLog.Print(circle.CalculateArea());

        Shape square = ShapeCache.GetShape(2);
        MLog.Print(square.CalculateArea());
    }

    public class ShapeCache
    {
        private static Dictionary<int, Shape> shapeDic = new Dictionary<int, Shape>();

        public static Shape GetShape(int id)
        {
            Shape cachedShape = shapeDic[id];
            return (Shape)cachedShape.Clone();
        }

        public static void LoadCache()
        {
            Circle circle = new Circle(2);
            shapeDic.Add(1, circle);

            Square square = new Square(5);
            shapeDic.Add(2, square);
        }
    }

    public abstract class Shape : ICloneable
    {
        public abstract float CalculateArea();

        public abstract object Clone();
    }
    public class Circle : Shape
    {
        public float radius;

        public Circle(float radius)
        {
            this.radius = radius;
        }

        public override float CalculateArea()
        {
            return 2 * 3.14f * radius;
        }

        public override object Clone()
        {
            Circle circle = new Circle(radius);
            return circle;
        }
    }
    public class Square : Shape
    {
        public float side;

        public Square(float side)
        {
            this.side = side;
        }

        public override float CalculateArea()
        {
            return side * side;
        }

        public override object Clone()
        {
            Square square = new Square(side);
            return square;
        }
    }
}
