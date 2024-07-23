using UnityEngine;

public class IdlePlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();//�������(!isGrounded)
        player.SnapToGround();//�������(isGrounded)
        player.Jump();//��Ծ
        player.Fall();//��ؼ�׹��
        player.Spin();//��ת����
        player.PickAndThrow();//ʰȡ�붪��
        player.RegularSlopeFactor();//б�����ӣ�����
        player.Friction();//Ħ����

        Vector3 inputDirection = player.inputs.GetMovementDirection();
        //���������ٶȣ��л���Walk״̬
        if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
        {
            player.states.Change<WalkPlayerState>();
        }
        //�������׼����л���Crouch״̬(�׺����)
        else if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}