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
        if (inputDirection.sqrMagnitude > 0)//������
        {
            float dot = Vector3.Dot(inputDirection, player.lateralVelocity);//������ԭ����Խ����Խ��

            //������ɲ����ֵ
            if (dot >= player.stats.current.brakeThreshold)
            {
                //�ƶ�
                player.Accelerate(inputDirection);
                player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else//����ɲ����ֵ(�������Ƿ�����)
            {
                //����ɲ��״̬
                player.states.Change<BrakePlayerState>();
            }
        }
        else//δ����
        {
            //��������״̬
            player.Friction();
            if (player.lateralVelocity.sqrMagnitude <= 0)
            {
                player.states.Change<IdlePlayerState>();
            }
        }

        //�������׼����л���Crouch״̬(�׺����)
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
