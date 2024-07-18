using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;//初始生命值
    public int max = 3;//最大生命值
    public float coolDown = 1f;//冷却时间(无敌时间)

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
    /// 是否已经死亡(生命值为0)
    /// </summary>
    public virtual bool isEmpty => current == 0;

    /// <summary>
    /// 恢复状态(不可收到攻击)
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