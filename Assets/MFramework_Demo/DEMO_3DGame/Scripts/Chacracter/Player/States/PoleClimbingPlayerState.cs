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
        player.skin.position += player.transform.rotation * player.stats.current.poleClimbSkinOffset;
    }

    protected override void OnStep(Player player)
    {
        var poleDirection = player.pole.GetDirectionToPole(player.transform);
        var inputDirection = player.inputs.GetMovementDirection();

        player.FaceDirection(poleDirection);
        player.lateralVelocity = player.transform.right * inputDirection.x * player.stats.current.climbRotationSpeed;

        if (inputDirection.z != 0)
        {
            var speed = inputDirection.z > 0 ? player.stats.current.climbUpSpeed : -player.stats.current.climbDownSpeed;
            player.verticalVelocity = Vector3.up * speed;
        }
        else
        {
            player.verticalVelocity = Vector3.zero;
        }

        if (player.inputs.GetJumpDown())
        {
            player.FaceDirection(-poleDirection);
            player.DirectionalJump(-poleDirection, player.stats.current.poleJumpHeight, player.stats.current.poleJumpDistance);
            player.states.Change<FallPlayerState>();
        }

        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }

        var offset = player.height * 0.5f + player.center.y;
        var center = new Vector3(player.pole.center.x, player.transform.position.y, player.pole.center.z);
        var position = center - poleDirection * m_collisionRadius;

        player.transform.position = player.pole.ClampPointToPoleHeight(position, offset);
    }

    protected override void OnExit(Player player)
    {
        player.skin.position -= player.transform.rotation * player.stats.current.poleClimbSkinOffset;
    }

    public override void OnContact(Player player, Collider other) { }
}
