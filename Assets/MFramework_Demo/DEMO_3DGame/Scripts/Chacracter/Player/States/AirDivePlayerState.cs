using UnityEngine;

public class AirDivePlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        //�ṩ��ǰ����
        player.verticalVelocity = Vector3.zero;
        player.lateralVelocity = player.transform.forward * player.stats.current.airDiveForwardForce;
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.Jump();

        //���˵�����
        if (player.stats.current.applyDiveSlopeFactor)
            player.SlopeFactor(player.stats.current.slopeUpwardForce,
                player.stats.current.slopeDownwardForce);

        player.FaceDirection(player.lateralVelocity);

        //���
        if (player.isGrounded)
        {
            Vector3 inputDirection = player.inputs.GetMovementCameraDirection();
            Vector3 localInputDirection = player.transform.InverseTransformDirection(inputDirection);//����ת����
            float rotation = localInputDirection.x * player.stats.current.airDiveRotationSpeed * Time.deltaTime;

            //ͨ��rotation��ת����תPlayer
            player.lateralVelocity = Quaternion.Euler(0, rotation, 0) * player.lateralVelocity;

            if (player.OnSlopingGround())//б��
            {
                player.Decelerate(player.stats.current.airDiveSlopeFriction);
            }
            else//��б��
            {
                player.Decelerate(player.stats.current.airDiveFriction);

                //����
                if (player.lateralVelocity.sqrMagnitude == 0)
                {
                    //С��
                    player.verticalVelocity = Vector3.up * player.stats.current.airDiveGroundLeapHeight;
                    //������Ҫ����Fall
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