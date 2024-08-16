using UnityEngine;

public abstract class EntityParticles : MonoBehaviour
{
    /// <summary>
    /// ��������
    /// </summary>
    public virtual void Play(ParticleSystem particle)
    {
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }
    /// <summary>
    /// ֹͣ����
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
