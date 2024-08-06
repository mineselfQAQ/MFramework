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
            MCoroutineManager.Instance.EndCoroutine(m_clearParentRoutine);

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
        //顶面原点---抓住边缘点前上方
        Vector3 topOrigin = player.position + Vector3.up * ledgeTopHeightOffset + player.transform.forward * ledgeTopMaxDistance;
        //侧面(Player面对的那个面)原点---Player顶偏下
        Vector3 sideOrigin = player.position + Vector3.up * player.height * 0.5f + Vector3.down * player.stats.current.ledgeSideHeightOffset;
        float rayDistance = player.radius + player.stats.current.ledgeSideMaxDistance;
        float rayRadius = player.stats.current.ledgeSideCollisionRadius;

        //获取侧面以及顶面击中点
        if (Physics.SphereCast(sideOrigin, rayRadius, player.transform.forward, out var sideHit,
            rayDistance, player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore) &&
            Physics.Raycast(topOrigin, Vector3.down, out var topHit, player.height,
            player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore))
        {
            //Player y坐标
            float ledgeHeight = topHit.point.y - player.height * 0.5f;
            //侧面前方向(与人物前方向同向)
            Vector3 sideForward = -new Vector3(sideHit.normal.x, 0, sideHit.normal.z).normalized;

            //Player爬沿后目标高度偏移
            float destinationHeight = player.height * 0.5f + Physics.defaultContactOffset;
            //Player爬沿后最终位置
            Vector3 climbDestination = topHit.point + Vector3.up * destinationHeight +
                player.transform.forward * player.radius;

            player.FaceDirection(sideForward);

            Vector3 inputDirection = player.inputs.GetMovementDirection();
            //人物边缘原点
            Vector3 ledgeSideOrigin = sideOrigin + player.transform.right * Mathf.Sign(inputDirection.x) * player.radius;
            //Player左右移动后，如果侧边已到达极限，不再移动
            if (Physics.Raycast(ledgeSideOrigin, sideForward, rayDistance,
                player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore))
            {
                player.lateralVelocity = player.transform.right * inputDirection.x * player.stats.current.ledgeMovementSpeed;
            }
            else
            {
                player.lateralVelocity = Vector3.zero;
            }

            //根据sideHit重计算位置信息
            player.transform.position = new Vector3(sideHit.point.x, ledgeHeight, sideHit.point.z) - sideForward * player.radius - player.center;

            if (inputDirection.z < 0)
            {
                player.FaceDirection(-sideForward);
                player.states.Change<FallPlayerState>();
            }
            else if (player.inputs.GetJumpDown())
            {
                player.Jump(player.stats.current.maxJumpHeight);
                player.states.Change<FallPlayerState>();
            }
            //爬沿条件：
            //1.按上 2.开启canClimbLedges 3.攀爬物体layer包含在ledgeClimbingLayers中
            //4.Player目标位置可用
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
