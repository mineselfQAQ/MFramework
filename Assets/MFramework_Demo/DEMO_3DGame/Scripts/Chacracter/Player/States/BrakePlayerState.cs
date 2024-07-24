using UnityEngine;

public class BrakePlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        var inputDirection = player.inputs.GetMovementCameraDirection();

        //����շ�Ҫ��
        //1.����canBackflip
        //2.������Ծ��
        //3.���뷽�����ҷ����Ƿ����
        if (player.stats.current.canBackflip &&
            Vector3.Dot(inputDirection, player.transform.forward) < 0 &&
            player.inputs.GetJumpDown())
        {
            player.Backflip(player.stats.current.backflipBackwardTurnForce);
        }
        else
        {
            player.SnapToGround();
            player.Jump();
            player.Fall();
            player.Decelerate();//�ؼ�---����

            //���ٶȽ���0ʱ���л���Idle
            if (player.lateralVelocity.sqrMagnitude == 0)
            {
                player.states.Change<IdlePlayerState>();
            }
        }
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}
