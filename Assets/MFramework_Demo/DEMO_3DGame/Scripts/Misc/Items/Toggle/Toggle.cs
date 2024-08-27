using MFramework;
using UnityEngine;
using UnityEngine.Events;

public abstract class Toggle : MonoBehaviour, IToggle
{
    public bool state = true;
    public float delay;
     
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    public virtual void Set(bool value)
    {
        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            if (value)
            {
                if (!state)
                {
                    state = true;
                    onActivate?.Invoke();
                    OnActivateInternal();
                }
            }
            else if (state)
            {
                state = false;
                onDeactivate?.Invoke();
                OnDeactivateInternal();
            }
        }, delay);
    }

    protected abstract void OnActivateInternal();
    protected abstract void OnDeactivateInternal();
}
