using UnityEngine;

public class TriggerButton : Activator
{
    [Header("TriggerButton Settings")]
    /// <summary>
    /// 该触发器所控制的物体
    /// </summary>
    public Toggle[] toggles;

    protected const string BUTTONNAME = "Button";

    protected Mover m_mover;
    protected Mover Mover
    {
        get
        {
            if (m_mover == null)
            {
                m_mover = transform.Find(BUTTONNAME).GetComponent<Mover>();
            }
            return m_mover;
        }
    }

    public override void OnActivateInternal()
    {
        Mover.Apply();
        foreach (var toggle in toggles)
        {
            toggle.Set(false);
        }
    }

    public override void OnDeactivateInternal()
    {
        Mover.Undo();
        foreach (var toggle in toggles)
        {
            toggle.Set(true);
        }
    }
}
