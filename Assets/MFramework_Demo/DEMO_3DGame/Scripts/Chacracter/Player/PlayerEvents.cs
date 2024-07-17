using System;
using UnityEngine.Events;

[Serializable]
public class PlayerEvents
{
    public UnityEvent OnJump;

    public UnityEvent OnHurt;

    public UnityEvent OnDie;

    public UnityEvent OnSpin;

    public UnityEvent OnPickUp;

    public UnityEvent OnThrow;

    public UnityEvent OnStompStarted;

    public UnityEvent OnStompFalling;

    public UnityEvent OnStompLanding;

    public UnityEvent OnStompEnding;

    public UnityEvent OnLedgeGrabbed;

    public UnityEvent OnLedgeClimbing;

    public UnityEvent OnAirDive;

    public UnityEvent OnBackflip;

    public UnityEvent OnGlidingStart;

    public UnityEvent OnGlidingStop;

    public UnityEvent OnDashStarted;

    public UnityEvent OnDashEnded;
}
