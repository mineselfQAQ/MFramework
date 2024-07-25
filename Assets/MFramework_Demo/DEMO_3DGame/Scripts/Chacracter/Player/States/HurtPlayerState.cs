using UnityEngine;

public class HurtPlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();//�ܻ�����Ȼ���

        if (player.isGrounded && (player.verticalVelocity.y <= 0))
        {
            if (player.health.current > 0)//��Ѫ�ָ�Idle
            {
                player.states.Change<IdlePlayerState>();
            }
            else//ûѪ��Die
            {
                player.states.Change<DiePlayerState>();
            }
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}
