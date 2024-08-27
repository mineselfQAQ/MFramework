public class ActivatedSpike : Toggle
{
    protected const string SPIKENAME = "Spikes";

    protected Mover m_mover;
    protected Mover Mover
    {
        get
        {
            if (m_mover == null)
            {
                m_mover = transform.Find(SPIKENAME).GetComponent<Mover>();
            }
            return m_mover;
        }
    }

    protected override void OnActivateInternal()
    {
        Mover.Undo();
    }

    protected override void OnDeactivateInternal()
    {
        Mover.Apply();
    }
}
