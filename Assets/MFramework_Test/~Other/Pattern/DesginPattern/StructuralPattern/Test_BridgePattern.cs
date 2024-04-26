using MFramework;
using UnityEngine;

public class Test_BridgePattern : MonoBehaviour
{
    private void Start()
    {
        Shape redCircle = new Circle(0, 0, 5, new RedCircle());
        redCircle.Draw();

        Shape greenCircle = new Circle(2, 3, 1, new GreenCircle());
        greenCircle.Draw();
    }

    public abstract class Shape
    {
        protected DrawAPI drawAPI;

        protected Shape(DrawAPI drawAPI)
        {
            this.drawAPI = drawAPI;
        }

        public abstract void Draw();
    }
    public class Circle : Shape
    {
        private int x;
        private int y;
        private int radius;

        public Circle(int x, int y, int radius, DrawAPI drawAPI) : base(drawAPI)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        public override void Draw()
        {
            drawAPI.DrawCircle(radius, x, y);
        }
    }

    public interface DrawAPI
    {
        void DrawCircle(int radius, int x, int y);
    }
    public class RedCircle : DrawAPI
    {
        public void DrawCircle(int radius, int x, int y)
        {
            MLog.Print($"RedCircle: Radius:{radius}, [{x},{y}]");
        }
    }
    public class GreenCircle : DrawAPI
    {
        public void DrawCircle(int radius, int x, int y)
        {
            MLog.Print($"GreenCircle: Radius:{radius}, [{x},{y}]");
        }
    }
}
