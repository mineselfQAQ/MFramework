public class Log : Activator
{
    protected Mover mover => GetComponent<Mover>();

    public override void OnActivateInternal()
    {
        mover.Apply();
    }

    public override void OnDeactivateInternal()
    {

    }
}
