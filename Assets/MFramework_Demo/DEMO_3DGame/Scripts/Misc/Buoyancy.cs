using UnityEngine;

/// <summary>
/// ��ˮ��ʱ�ṩ����
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    public float force = 10f;

    protected Rigidbody m_rigidbody;

    protected virtual void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTags.VolumeWater))
        {
            //�������ˮ��(���ĵ����ˮ��)
            if (transform.position.y < other.bounds.max.y)
            {
                float multiplier = Mathf.Clamp01((other.bounds.max.y - transform.position.y));
                Vector3 buoyancy = Vector3.up * force * multiplier;
                m_rigidbody.AddForce(buoyancy);//ʩ�Ӹ���
            }
        }
    }
}
