using UnityEngine;

public class WallDragPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.ResetJumps();
        player.ResetAirSpins();
        player.ResetAirDash();
        player.velocity = Vector3.zero;
        var direction = player.lastWallNormal;
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        player.FaceDirection(direction);
        player.skin.position += player.transform.rotation * player.stats.current.wallDragSkinOffset;
    }

    protected override void OnStep(Player player)
    {
        player.verticalVelocity += Vector3.down * player.stats.current.wallDragGravity * Time.deltaTime;

        if (player.isGrounded || !player.CapsuleCast(-player.transform.forward, player.radius))
        {
            player.states.Change<IdlePlayerState>();
        }
        else if (player.inputs.GetJumpDown())
        {
            if (player.stats.current.wallJumpLockMovement)
            {
                player.inputs.LockMovementDirection();
            }

            player.DirectionalJump(player.transform.forward, player.stats.current.wallJumpHeight, player.stats.current.wallJumpDistance);
            player.states.Change<FallPlayerState>();
        }
    }

    protected override void OnExit(Player player)
    {
        player.skin.position -= player.transform.rotation * player.stats.current.wallDragSkinOffset;

        if (!player.isGrounded && player.transform.parent != null)
            player.transform.parent = null;
    }

    public override void OnContact(Player player, Collider other) { }
}