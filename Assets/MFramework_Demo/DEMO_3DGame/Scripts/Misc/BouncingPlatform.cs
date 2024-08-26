using MFramework;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class BouncingPlatform : MonoBehaviour, IEntityContact
{
    public float force = 25f;
    public AudioClip clip;

    protected AudioSource m_audio;
    protected Collider m_collider;

    protected virtual void Start()
    {
        tag = GameTags.Spring;
        m_collider = GetComponent<Collider>();
        m_audio = GetComponent<AudioSource>();
    }

    public void OnEntityContact(Entity entity)
    {
        //弹跳要求：
        //1.在蹦床上方 2.Player可用 3.Player活着
        if (entity.IsPointUnderStep(m_collider.bounds.max) &&
            entity is Player player && player.isAlive)
        {
            ApplyForce(player);

            player.SetJumps(1);
            player.ResetAirSpins();
            player.ResetAirDash();
            player.states.Change<FallPlayerState>();
        }
    }

    public void ApplyForce(Player player)
    {
        if (player.verticalVelocity.y <= 0)
        {
            m_audio.PlayOneShot(clip);
            player.verticalVelocity = Vector3.up * force;
        }
    }
}
