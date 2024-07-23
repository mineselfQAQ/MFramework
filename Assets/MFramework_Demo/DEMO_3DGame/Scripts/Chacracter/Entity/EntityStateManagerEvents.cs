using System;
using UnityEngine.Events;

[Serializable]
public class EntityStateManagerEvents
{
    public UnityEvent onChange;

    public UnityEvent<Type> onEnter;

    public UnityEvent<Type> onExit;
}