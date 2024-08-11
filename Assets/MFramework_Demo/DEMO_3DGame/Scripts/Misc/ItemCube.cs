using UnityEngine;
using UnityEngine.Events;

//TODO:BUG���ڲ������ȥ�Ļ��ᴥ�����OnEntityContact()
//TODO:�Ľ�������Ӧ��Ҳ���Ի�ȡ����
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
            //���ԣ�
            //�����ڲ���������(����������)�������أ�ʰȡʱ��ʾ
            //��������������(��������ȡ)������ͨ��Collectable�ű�����ʰȡ������ͨ��OnEntityContact()�Զ�ʰȡ
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
