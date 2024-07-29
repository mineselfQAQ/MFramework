using MFramework;
using System.Collections;
using UnityEngine;

public class LedgeClimbingPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        MCoroutineManager.Instance.BeginCoroutineNoRecord(SetPositionRoutine(player));
    }

    protected override void OnStep(Player player) { }

    protected override void OnExit(Player player)
    {
        player.ResetSkinParent();
    }

    public override void OnContact(Player player, Collider other) { }

    protected virtual IEnumerator SetPositionRoutine(Player player)
    {
        float elapsedTime = 0f;
        float totalDuration = player.stats.current.ledgeClimbingDuration;
        float halfDuration = totalDuration / 2f;

        Vector3 initialPosition = player.transform.localPosition;
        Vector3 targetVerticalPosition = player.transform.position + Vector3.up * (player.height + Physics.defaultContactOffset);
        Vector3 targetLateralPosition = targetVerticalPosition + player.transform.forward * player.radius * 2f;

        //Player�и����壬����������skin������playerͬ�㣬����ֱ�ӷ��������
        if (player.transform.parent != null)
        {
            targetVerticalPosition = player.transform.parent.InverseTransformPoint(targetVerticalPosition);
            targetLateralPosition = player.transform.parent.InverseTransformPoint(targetLateralPosition);
        }
        player.SetSkinParent(player.transform.parent);
        //����ģ��ƫ��
        player.skin.position += player.transform.rotation * player.stats.current.ledgeClimbingSkinOffset;

        //1.�����ƶ�
        while (elapsedTime <= halfDuration)
        {
            elapsedTime += Time.deltaTime;
            player.transform.localPosition = Vector3.Lerp(initialPosition, targetVerticalPosition, elapsedTime / halfDuration);
            yield return null;
        }
        elapsedTime = 0;
        player.transform.localPosition = targetVerticalPosition;
        //2.��ǰ�ƶ�
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
