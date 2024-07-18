using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 实体通过Inspector添加某状态onEnter/onExit事件
/// </summary>
public class EntityStateManagerListener : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public List<string> states;

    protected EntityStateManager m_manager;

    protected virtual void Start()
    {
        if (!m_manager)
        {
            m_manager = GetComponentInParent<EntityStateManager>();
        }

        //将事件整合至EntityStateManager的events中
        m_manager.events.onEnter.AddListener(OnEnter);
        m_manager.events.onExit.AddListener(OnExit);
    }

    protected virtual void OnEnter(Type state)
    {
        if (states.Contains(state.Name))
        {
            onEnter.Invoke();
        }
    }

    protected virtual void OnExit(Type state)
    {
        if (states.Contains(state.Name))
        {
            onExit.Invoke();
        }
    }
}
