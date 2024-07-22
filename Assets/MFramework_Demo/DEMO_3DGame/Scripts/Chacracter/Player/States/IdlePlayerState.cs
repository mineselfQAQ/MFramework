using UnityEngine;

public class IdlePlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnExit(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();//空中情况(!isGrounded)
        player.SnapToGround();//地面情况(isGrounded)
        player.Jump();
        player.Fall();
        player.Spin();
        player.PickAndThrow();
        player.RegularSlopeFactor();
        player.Friction();

        var inputDirection = player.inputs.GetMovementDirection();

        if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
        {
            player.states.Change<WalkPlayerState>();
        }
        else if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other) { }
}