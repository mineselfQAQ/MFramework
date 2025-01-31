using MFramework;
using UnityEngine;

[RequireComponent(typeof(MAudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Effects")]
    public AudioClip death;

    protected Enemy m_enemy;
    protected AudioSource m_audio;

    protected virtual void Start()
    {
        InitializeEnemy();
        InitializeAudio();
        InitializeCallbacks();
    }

    protected virtual void InitializeEnemy()
    {
        m_enemy = GetComponent<Enemy>();
    } 
    protected virtual void InitializeAudio()
    {
        m_audio = GetComponent<AudioSource>();
    }
    protected virtual void InitializeCallbacks()
    {
        m_enemy.enemyEvents.OnDie.AddListener(() => m_audio.PlayOneShot(death));
    }
}
