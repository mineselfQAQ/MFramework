using UnityEngine;

public abstract class EntityParticles : MonoBehaviour
{
    /// <summary>
    /// ²¥·ÅÁ£×Ó
    /// </summary>
    public virtual void Play(ParticleSystem particle)
    {
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }
    /// <summary>
    /// Í£Ö¹Á£×Ó
    /// </summary>
    public virtual void Stop(ParticleSystem particle, bool clear = false)
    {
        if (particle.isPlaying)
        {
            var mode = clear ? ParticleSystemStopBehavior.StopEmittingAndClear :
                ParticleSystemStopBehavior.StopEmitting;
            particle.Stop(true, mode);
        }
    }
}
