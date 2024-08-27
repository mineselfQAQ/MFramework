using MFramework;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallingPlatform : MonoBehaviour, IEntityContact
{
    public bool autoReset = true;
    public float fallDelay = 2f;
    public float resetDelay = 5f;
    public float fallGravity = 40f;

    [Header("Shake Setting")]
    public bool shake = true;
    public float speed = 45f;
    public float height = 0.1f;

    protected Collider m_collider;
    protected Vector3 m_initialPosition;

    protected Collider[] m_overlaps = new Collider[32];

    public bool activated { get; protected set; }
    public bool falling { get; protected set; }

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (falling)
        {
            transform.position += fallGravity * Vector3.down * Time.deltaTime;
        }
    }
    public void OnEntityContact(Entity entity)
    {
        //����׹��Ҫ��
        //1.��Player 2.��ƽ̨��
        if (entity is Player && entity.IsPointUnderStep(m_collider.bounds.max))
        {
            if (!activated)
            {
                activated = true;

                //��0.5*FallDelay��ʼ��
                //��1*FallDelay��׹��
                //��׹���1*resetDelay����������
                MTween.DoTween01NoRecord((f) =>
                {
                    if (shake && (f >= 0.5f))
                    {
                        float shake = Mathf.Sin(Time.time * speed) * height;//�����
                        transform.position = m_initialPosition + Vector3.up * shake;
                    }
                }, MCurve.Linear, fallDelay, () =>
                {
                    Fall();

                    if (autoReset)
                    {
                        MCoroutineManager.Instance.DelayNoRecord(() =>
                        {
                            Restart();
                        }, resetDelay);
                    }
                });
            }
        }
    }

    public virtual void Fall()
    {
        falling = true;
        m_collider.isTrigger = true;
    }

    public virtual void Restart()
    {
        activated = false;
        falling = false;
        transform.position = m_initialPosition;
        m_collider.isTrigger = false;
        OffsetPlayer();
    }

    /// <summary>
    /// ���Player������Collider�У�ƫ����ƽ̨�Ϸ�
    /// </summary>
    protected virtual void OffsetPlayer()
    {
        //����Ƿ���ײ
        Vector3 center = m_collider.bounds.center;
        Vector3 extents = m_collider.bounds.extents;
        float maxY = m_collider.bounds.max.y;
        int overlaps = Physics.OverlapBoxNonAlloc(center, extents, m_overlaps);

        for (int i = 0; i < overlaps; i++)
        {
            if (!m_overlaps[i].CompareTag(GameTags.Player)) continue;

            //������Ӧ��
            float distance = maxY - m_overlaps[i].transform.position.y;
            float height = m_overlaps[i].GetComponent<Player>().height;
            Vector3 offset = Vector3.up * (distance + height * 0.5f);

            m_overlaps[i].transform.position += offset;
        }
    }
}
