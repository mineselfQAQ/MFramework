using System;
using UnityEngine.Events;

[Serializable]
public class EnemyEvents
{
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerStay;
    public UnityEvent OnPlayerExit;
    public UnityEvent OnPlayerContact;
    public UnityEvent OnDamage;
    public UnityEvent OnDie;
    public UnityEvent OnDead;
    public UnityEvent OnRevive;
}