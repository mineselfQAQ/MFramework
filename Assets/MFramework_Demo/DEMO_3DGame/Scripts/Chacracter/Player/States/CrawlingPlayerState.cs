using UnityEngine;

public class CrawlingPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.ResizeCollider(player.stats.current.crouchHeight);
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Jump();
        player.Fall();

        Vector3 inputDirection = player.inputs.GetMovementCameraDirection();
        //�� ���ﰴס�¶����м� �� ������������
        if (player.inputs.GetCrouchAndCraw() || !player.canStandUp)
        {
            if (inputDirection.sqrMagnitude > 0)//������
            {
                player.CrawlingAccelerate(inputDirection);
                player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else
            {
                player.Decelerate(player.stats.current.crawlingFriction);
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