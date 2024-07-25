using UnityEngine;

public class SpinPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        //空转情况
        if (!player.isGrounded)
        {
            //上旋
            player.verticalVelocity = Vector3.up * player.stats.current.airSpinUpwardForce;
        }
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.AirDive();
        player.StompAttack();

        player.AccelerateToInputDirection();

        //旋转时间已过
        if (timeSinceEntered >= player.stats.current.spinDuration)
        {
            if (player.isGrounded)//已落地
            {
                player.states.Change<IdlePlayerState>();
            }
            else//还在空中
            {
                player.states.Change<FallPlayerState>();
            }
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}
