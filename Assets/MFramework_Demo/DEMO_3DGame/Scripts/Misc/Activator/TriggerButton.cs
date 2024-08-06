using UnityEngine;

//TODO:触发略有问题，连跳会导致刺不正常弹出
public class TriggerButton : Activator
{
    [Header("TriggerButton Settings")]
    public Toggle[] toggles;
    public Mover mover;

    public override void OnActivateInternal()
    {
        mover.Apply();
        foreach (var toggle in toggles)
        {
            toggle.Set(false);
        }
    }

    public override void OnDeactivateInternal()
    {
        mover.Undo();
        foreach (var toggle in toggles)
        {
            toggle.Set(true);
        }
    }
}
