public class ButtonActivatedSpike : Toggle
{
    protected Mover mover => GetComponentInChildren<Mover>();

    protected override void OnActivateInternal()
    {
        mover.Undo();
    }

    protected override void OnDeactivateInternal()
    {
        mover.Apply();
    }
}
