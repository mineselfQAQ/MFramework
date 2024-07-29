using UnityEngine;

public class SwimPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        //Ӧ������
        player.velocity *= player.stats.current.waterConversion;
    }

    protected override void OnStep(Player player)
    {
        if (player.onWater)
        {
            var inputDirection = player.inputs.GetMovementCameraDirection();
            //ˮ���ƶ�
            player.WaterAccelerate(inputDirection);
            player.WaterFaceDirectionSmooth(player.lateralVelocity);

            //Tip�������״̬��Ȼִ�й�EnterWater()����ô��ˮ���Ǳ�Ȼ��
            if (player.position.y < player.water.bounds.max.y)//������ˮ��
            {
                if (player.isGrounded)//ˮ�����
                {
                    player.verticalVelocity = Vector3.zero;
                }

                //ˮ�и���(PlayerƯ����ˮ��)
                player.verticalVelocity += Vector3.up * player.stats.current.waterUpwardsForce * Time.deltaTime;
            }
            else//��ˮ
            {
                player.verticalVelocity = Vector3.zero;

                if (player.inputs.GetJumpDown())
                {
                    player.Jump(player.stats.current.waterJumpHeight);
                    player.states.Change<FallPlayerState>();
                }
            }

            //��ǱҪ��
            //1.��û���� 2.���¸����
            if (!player.isGrounded && player.inputs.GetDive())
            {
                player.verticalVelocity += Vector3.down * player.stats.current.swimDiveForce * Time.deltaTime;
            }
            //�����룬��������
            if (inputDirection.sqrMagnitude == 0)
            {
                player.Decelerate(player.stats.current.swimDeceleration);
            }
        }
        else//��ˮ����Walk(Tip����ʱ���Ϊ�����߳�ˮ���)
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