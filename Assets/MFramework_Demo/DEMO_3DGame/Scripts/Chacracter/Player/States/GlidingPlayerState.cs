using UnityEngine;

public class GlidingPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.verticalVelocity = Vector3.zero;
        player.playerEvents.OnGlidingStart.Invoke();
    }

    protected override void OnStep(Player player)
    {
        Vector3 inputDirection = player.inputs.GetMovementCameraDirection();

        GlidingGravity(player);
        player.FaceDirection(player.lateralVelocity);
        player.Accelerate(inputDirection, player.stats.current.glidingTurningDrag,
            player.stats.current.airAcceleration, player.stats.current.topSpeed);

        player.LedgeGrab();

        //落地进入Idle
        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }
        //松开滑翔键进入Fall
        else if (!player.inputs.GetGlide())
        {
            player.states.Change<FallPlayerState>();
        }
    }

    protected override void OnExit(Player player) =>
        player.playerEvents.OnGlidingStop.Invoke();

    public override void OnContact(Player player, Collider other)
    {
        player.WallDrag(other);
        player.GrabPole(other);
    }

    protected virtual void GlidingGravity(Player player)
    {
        float yVelocity = player.verticalVelocity.y;
        yVelocity -= player.stats.current.glidingGravity * Time.deltaTime;
        yVelocity = Mathf.Max(yVelocity, -player.stats.current.glidingMaxFallSpeed);
        player.verticalVelocity = new Vector3(0, yVelocity, 0);
    }
}