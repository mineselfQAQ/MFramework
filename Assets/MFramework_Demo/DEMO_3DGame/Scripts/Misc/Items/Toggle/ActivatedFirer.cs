using UnityEngine;

public class ActivatedFirer : Toggle
{
    protected ParticleSystem m_fireParticle;
    protected BoxCollider m_collider;

    protected void Awake()
    {
        m_fireParticle = GetComponent<ParticleSystem>();
        m_collider = GetComponent<BoxCollider>();
    }

    protected override void OnActivateInternal()
    {
        m_fireParticle.Play();
        m_collider.enabled = true;
    }

    protected override void OnDeactivateInternal()
    {
        m_fireParticle.Stop();
        m_collider.enabled = false;
    }
}
