using UnityEngine;

public class CrouchPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.ResizeCollider(player.stats.current.crouchHeight);
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Fall();

        player.Decelerate(player.stats.current.crouchFriction);

        Vector3 inputDirection = player.inputs.GetMovementDirection();
        //当 人物按住下蹲爬行键 或 根本爬不起来
        if (player.inputs.GetCrouchAndCraw() || !player.canStandUp)
        {
            //输入后，如果人物 未持物 且 没在动，进入爬行状态
            if (inputDirection.sqrMagnitude > 0 && !player.holding)
            {
                float speedMagnitude = player.lateralVelocity.sqrMagnitude;
                if (player.lateralVelocity.sqrMagnitude == 0)
                {
                    player.states.Change<CrawlingPlayerState>();
                }
            }
            //输入跳跃键即可后空翻
            else if (player.inputs.GetJumpDown())
            {
                player.Backflip(player.stats.current.backflipBackwardForce);
            }
        }
        else//爬得起来就Idle状态
        {
            player.states.Change<IdlePlayerState>();
        }
    }

    protected override void OnExit(Player player)
    {
        player.ResizeCollider(player.originalHeight);
    }

    public override void OnContact(Player player, Collider other) { }
}
