using UnityEngine;

public class HurtPlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();

        if (player.isGrounded && (player.verticalVelocity.y <= 0))
        {
            if (player.health.current > 0)
            {
                player.states.Change<IdlePlayerState>();
            }
            else
            {
                player.states.Change<DiePlayerState>();
            }
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}
