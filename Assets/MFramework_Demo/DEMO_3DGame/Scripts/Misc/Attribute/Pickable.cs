using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ʰȡ����
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour
{
    [Header("General Settings")]
    public Vector3 offset;//����ʰȡ��λ�õ�ƫ��
    public float releaseOffset = 0.5f;//����ʱ��ƫ��

    [Header("Respawn Settings")]
    public bool autoRespawn;//�Ƿ����������������
    public bool respawnOnHitHazards;//�Ƿ��ٴ���Σ����ʱ��������
    public float respawnHeightLimit = -100;//�������ø߶�(���Ķ��)

    [Header("Attack Settings")]
    public bool attackEnemies = true;
    public int damage = 1;
    public float minDamageSpeed = 5f;

    [Space(15)]

    public UnityEvent onPicked;
    public UnityEvent onReleased;
    public UnityEvent onRespawn;

    protected Collider m_collider;
    protected Rigidbody m_rigidBody;

    protected Vector3 m_initialPosition;
    protected Quaternion m_initialRotation;
    protected Transform m_initialParent;

    protected RigidbodyInterpolation m_interpolation;

    public bool beingHold { get; protected set; }

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_initialPosition = transform.localPosition;
        m_initialRotation = transform.localRotation;
        m_initialParent = transform.parent;
    }

    protected virtual void Update()
    {
        //�����忪��autoRespawn(��Ҫ���壬���ɶ���)�����ұ���������ͼ��ʱ������
        if (autoRespawn && transform.position.y <= respawnHeightLimit)
        {
            Respawn();
        }
    }

    protected virtual void OnTriggerEnter(Collider other) =>
        EvaluateHazardRespawn(other);
    protected virtual void OnCollisionEnter(Collision collision) =>
        EvaluateHazardRespawn(collision.collider);

    public virtual void PickUp(Transform slot)
    {
        if (!beingHold)
        {
            beingHold = true;
            transform.parent = slot;
            transform.localPosition = Vector3.zero + offset;
            transform.localRotation = Quaternion.identity;
            m_rigidBody.isKinematic = true;
            //m_collider.isTrigger = true;
            m_collider.enabled = false;
            m_interpolation = m_rigidBody.interpolation;
            m_rigidBody.interpolation = RigidbodyInterpolation.None;
            onPicked?.Invoke();
        }
    }

    public virtual void Release(Vector3 direction, float force)
    {
        if (beingHold)
        {
            transform.parent = null;
            transform.position += direction * releaseOffset;//�ӳ�ȥһ�̵�ƫ��
            //m_collider.isTrigger = false;
            m_collider.enabled = true;
            m_rigidBody.isKinematic = false;
            beingHold = false;
            m_rigidBody.interpolation = m_interpolation;
            m_rigidBody.velocity = direction * force;//�������ٶ�
            onReleased?.Invoke();
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Respawn()
    {
        //��һ������Ϊ��ʼ״̬
        transform.parent = m_initialParent;
        transform.SetPositionAndRotation(m_initialPosition, m_initialRotation);
        m_rigidBody.isKinematic = false;
        m_rigidBody.velocity = Vector3.zero;
        m_collider.isTrigger = false;
        beingHold = false;
        onRespawn?.Invoke();
    }

    /// <summary>
    /// ���е���ʱ����˺�
    /// </summary>
    /// <param Name="entity"></param>
    public void OnEnemyHit(Entity entity)
    {
        //Ҫ��1.��Enemy 2.�����ɹ������� 3.�ٶȴ�������޶�
        if (attackEnemies && entity is Enemy &&
            m_rigidBody.velocity.magnitude > minDamageSpeed)
        {
            entity.ApplyDamage(damage, transform.position);
        }
    }

    protected virtual void EvaluateHazardRespawn(Collider other)
    {
        //�����忪��autoRespawn/respawnOnHitHazards(��Ҫ���壬���ɶ���)��
        //������Σ����ʱ��Ҫ����
        if (autoRespawn && respawnOnHitHazards && other.CompareTag(GameTags.Hazard))
        {
            Respawn();
        }
    }
}

