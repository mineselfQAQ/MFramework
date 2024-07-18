using UnityEngine;

public abstract class EntityStatsManager<T> : MonoBehaviour where T : EntityStats<T>
{
    public T[] stats;//EntityStats(一个ScriptableObject数据表)

    public T current { get; protected set; }

    protected virtual void Start()
    {
        if (stats.Length > 0)
        {
            current = stats[0];//默认使用第一个数据表
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
