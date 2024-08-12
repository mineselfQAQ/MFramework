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
        //����ģ��ƫ��
        player.skin.position += player.transform.rotation * player.stats.current.poleClimbSkinOffset;
    }

    protected override void OnStep(Player player)
    {
        Vector3 poleDirection = player.pole.GetDirectionToPole(player.transform);
        Vector3 inputDirection = player.inputs.GetMovementDirection();

        player.FaceDirection(poleDirection);//�������

        //�Ƹ���
        //TODO:��һ�����ݿ�����ס����(FaceDirection()ȥ�����ǻ����ڸ�����)����֪�����ĸ�
        //Tip:����ֻ�Ǽ򵥵����ң���Ҫ���FaceDirection()�Լ����������
        player.lateralVelocity = player.transform.right * inputDirection.x * player.stats.current.climbRotationSpeed;
        //������
        if (inputDirection.z != 0)
        {
            float speed = inputDirection.z > 0 ? player.stats.current.climbUpSpeed : -player.stats.current.climbDownSpeed;
            player.verticalVelocity = Vector3.up * speed;
        }
        else
        {
            player.verticalVelocity = Vector3.zero;
        }

        //������Ծ������еŸ���(Fall״̬)
        if (player.inputs.GetJumpDown())
        {
            player.FaceDirection(-poleDirection);
            player.DirectionalJump(-poleDirection, player.stats.current.poleJumpHeight, player.stats.current.poleJumpDistance);
            player.states.Change<FallPlayerState>();
        }
        //�������ϣ�����Idle״̬
        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }

        //ͨ�����Ӽ���ó�����λ��
        Vector3 center = new Vector3(player.pole.center.x, player.transform.position.y, player.pole.center.z);
        Vector3 position = center - poleDirection * m_collisionRadius;
        //��ֹPlayerԽ������(��Player�ﵽ����ʱ��Player������˶���˵׻��а������)
        float offset = player.height * 0.5f + player.center.y;
        player.transform.position = player.pole.ClampPointToPoleHeight(position, offset);
    }

    protected override void OnExit(Player player)
    {
        //����ģ��ƫ��
        player.skin.position -= player.transform.rotation * player.stats.current.poleClimbSkinOffset;
    }

    public override void OnContact(Player player, Collider other) { }
}
