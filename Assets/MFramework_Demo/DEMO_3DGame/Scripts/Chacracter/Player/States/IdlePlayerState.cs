using UnityEngine;

public class IdlePlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();//空中情况(!isGrounded)
        player.SnapToGround();//地面情况(isGrounded)
        player.Jump();//跳跃
        player.Fall();//离地即坠落
        player.Spin();//旋转动作
        player.PickAndThrow();//拾取与丢弃
        player.RegularSlopeFactor();//斜坡因子？？？
        player.Friction();//摩擦力

        Vector3 inputDirection = player.inputs.GetMovementDirection();
        //如果玩家有速度，切换至Walk状态
        if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
        {
            player.states.Change<WalkPlayerState>();
        }
        //如果输入蹲键，切换至Crouch状态(蹲后可爬)
        else if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}