using UnityEngine;

[RequireComponent(typeof(Rotator))]
public class FinishZone : Activator
{
    protected Rotator m_rotator => GetComponent<Rotator>();
    protected LevelFinisher m_finisher => LevelFinisher.Instance;

    public override void OnActivateInternal()
    {
        m_rotator.enabled = false;
        m_finisher.Finish();
    }

    public override void OnDeactivateInternal()
    {

    }
}
