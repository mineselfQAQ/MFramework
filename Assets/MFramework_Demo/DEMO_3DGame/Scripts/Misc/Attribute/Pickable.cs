using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 可拾取功能
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour
{
    [Header("General Settings")]
    public Vector3 offset;//物体拾取后位置的偏移
    public float releaseOffset = 0.5f;//丢出时的偏移

    [Header("Respawn Settings")]
    public bool autoRespawn;//是否意外情况重置物体
    public bool respawnOnHitHazards;//是否再触碰危险区时重置物体
    public float respawnHeightLimit = -100;//触发重置高度(掉的多低)

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
        //当物体开启autoRespawn(重要物体，不可丢弃)，而且被丢弃至地图外时，重置
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
            transform.position += direction * releaseOffset;//扔出去一刻的偏移
            //m_collider.isTrigger = false;
            m_collider.enabled = true;
            m_rigidBody.isKinematic = false;
            beingHold = false;
            m_rigidBody.interpolation = m_interpolation;
            m_rigidBody.velocity = direction * force;//真正的速度
            onReleased?.Invoke();
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    public virtual void Respawn()
    {
        //将一切设置为初始状态
        transform.parent = m_initialParent;
        transform.SetPositionAndRotation(m_initialPosition, m_initialRotation);
        m_rigidBody.isKinematic = false;
        m_rigidBody.velocity = Vector3.zero;
        m_collider.isTrigger = false;
        beingHold = false;
        onRespawn?.Invoke();
    }

    /// <summary>
    /// 击中敌人时造成伤害
    /// </summary>
    /// <param Name="entity"></param>
    public void OnEnemyHit(Entity entity)
    {
        //要求：1.是Enemy 2.开启可攻击敌人 3.速度大于最低限度
        if (attackEnemies && entity is Enemy &&
            m_rigidBody.velocity.magnitude > minDamageSpeed)
        {
            entity.ApplyDamage(damage, transform.position);
        }
    }

    protected virtual void EvaluateHazardRespawn(Collider other)
    {
        //当物体开启autoRespawn/respawnOnHitHazards(重要物体，不可丢弃)，
        //触碰到危险物时需要重置
        if (autoRespawn && respawnOnHitHazards && other.CompareTag(GameTags.Hazard))
        {
            Respawn();
        }
    }
}

