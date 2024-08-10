using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hazard : MonoBehaviour, IEntityContact
{
    public bool damageOnlyFromAbove;
    public int damage = 1;

    protected Collider m_collider;

    protected virtual void Awake()
    {
        tag = GameTags.Hazard;
        m_collider = GetComponent<Collider>();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                TryApplyDamageTo(player);
            }
        }
    }

    public virtual void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            TryApplyDamageTo(player);
        }
    }

    protected virtual void TryApplyDamageTo(Player player)
    {
        //检测是否是在上侧发生碰撞
        if (!damageOnlyFromAbove || player.velocity.y <= 0 &&
            player.IsPointUnderStep(m_collider.bounds.max))
        {
            player.ApplyDamage(damage, transform.position);
        }
    }
}
