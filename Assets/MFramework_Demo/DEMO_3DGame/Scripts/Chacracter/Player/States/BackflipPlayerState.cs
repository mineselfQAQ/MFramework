using UnityEngine;

public class BackflipPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.SetJumps(1);//jumpCounter = 1
        player.playerEvents.OnJump.Invoke();

        //锁定后空翻移动
        if (player.stats.current.backflipLockMovement)
        {
            player.inputs.LockMovementDirection();
        }
    }

    protected override void OnStep(Player player)
    {
        player.Gravity(player.stats.current.backflipGravity);

        Vector3 inputDirection = player.inputs.GetMovementCameraDirection();
        player.BackflipAccelerate(inputDirection);

        if (player.isGrounded)//落地
        {
            player.lateralVelocity = Vector3.zero;
            player.states.Change<IdlePlayerState>();
        }
        else if (player.verticalVelocity.y < 0)//下落
        {
            player.Spin();
            player.AirDive();
            player.StompAttack();
            player.Glide();
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
        player.WallDrag(other);
        player.GrabPole(other);
    }
}
