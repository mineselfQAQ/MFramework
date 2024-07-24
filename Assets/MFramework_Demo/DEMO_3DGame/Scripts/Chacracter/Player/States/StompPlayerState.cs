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
        if (!m_falling)//�׶�1---�Ϳ�
        {
            m_airTimer += Time.deltaTime;

            if (m_airTimer >= player.stats.current.stompAirTime)
            {
                m_falling = true;
                player.playerEvents.OnStompFalling.Invoke();
            }
        }
        else//�׶�2---����
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

            if (m_groundTimer >= player.stats.current.stompGroundTime)//�׶�4---����
            {
                player.verticalVelocity = Vector3.up * player.stats.current.stompGroundLeapHeight;
                player.states.Change<FallPlayerState>();
            }
            else//�׶�3---���
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
