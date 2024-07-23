using System.Collections;
using UnityEngine;

public class LedgeClimbingPlayerState : PlayerState
{
    protected IEnumerator m_routine;

    protected override void OnEnter(Player player)
    {
        m_routine = SetPositionRoutine(player);
        player.StartCoroutine(m_routine);
    }

    protected override void OnStep(Player player) { }

    protected override void OnExit(Player player)
    {
        player.ResetSkinParent();
        player.StopCoroutine(m_routine);
    }

    public override void OnContact(Player player, Collider other) { }

    protected virtual IEnumerator SetPositionRoutine(Player player)
    {
        var elapsedTime = 0f;
        var totalDuration = player.stats.current.ledgeClimbingDuration;
        var halfDuration = totalDuration / 2f;

        var initialPosition = player.transform.localPosition;
        var targetVerticalPosition = player.transform.position + Vector3.up * (player.height + Physics.defaultContactOffset);
        var targetLateralPosition = targetVerticalPosition + player.transform.forward * player.radius * 2f;

        if (player.transform.parent != null)
        {
            targetVerticalPosition = player.transform.parent.InverseTransformPoint(targetVerticalPosition);
            targetLateralPosition = player.transform.parent.InverseTransformPoint(targetLateralPosition);
        }

        player.SetSkinParent(player.transform.parent);
        player.skin.position += player.transform.rotation * player.stats.current.ledgeClimbingSkinOffset;

        while (elapsedTime <= halfDuration)
        {
            elapsedTime += Time.deltaTime;
            player.transform.localPosition = Vector3.Lerp(initialPosition, targetVerticalPosition, elapsedTime / halfDuration);
            yield return null;
        }

        elapsedTime = 0;
        player.transform.localPosition = targetVerticalPosition;

        while (elapsedTime <= halfDuration)
        {
            elapsedTime += Time.deltaTime;
            player.transform.localPosition = Vector3.Lerp(targetVerticalPosition, targetLateralPosition, elapsedTime / halfDuration);
            yield return null;
        }

        player.transform.localPosition = targetLateralPosition;
        player.states.Change<IdlePlayerState>();
    }
}
