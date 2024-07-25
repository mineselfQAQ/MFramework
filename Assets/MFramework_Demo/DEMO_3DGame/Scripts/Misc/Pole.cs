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
    /// ��ȡ��һ�������嵽���ӵ�ˮƽ����
    /// </summary>
    public Vector3 GetDirectionToPole(Transform other) => GetDirectionToPole(other, out _);
    ///<summary>
    /// ��ȡ��һ�������嵽���ӵ�ˮƽ����
    /// </summary>
    /// <param name="distance">���嵽���ӵĳ���</param>
    public Vector3 GetDirectionToPole(Transform other, out float distance)
    {
        Vector3 target = new Vector3(center.x, other.position.y, center.z) - other.position;//���嵽���ӵ�ˮƽ����
        distance = target.magnitude;
        return target / distance;//��ȡ����(��һ����)
    }

    /// <summary>
    /// ��point�����ڸ��Ӹ߶���
    /// </summary>
    public Vector3 ClampPointToPoleHeight(Vector3 point, float offset)
    {
        float minHeight = collider.bounds.min.y + offset;
        float maxHeight = collider.bounds.max.y - offset;
        float clampedHeight = Mathf.Clamp(point.y, minHeight, maxHeight);
        return new Vector3(point.x, clampedHeight, point.z);
    }
}
