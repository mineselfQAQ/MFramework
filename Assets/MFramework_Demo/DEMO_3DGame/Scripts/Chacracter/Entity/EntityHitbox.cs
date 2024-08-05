using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EntityHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    public bool breakObjects;
    public int damage = 1;

    [Header("Rebound Settings")]
    public bool rebound;
    public float reboundMinForce = 10f;
    public float reboundMaxForce = 25f;

    [Header("Push Back Settings")]
    public bool pushBack;
    public float pushBackMinMagnitude = 5f;
    public float pushBackMaxMagnitude = 10f;

    protected Entity m_entity;
    protected Collider m_collider;

    protected virtual void Start()
    {
        InitializeEntity();
        InitializeCollider();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
        HandleCustomCollision(other);
    }

    protected virtual void InitializeEntity()
    {
        if (!m_entity)
        {
            m_entity = GetComponentInParent<Entity>();
        }
    }
    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    protected virtual void HandleCollision(Collider other)
    {
        if (other != m_entity.controller)//排除自身Controller
        {
            if (other.TryGetComponent(out Entity target))//1.敌人
            {
                HandleEntityAttack(target);

                HandleRebound();
                HandlePushBack();
            }
            else if (other.TryGetComponent(out Breakable breakable))//2.可击破物体
            {
                HandleBreakableObject(breakable);
            }
        }
    }
    /// <summary>
    /// 造成伤害
    /// </summary>
    protected virtual void HandleEntityAttack(Entity other)
    {
        other.ApplyDamage(damage, transform.position);
    }
    /// <summary>
    /// 击中后弹起
    /// </summary>
    protected virtual void HandleRebound()
    {
        if (rebound)
        {
            float force = -m_entity.velocity.y;
            force = Mathf.Clamp(force, reboundMinForce, reboundMaxForce);
            m_entity.verticalVelocity = Vector3.up * force;
        }
    }
    /// <summary>
    /// 击中后后退
    /// </summary>
    protected virtual void HandlePushBack()
    {
        if (pushBack)
        {
            var force = m_entity.lateralVelocity.magnitude;
            force = Mathf.Clamp(force, pushBackMinMagnitude, pushBackMaxMagnitude);
            m_entity.lateralVelocity = -transform.forward * force;
        }
    }

    protected virtual void HandleBreakableObject(Breakable breakable)
    {
        if (breakObjects)
        {
            breakable.Break();
        }
    }

    protected virtual void HandleCustomCollision(Collider other) { }
}
