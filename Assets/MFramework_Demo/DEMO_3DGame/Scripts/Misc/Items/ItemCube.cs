using UnityEngine;
using UnityEngine.Events;

//TODO:BUG：在侧面蹭上去的话会触发多次OnEntityContact()
//TODO:改进：下砸应该也可以获取物体
[RequireComponent(typeof(BoxCollider))]
public class ItemCube : MonoBehaviour, IEntityContact
{
    public Material emptyItemCubeMaterial;


    [Space(15)]
    public UnityEvent onCollect;
    public UnityEvent onDisable;

    protected int m_index;
    protected bool m_enabled = true;

    protected Collectable[] m_collectables;
    protected BoxCollider m_collider;
    protected MeshRenderer m_itemCubeRenderer;

    protected virtual void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        m_itemCubeRenderer = transform.Find("ItemCubeModel").GetComponent<MeshRenderer>();

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
        Transform items = transform.Find("Items");
        m_collectables = items.GetComponentsInChildren<Collectable>(true);

        foreach (var collectable in m_collectables)
        {
            //策略：
            //·对于不隐藏物体(顶开即弹出)，先隐藏，拾取时显示
            //·对于隐藏物体(顶开即获取)，不可通过Collectable脚本控制拾取，必须通过OnEntityContact()自动拾取
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
            if (m_index < m_collectables.Length)
            {
                if (!m_collectables[m_index].hidden)
                {
                    m_collectables[m_index].gameObject.SetActive(true);
                }
                else
                {
                    m_collectables[m_index].Collect(player);
                }

                m_index++;
                onCollect?.Invoke();
            }

            if (m_index == m_collectables.Length)
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
            m_itemCubeRenderer.sharedMaterial = emptyItemCubeMaterial;
            onDisable?.Invoke();
        }
    }
}
