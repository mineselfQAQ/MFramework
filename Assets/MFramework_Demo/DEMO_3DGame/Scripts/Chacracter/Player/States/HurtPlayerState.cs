using UnityEngine;

public class HurtPlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();//受击后自然落地

        if (player.isGrounded && (player.verticalVelocity.y <= 0))
        {
            if (player.health.current > 0)//有血恢复Idle
            {
                player.states.Change<IdlePlayerState>();
            }
            else//没血就Die
            {
                player.states.Change<DiePlayerState>();
            }
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}
