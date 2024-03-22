using UnityEngine;

[ExecuteAlways]
public class Test_Math : MonoBehaviour
{
    public Transform obj1;
    public Vector2 v = new Vector2(0, 1);
    [Range(0, 1)] public float t;

    private Vector3 oldPos;

    private void Start()
    {
        oldPos = obj1.position;
    }

    private void Update()
    {
        //位置+向量=新位置
        Vector3 newPos = oldPos + new Vector3(v.x, v.y);
        obj1.position = Vector3.LerpUnclamped(oldPos, newPos, t);
    }
}