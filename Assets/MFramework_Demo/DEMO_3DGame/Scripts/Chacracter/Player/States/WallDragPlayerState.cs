using UnityEngine;

public class WallDragPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.ResetJumps();
        player.ResetAirSpins();
        player.ResetAirDash();
        player.velocity = Vector3.zero;
        Vector3 direction = player.lastWallNormal;
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        player.FaceDirection(direction);

        //人物模型偏移
        player.skin.position += player.transform.rotation * player.stats.current.wallDragSkinOffset;
    }

    protected override void OnStep(Player player)
    {
        //下滑
        player.verticalVelocity += Vector3.down * player.stats.current.wallDragGravity * Time.deltaTime;

        //滑到地上或者脱离墙面，则进入Idle状态
        if (player.isGrounded || !player.CapsuleCast(-player.transform.forward, player.radius))
        {
            player.states.Change<IdlePlayerState>();
        }
        //按下跳跃，则进行蹬墙跳(Fall状态)
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
        //人物模型偏移
        player.skin.position -= player.transform.rotation * player.stats.current.wallDragSkinOffset;

        //Platform Tag情况需要还原
        if (!player.isGrounded && player.transform.parent != null)
            player.transform.parent = null;
    }

    public override void OnContact(Player player, Collider other) { }
}