using System;
using UnityEngine.Events;

[Serializable]
public class EntityStateManagerEvents
{
    public UnityEvent onChange;

    [NonSerialized]
    public UnityEvent<Type> onEnter;

    [NonSerialized]
    public UnityEvent<Type> onExit;
}