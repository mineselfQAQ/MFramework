using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// 状态机中的状态泛型类
/// </summary>
public abstract class EntityState<T> where T : Entity<T>
{
    /// <summary>
    /// 进入该状态后的计时
    /// </summary>
    public float timeSinceEntered { get; protected set; }

    public void Enter(T entity)
    {
        timeSinceEntered = 0;
        OnEnter(entity);
    }
    public void Step(T entity)
    {
        timeSinceEntered += Time.deltaTime;
        OnStep(entity);
    }
    public void Exit(T entity)
    {
        OnExit(entity);
    }

    protected abstract void OnEnter(T entity);
    protected abstract void OnStep(T entity);
    protected abstract void OnExit(T entity);

    public abstract void OnContact(T entity, Collider other);

    /// <summary>
    /// 通过名字获取类型实例
    /// </summary>
    public static EntityState<T> CreateFromString(string typeName)
    {
        return (EntityState<T>)System.Activator
            .CreateInstance(System.Type.GetType(typeName));
    }
    /// <summary>
    /// 通过名字数组获取所有类型实例
    /// </summary>
    public static List<EntityState<T>> CreateListFromStringArray(string[] array)
    {
        var list = new List<EntityState<T>>();

        foreach (string typeName in array)
        {
            list.Add(CreateFromString(typeName));
        }

        return list;
    }
}