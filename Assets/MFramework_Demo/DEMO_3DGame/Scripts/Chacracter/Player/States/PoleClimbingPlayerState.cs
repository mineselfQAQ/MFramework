using UnityEngine;

public class PoleClimbingPlayerState : PlayerState
{
    protected float m_collisionRadius;

    protected const float k_poleOffset = 0.01f;

    protected override void OnEnter(Player player)
    {
        player.ResetJumps();
        player.ResetAirSpins();
        player.ResetAirDash();
        player.velocity = Vector3.zero;

        player.pole.GetDirectionToPole(player.transform, out m_collisionRadius);
        //人物模型偏移
        player.skin.position += player.transform.rotation * player.stats.current.poleClimbSkinOffset;
    }

    protected override void OnStep(Player player)
    {
        Vector3 poleDirection = player.pole.GetDirectionToPole(player.transform);
        Vector3 inputDirection = player.inputs.GetMovementDirection();

        player.FaceDirection(poleDirection);//面向杆子

        //绕杆爬
        //TODO:有一个内容可以吸住物体(FaceDirection()去掉后还是会吸在杆子上)，不知道是哪个
        //Tip:这里只是简单的左右，需要配合FaceDirection()以及？？？完成
        player.lateralVelocity = player.transform.right * inputDirection.x * player.stats.current.climbRotationSpeed;
        //上下爬
        if (inputDirection.z != 0)
        {
            float speed = inputDirection.z > 0 ? player.stats.current.climbUpSpeed : -player.stats.current.climbDownSpeed;
            player.verticalVelocity = Vector3.up * speed;
        }
        else
        {
            player.verticalVelocity = Vector3.zero;
        }

        //按下跳跃，则进行蹬杆跳(Fall状态)
        if (player.inputs.GetJumpDown())
        {
            player.FaceDirection(-poleDirection);
            player.DirectionalJump(-poleDirection, player.stats.current.poleJumpHeight, player.stats.current.poleJumpDistance);
            player.states.Change<FallPlayerState>();
        }
        //滑到地上，进入Idle状态
        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }

        //通过杆子计算得出人物位置
        Vector3 center = new Vector3(player.pole.center.x, player.transform.position.y, player.pole.center.z);
        Vector3 position = center - poleDirection * m_collisionRadius;
        //防止Player越过杆子(当Player达到极限时，Player中心离杆顶或杆底还有半身距离)
        float offset = player.height * 0.5f + player.center.y;
        player.transform.position = player.pole.ClampPointToPoleHeight(position, offset);
    }

    protected override void OnExit(Player player)
    {
        //人物模型偏移
        player.skin.position -= player.transform.rotation * player.stats.current.poleClimbSkinOffset;
    }

    public override void OnContact(Player player, Collider other) { }
}
