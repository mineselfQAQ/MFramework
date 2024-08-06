using UnityEngine;

[RequireComponent(typeof(Rotator))]
public class FinishZone : Activator
{
    protected Rotator rotator => GetComponent<Rotator>();
    protected LevelFinisher m_finisher => LevelFinisher.Instance;

    public override void OnActivateInternal()
    {
        rotator.enabled = false;
        m_finisher.Finish();
    }

    public override void OnDeactivateInternal()
    {

    }
}
