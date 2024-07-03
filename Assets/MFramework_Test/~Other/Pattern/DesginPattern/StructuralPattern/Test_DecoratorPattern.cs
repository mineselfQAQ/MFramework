using MFramework;
using UnityEngine;

public class Test_Decorator : MonoBehaviour
{
    private void Start()
    {
        Circle circle = new Circle();
        circle.Draw();
        ShapeDecorator redCircle = new RedShapeDecorator(circle);
        redCircle.Draw();

        ShapeDecorator redRect = new RedShapeDecorator(new Rectangle());
        redRect.Draw();
    }

    public abstract class ShapeDecorator : Shape
    {
        protected Shape decoratedShape;

        public ShapeDecorator(Shape decoratedShape)
        {
            this.decoratedShape = decoratedShape;
        }

        public virtual void Draw()
        {
            decoratedShape.Draw();
        }
    }
    public class RedShapeDecorator : ShapeDecorator
    {
        public RedShapeDecorator(Shape decoratedShape) : base(decoratedShape){ }

        public override void Draw()
        {
            base.Draw();
            SetRedBorder(decoratedShape);

        }

        private void SetRedBorder(Shape decoratedShape)
        {
            MLog.Print("Draw red border");
        }
    }

    public interface Shape
    {
        void Draw();
    }
    public class Rectangle : Shape
    {
        public void Draw()
        {
            MLog.Print("Draw Rectangle");
        }
    }
    public class Circle : Shape
    {
        public void Draw()
        {
            MLog.Print("Draw Circle");
        }
    }
}
