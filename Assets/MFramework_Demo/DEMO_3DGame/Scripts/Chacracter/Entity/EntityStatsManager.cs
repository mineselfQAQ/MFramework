using UnityEngine;

public abstract class EntityStatsManager<T> : MonoBehaviour where T : EntityStats<T>
{
    public T[] stats;//EntityStats(һ��ScriptableObject���ݱ�)

    public T current { get; protected set; }

    protected virtual void Start()
    {
        if (stats.Length > 0)
        {
            current = stats[0];//Ĭ��ʹ�õ�һ�����ݱ�
        }
    }

    public virtual void Change(int to)
    {
        if (to >= 0 && to < stats.Length)
        {
            if (current != stats[to])
            {
                current = stats[to];
            }
        }
    }
}
