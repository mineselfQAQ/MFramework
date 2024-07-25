using UnityEngine;

public class CrouchPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.ResizeCollider(player.stats.current.crouchHeight);
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Fall();

        player.Decelerate(player.stats.current.crouchFriction);

        Vector3 inputDirection = player.inputs.GetMovementDirection();
        //�� ���ﰴס�¶����м� �� ������������
        if (player.inputs.GetCrouchAndCraw() || !player.canStandUp)
        {
            //������������ δ���� �� û�ڶ�����������״̬
            if (inputDirection.sqrMagnitude > 0 && !player.holding)
            {
                float speedMagnitude = player.lateralVelocity.sqrMagnitude;
                if (player.lateralVelocity.sqrMagnitude == 0)
                {
                    player.states.Change<CrawlingPlayerState>();
                }
            }
            //������Ծ�����ɺ�շ�
            else if (player.inputs.GetJumpDown())
            {
                player.Backflip(player.stats.current.backflipBackwardForce);
            }
        }
        else//����������Idle״̬
        {
            player.states.Change<IdlePlayerState>();
        }
    }

    protected override void OnExit(Player player)
    {
        player.ResizeCollider(player.originalHeight);
    }

    public override void OnContact(Player player, Collider other) { }
}
