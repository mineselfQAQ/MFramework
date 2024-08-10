using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class ItemBox : MonoBehaviour, IEntityContact
{
    public Collectable[] collectables;
    public MeshRenderer itemBoxRenderer;
    public Material emptyItemBoxMaterial;

    [Space(15)]
    public UnityEvent onCollect;
    public UnityEvent onDisable;

    protected int m_index;
    protected bool m_enabled = true;

    protected BoxCollider m_collider;

    protected virtual void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        InitializeCollectables();
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            if (entity.velocity.y > 0 && entity.position.y < m_collider.bounds.min.y)
            {
                Collect(player);
            }
        }
    }

    protected virtual void InitializeCollectables()
    {
        foreach (var collectable in collectables)
        {
            //���ԣ�
            //�����ڲ��������壬�����أ�ʰȡʱ��ʾ
            //�������������壬����ͨ��Collectable�ű�����ʰȡ������ͨ��OnEntityContact()����ʰȡ
            if (!collectable.hidden)
            {
                collectable.gameObject.SetActive(false);
            }
            else
            {
                collectable.autoCollect = false;
            }
        }
    }

    public virtual void Collect(Player player)
    {
        if (m_enabled)
        {
            if (m_index < collectables.Length)
            {
                if (collectables[m_index].hidden)
                {
                    collectables[m_index].Collect(player);
                }
                else
                {
                    collectables[m_index].gameObject.SetActive(true);
                }

                m_index++;
                onCollect?.Invoke();
            }

            if (m_index == collectables.Length)
            {
                Disable();
            }
        }
    }

    public virtual void Disable()
    {
        if (m_enabled)
        {
            m_enabled = false;
            itemBoxRenderer.sharedMaterial = emptyItemBoxMaterial;
            onDisable?.Invoke();
        }
    }
}
