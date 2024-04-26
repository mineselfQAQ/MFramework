using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_FacadePattern : MonoBehaviour
{
    private void Start()
    {
        ShapeMaker shapeMaker = new ShapeMaker();
        shapeMaker.DrawCircle();
        shapeMaker.DrawRectangle();
        shapeMaker.DrawSquare();
    }

    public class ShapeMaker
    {
        private Shape circle;
        private Shape rectangle;
        private Shape square;

        public ShapeMaker()
        {
            circle = new Circle();
            rectangle = new Rectangle();
            square = new Square();
        }

        public void DrawCircle()
        {
            circle.Draw();
        }
        public void DrawRectangle()
        {
            rectangle.Draw();
        }
        public void DrawSquare()
        {
            square.Draw();
        }
    }

    public interface Shape
    {
        void Draw();
    }
    public class Circle : Shape
    {
        public void Draw()
        {
            MLog.Print("DrawCircle");
        }
    }
    public class Rectangle : Shape
    {
        public void Draw()
        {
            MLog.Print("DrawRectangle");
        }
    }
    public class Square : Shape
    {
        public void Draw()
        {
            MLog.Print("DrawSquare");
        }
    }
}
