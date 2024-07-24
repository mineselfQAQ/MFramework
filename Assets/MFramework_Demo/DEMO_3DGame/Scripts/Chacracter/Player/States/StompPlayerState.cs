using UnityEngine;

public class StompPlayerState : PlayerState
{
    protected float m_airTimer;
    protected float m_groundTimer;

    protected bool m_falling;
    protected bool m_landed;

    protected override void OnEnter(Player player)
    {
        m_landed = false;
        m_falling = false;
        m_airTimer = 0;
        m_groundTimer = 0;
        player.velocity = Vector3.zero;
        player.playerEvents.OnStompStarted?.Invoke();
    }

    protected override void OnStep(Player player)
    {
        if (!m_falling)//½×¶Î1---ÖÍ¿Õ
        {
            m_airTimer += Time.deltaTime;

            if (m_airTimer >= player.stats.current.stompAirTime)
            {
                m_falling = true;
                player.playerEvents.OnStompFalling.Invoke();
            }
        }
        else//½×¶Î2---ÏÂÔÒ
        {
            player.verticalVelocity += Vector3.down * player.stats.current.stompDownwardForce;
        }

        if (player.isGrounded)
        {
            if (!m_landed)
            {
                m_landed = true;
                player.playerEvents.OnStompLanding?.Invoke();
            }

            if (m_groundTimer >= player.stats.current.stompGroundTime)//½×¶Î4---µ¯Æð
            {
                player.verticalVelocity = Vector3.up * player.stats.current.stompGroundLeapHeight;
                player.states.Change<FallPlayerState>();
            }
            else//½×¶Î3---ÂäµØ
            {
                m_groundTimer += Time.deltaTime;
            }
        }
    }

    protected override void OnExit(Player player)
    {
        player.playerEvents.OnStompEnding?.Invoke();
    }

    public override void OnContact(Player player, Collider other) { }
}
