using UnityEngine;

public class WalkPlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Jump();
        player.Fall();
        player.Spin();
        player.PickAndThrow();
        player.Dash();
        player.RegularSlopeFactor();

        Vector3 inputDirection = player.inputs.GetMovementCameraDirection();
        if (inputDirection.sqrMagnitude > 0)//有输入
        {
            float dot = Vector3.Dot(inputDirection, player.lateralVelocity);//输入与原方向越相似越大

            //不超过刹车阈值
            if (dot >= player.stats.current.brakeThreshold)
            {
                //移动
                player.Accelerate(inputDirection);
                player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else//超过刹车阈值(几乎就是反向了)
            {
                //进入刹车状态
                player.states.Change<BrakePlayerState>();
            }
        }
        else//未输入
        {
            //进入闲置状态
            player.Friction();
            if (player.lateralVelocity.sqrMagnitude <= 0)
            {
                player.states.Change<IdlePlayerState>();
            }
        }

        //如果输入蹲键，切换至Crouch状态(蹲后可爬)
        if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}
