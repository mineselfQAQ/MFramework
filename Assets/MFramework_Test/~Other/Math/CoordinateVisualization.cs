using UnityEngine;

[ExecuteAlways]
public class CoordinateVisualization : MonoBehaviour
{
    public int XAmount = 9;
    public int YAmount = 16;
    public float spacing = 1;
    public float width = 10;
    public float height = 10;

    private void Update()
    {
        for (int i = 0; i < XAmount; i++)
        {
            float x = i * spacing;
            Debug.DrawRay(new Vector2(x, 0), Vector2.up * width);
            Debug.DrawRay(new Vector2(x, 0), Vector2.down * width);
            Debug.DrawRay(new Vector2(-x, 0), Vector2.up * width);
            Debug.DrawRay(new Vector2(-x, 0), Vector2.down * width);
        }
        for (int i = 0; i < YAmount; i++)
        {
            float y = i * spacing;
            Debug.DrawRay(new Vector2(0, y), Vector2.left * height);
            Debug.DrawRay(new Vector2(0, y), Vector2.right * height);
            Debug.DrawRay(new Vector2(0, -y), Vector2.left * height);
            Debug.DrawRay(new Vector2(0, -y), Vector2.right * height);
        }
    }
}
