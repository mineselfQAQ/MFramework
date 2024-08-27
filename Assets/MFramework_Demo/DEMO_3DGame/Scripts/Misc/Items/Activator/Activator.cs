using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 触发器，当Player或其它物体接触时发生事件
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public abstract class Activator : MonoBehaviour, IEntityContact
{
    public bool autoToggle;
    public bool requireStomp;
    public bool requirePlayer;
    public AudioClip activateClip;
    public AudioClip deactivateClip;

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    protected Collider m_collider;
    protected Collider m_entityActivator;
    protected Collider m_otherActivator;

    protected AudioSource m_audio;

    public bool activated { get; protected set; }

    protected virtual void Start()
    {
        gameObject.tag = GameTags.Activator;
        m_collider = GetComponent<Collider>();
        m_audio = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        //遇到接触物
        if (m_entityActivator || m_otherActivator)
        {
            Vector3 center = m_collider.bounds.center;
            float contactOffset = Physics.defaultContactOffset + 0.1f;
            Vector3 size = m_collider.bounds.size + Vector3.up * contactOffset;
            var bounds = new Bounds(center, size);

            bool intersectsEntity = m_entityActivator && bounds.Intersects(m_entityActivator.bounds);
            bool intersectsOther = m_otherActivator && bounds.Intersects(m_otherActivator.bounds);

            if (intersectsEntity || intersectsOther)//接触
            {
                Activate();
            }
            else//未接触(离开的一瞬间)
            {
                m_entityActivator = intersectsEntity ? m_entityActivator : null;
                m_otherActivator = intersectsOther ? m_otherActivator : null;

                if (autoToggle)
                {
                    Deactivate();
                }
            }
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        //检测非Player接触物体
        if (!(requirePlayer || requireStomp) && !collision.collider.CompareTag(GameTags.Player))
        {
            m_otherActivator = collision.collider;
        }
    }

    public void OnEntityContact(Entity entity)
    {
        //启动条件：
        //1.Entity在下落 2.Entity踩在上面
        //3.Entity必须是Player(除非!requirePlayer)
        //4.Entity必须是正在下砸的Player(除非!requireStomp)
        if (entity.velocity.y <= 0 && entity.IsPointUnderStep(m_collider.bounds.max))
        {
            if ((!requirePlayer || entity is Player) &&
                (!requireStomp || (entity as Player).states.IsCurrentOfType(typeof(StompPlayerState))))
            {
                m_entityActivator = entity.controller;
            }
        }
    }

    public virtual void Activate()
    {
        if (!activated)
        {
            if (activateClip)
            {
                m_audio.PlayOneShot(activateClip);
            }

            activated = true;
            OnActivate?.Invoke();
            OnActivateInternal();
        }
    }

    public virtual void Deactivate()
    {
        if (activated)
        {
            if (deactivateClip)
            {
                m_audio.PlayOneShot(deactivateClip);
            }

            activated = false;
            OnDeactivate?.Invoke();
            OnDeactivateInternal();
        }
    }

    public abstract void OnActivateInternal();
    public abstract void OnDeactivateInternal();
}
