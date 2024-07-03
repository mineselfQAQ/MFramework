using MFramework;
using UnityEngine;

public class Test_LSP : MonoBehaviour
{
    private void Start()
    {
        Rectangle rectangle = new Square();
        //不符合里氏替换原则，诡异，明明是一个正方形长宽却不相等
        rectangle.Width = 5;
        rectangle.Height = 10;
        MLog.Print(rectangle.CalculateArea());
    }

    class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int CalculateArea()
        {
            return Width * Height;
        }
    }

    class Square : Rectangle
    {
        private int _side;

        public override int Width
        {
            get { return _side; }
            set { _side = value; }
        }

        public override int Height
        {
            get { return _side; }
            set { _side = value; }
        }
    }

    //private void Start()
    //{
    //    Shape rectangle = new Rectangle() { Width = 5, Height = 10 };
    //    MLog.Print(rectangle.CalculateArea());

    //    Shape square = new Square() { Side = 5 };
    //    MLog.Print(square.CalculateArea());
    //}

    //abstract class Shape
    //{
    //    public abstract int CalculateArea();
    //}

    //class Rectangle : Shape
    //{
    //    public int Width { get; set; }
    //    public int Height { get; set; }

    //    public override int CalculateArea()
    //    {
    //        return Width * Height;
    //    }
    //}

    //class Square : Shape
    //{
    //    public int Side { get; set; }

    //    public override int CalculateArea()
    //    {
    //        return Side * Side;
    //    }
    //}
}