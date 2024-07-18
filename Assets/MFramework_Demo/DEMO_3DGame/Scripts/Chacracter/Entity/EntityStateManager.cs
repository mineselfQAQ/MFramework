using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体状态控制器泛型类，也就是状态机
/// </summary>
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    protected List<EntityState<T>> m_list = new List<EntityState<T>>();
    protected Dictionary<Type, EntityState<T>> m_states = new Dictionary<Type, EntityState<T>>();

    public EntityState<T> current { get; protected set; }
    public EntityState<T> last { get; protected set; }

    public T entity { get; protected set; }

    public int index => m_list.IndexOf(current);
    public int lastIndex => m_list.IndexOf(last);

    /// <summary>
    /// 获取所有状态
    /// </summary>
    /// <returns>类型实例列表</returns>
    protected abstract List<EntityState<T>> GetStateList();

    protected virtual void Start()
    {
        InitializeEntity();
        InitializeStates();
    }

    /// <summary>
    /// 状态的Update
    /// </summary>
    public virtual void Step()
    {
        if (current != null && Time.timeScale > 0)
        {
            current.Step(entity);
        }
    }

    /// <summary>
    /// 状态下的接触事件
    /// </summary>
    public virtual void OnContact(Collider other)
    {
        if (current != null && Time.timeScale > 0)
        {
            current.OnContact(entity, other);
        }
    }

    protected virtual void InitializeEntity()
    {
        entity = GetComponent<T>();
    }
    protected virtual void InitializeStates()
    {
        m_list = GetStateList();//获取所有状态实例列表

        //将状态实例列表转存为字典
        foreach (var state in m_list)
        {
            var type = state.GetType();

            if (!m_states.ContainsKey(type))
            {
                m_states.Add(type, state);
            }
        }

        if (m_list.Count > 0)
        {
            current = m_list[0];//获取当前状态
        }
    }

    /// <summary>
    /// 切换状态(使用Inspector中顺序索引)
    /// </summary>
    public virtual void Change(int to)
    {
        if (to >= 0 && to < m_list.Count)
        {
            Change(m_list[to]);
        }
    }
    /// <summary>
    /// 切换状态(泛型)
    /// </summary>
    public virtual void Change<TState>() where TState : EntityState<T>
    {
        var type = typeof(TState);

        if (m_states.ContainsKey(type))
        {
            Change(m_states[type]);
        }
    }

    /// <summary>
    /// 切换状态(使用目标状态)
    /// </summary>
    public virtual void Change(EntityState<T> to)
    {
        if (to != null && Time.timeScale > 0)
        {
            //退出上一状态
            if (current != null)
            {
                current.Exit(entity);
                events.onExit.Invoke(current.GetType());
                last = current;
            }

            //进入新状态
            current = to;
            current.Enter(entity);
            events.onEnter.Invoke(current.GetType());
            events.onChange?.Invoke();
        }
    }

    public virtual bool IsCurrentOfType(Type type)
    {
        if (current == null)
        {
            return false;
        }

        return current.GetType() == type;
    }

    public virtual bool ContainsStateOfType(Type type)
    {
        return m_states.ContainsKey(type);
    }
}

public abstract class EntityStateManager : MonoBehaviour
{
    /// <summary>
    /// 公有可添加事件(所有状态可执行(可根据传入Type限制))
    /// </summary>
    /// Tip：内部使用更多，Inspector可直接传入，EntityStateManagerListener也可传入
    public EntityStateManagerEvents events;
}