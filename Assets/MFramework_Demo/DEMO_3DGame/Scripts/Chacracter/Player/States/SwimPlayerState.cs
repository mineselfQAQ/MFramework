using UnityEngine;

public class SwimPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        //应用阻力
        player.velocity *= player.stats.current.waterConversion;
    }

    protected override void OnStep(Player player)
    {
        if (player.onWater)
        {
            var inputDirection = player.inputs.GetMovementCameraDirection();
            //水中移动
            player.WaterAccelerate(inputDirection);
            player.WaterFaceDirectionSmooth(player.lateralVelocity);

            //Tip：进入该状态必然执行过EnterWater()，那么在水中是必然的
            if (player.position.y < player.water.bounds.max.y)//还在入水中
            {
                if (player.isGrounded)//水中落地
                {
                    player.verticalVelocity = Vector3.zero;
                }

                //水中浮力(Player漂浮在水面)
                player.verticalVelocity += Vector3.up * player.stats.current.waterUpwardsForce * Time.deltaTime;
            }
            else//出水
            {
                player.verticalVelocity = Vector3.zero;

                if (player.inputs.GetJumpDown())
                {
                    player.Jump(player.stats.current.waterJumpHeight);
                    player.states.Change<FallPlayerState>();
                }
            }

            //下潜要求：
            //1.还没沉底 2.按下俯冲键
            if (!player.isGrounded && player.inputs.GetDive())
            {
                player.verticalVelocity += Vector3.down * player.stats.current.swimDiveForce * Time.deltaTime;
            }
            //无输入，慢慢减速
            if (inputDirection.sqrMagnitude == 0)
            {
                player.Decelerate(player.stats.current.swimDeceleration);
            }
        }
        else//出水进入Walk(Tip：此时大概为岸边走出水情况)
        {
            player.states.Change<WalkPlayerState>();
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}