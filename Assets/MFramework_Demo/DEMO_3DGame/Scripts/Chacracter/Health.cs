using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;//��ʼ����ֵ
    public int max = 3;//�������ֵ
    public float coolDown = 1f;//��ȴʱ��(�޵�ʱ��)

    public UnityEvent onChange;
    public UnityEvent onDamage;

    protected int m_currentHealth;
    protected float m_lastDamageTime;

    public int current
    {
        get { return m_currentHealth; }

        protected set
        {
            var last = m_currentHealth;

            if (value != last)
            {
                m_currentHealth = Mathf.Clamp(value, 0, max);
                onChange?.Invoke();
            }
        }
    }

    public virtual void Reset()
    {
        current = initial;
    }

    protected virtual void Awake()
    {
        current = initial;
    }

    /// <summary>
    /// �Ƿ��Ѿ�����(����ֵΪ0)
    /// </summary>
    public virtual bool isEmpty => current == 0;

    /// <summary>
    /// �ָ�״̬(�����յ�����)
    /// </summary>
    public virtual bool recovering => Time.time < m_lastDamageTime + coolDown;

    public virtual void Set(int amount) => current = amount;

    public virtual void Increase(int amount) => current += amount;

    public virtual void Damage(int amount)
    {
        if (!recovering)
        {
            current -= Mathf.Abs(amount);
            m_lastDamageTime = Time.time;
            onDamage?.Invoke();
        }
    }
}