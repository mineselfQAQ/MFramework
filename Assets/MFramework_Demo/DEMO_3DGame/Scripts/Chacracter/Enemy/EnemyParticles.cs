using UnityEngine;

public class EnemyParticles : EntityParticles
{
    public ParticleSystem deadDust;

    protected Enemy m_enemy;

    protected virtual void Start()
    {
        m_enemy = GetComponent<Enemy>();

        m_enemy.enemyEvents.OnDead.AddListener(OnDead);
    }

    protected virtual void OnDead()
    {
        Play(deadDust);
    }
}
