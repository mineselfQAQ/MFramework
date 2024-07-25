using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Pole : MonoBehaviour
{
    public new CapsuleCollider collider { get; protected set; }

    public float radius => collider.radius;

    public Vector3 center => transform.position;

    protected virtual void Awake()
    {
        tag = GameTags.Pole;
        collider = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// 获取归一化的物体到杆子的水平方向
    /// </summary>
    public Vector3 GetDirectionToPole(Transform other) => GetDirectionToPole(other, out _);
    ///<summary>
    /// 获取归一化的物体到杆子的水平方向
    /// </summary>
    /// <param name="distance">物体到杆子的长度</param>
    public Vector3 GetDirectionToPole(Transform other, out float distance)
    {
        Vector3 target = new Vector3(center.x, other.position.y, center.z) - other.position;//物体到杆子的水平向量
        distance = target.magnitude;
        return target / distance;//获取方向(归一化后)
    }

    /// <summary>
    /// 将point限制在杆子高度中
    /// </summary>
    public Vector3 ClampPointToPoleHeight(Vector3 point, float offset)
    {
        float minHeight = collider.bounds.min.y + offset;
        float maxHeight = collider.bounds.max.y - offset;
        float clampedHeight = Mathf.Clamp(point.y, minHeight, maxHeight);
        return new Vector3(point.x, clampedHeight, point.z);
    }
}
