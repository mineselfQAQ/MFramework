using UnityEngine;

public class AirDivePlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        //提供向前的力
        player.verticalVelocity = Vector3.zero;
        player.lateralVelocity = player.transform.forward * player.stats.current.airDiveForwardForce;
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.Jump();

        //TODO:???????????????????
        if (player.stats.current.applyDiveSlopeFactor)
            player.SlopeFactor(player.stats.current.slopeUpwardForce,
                player.stats.current.slopeDownwardForce);

        player.FaceDirection(player.lateralVelocity);
        
        //落地
        if (player.isGrounded)
        {
            Vector3 inputDirection = player.inputs.GetMovementCameraDirection();
            Vector3 localInputDirection = player.transform.InverseTransformDirection(inputDirection);//世界转本地
            float rotation = localInputDirection.x * player.stats.current.airDiveRotationSpeed * Time.deltaTime;

            //通过rotation旋转量旋转Player
            player.lateralVelocity = Quaternion.Euler(0, rotation, 0) * player.lateralVelocity;

            //TODO:???????????????????
            if (player.OnSlopingGround())
            {
                player.Decelerate(player.stats.current.airDiveSlopeFriction);
            }
            else
            {
                player.Decelerate(player.stats.current.airDiveFriction);

                //结束
                if (player.lateralVelocity.sqrMagnitude == 0)
                {
                    //小跳
                    player.verticalVelocity = Vector3.up * player.stats.current.airDiveGroundLeapHeight;
                    //空中需要进入Fall
                    player.states.Change<FallPlayerState>();
                }
            }
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other)
    {
        if (!player.isGrounded)
        {
            player.WallDrag(other);
            player.GrabPole(other);
        }
    }
}