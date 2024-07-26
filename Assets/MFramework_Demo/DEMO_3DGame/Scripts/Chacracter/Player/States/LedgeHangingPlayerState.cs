using MFramework;
using System.Collections;
using UnityEngine;

public class LedgeHangingPlayerState : PlayerState
{
    protected bool m_keepParent;
    protected Coroutine m_clearParentRoutine;

    protected const float k_clearParentDelay = 0.25f;

    protected override void OnEnter(Player player)
    {
        if (m_clearParentRoutine != null)
            MCoroutineManager.Instance.StopCoroutine(m_clearParentRoutine);

        m_keepParent = false;
        //人物模型偏移
        player.skin.position += player.transform.rotation * player.stats.current.ledgeHangingSkinOffset;
        player.ResetJumps();
        player.ResetAirSpins();
        player.ResetAirDash();
    }

    protected override void OnStep(Player player)
    {
        float ledgeTopMaxDistance = player.radius + player.stats.current.ledgeMaxForwardDistance;
        float ledgeTopHeightOffset = player.height * 0.5f + player.stats.current.ledgeMaxDownwardDistance;
        Vector3 topOrigin = player.position + Vector3.up * ledgeTopHeightOffset + player.transform.forward * ledgeTopMaxDistance;
        Vector3 sideOrigin = player.position + Vector3.up * player.height * 0.5f + Vector3.down * player.stats.current.ledgeSideHeightOffset;
        float rayDistance = player.radius + player.stats.current.ledgeSideMaxDistance;
        float rayRadius = player.stats.current.ledgeSideCollisionRadius;

        if (Physics.SphereCast(sideOrigin, rayRadius, player.transform.forward, out var sideHit,
            rayDistance, player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore) &&
            Physics.Raycast(topOrigin, Vector3.down, out var topHit, player.height,
            player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 inputDirection = player.inputs.GetMovementDirection();
            Vector3 ledgeSideOrigin = sideOrigin + player.transform.right * Mathf.Sign(inputDirection.x) * player.radius;
            float ledgeHeight = topHit.point.y - player.height * 0.5f;
            Vector3 sideForward = -new Vector3(sideHit.normal.x, 0, sideHit.normal.z).normalized;
            float destinationHeight = player.height * 0.5f + Physics.defaultContactOffset;
            Vector3 climbDestination = topHit.point + Vector3.up * destinationHeight +
                player.transform.forward * player.radius;

            player.FaceDirection(sideForward);

            if (Physics.Raycast(ledgeSideOrigin, sideForward, rayDistance,
                player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore))
            {
                player.lateralVelocity = player.transform.right * inputDirection.x * player.stats.current.ledgeMovementSpeed;
            }
            else
            {
                player.lateralVelocity = Vector3.zero;
            }

            player.transform.position = new Vector3(sideHit.point.x, ledgeHeight, sideHit.point.z) - sideForward * player.radius - player.center;

            if (player.inputs.GetReleaseLedgeDown())
            {
                player.FaceDirection(-sideForward);
                player.states.Change<FallPlayerState>();
            }
            else if (player.inputs.GetJumpDown())
            {
                player.Jump(player.stats.current.maxJumpHeight);
                player.states.Change<FallPlayerState>();
            }
            else if (inputDirection.z > 0 && player.stats.current.canClimbLedges &&
                    ((1 << topHit.collider.gameObject.layer) & player.stats.current.ledgeClimbingLayers) != 0 &&
                    player.FitsIntoPosition(climbDestination))
            {
                m_keepParent = true;
                player.states.Change<LedgeClimbingPlayerState>();
                player.playerEvents.OnLedgeClimbing?.Invoke();
            }
        }
        else
        {
            player.states.Change<FallPlayerState>();
        }
    }

    protected override void OnExit(Player player)
    {
        m_clearParentRoutine = MCoroutineManager.Instance.BeginCoroutineNoRecord(ClearParentRoutine(player));
        //人物模型偏移
        player.skin.position -= player.transform.rotation * player.stats.current.ledgeHangingSkinOffset;
    }

    public override void OnContact(Player player, Collider other) { }

    protected virtual IEnumerator ClearParentRoutine(Player player)
    {
        if (m_keepParent) yield break;

        yield return new WaitForSeconds(k_clearParentDelay);

        player.transform.parent = null;
    }
}
