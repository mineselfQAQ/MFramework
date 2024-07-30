using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailGrindPlayerState : PlayerState
{
    protected bool m_backwards;//���ﳯ���Ƿ���
    protected float m_speed;
    protected float m_lastDashTime;

    protected override void OnEnter(Player player)
    {
        //������Ϣ
        Evaluate(player, out var point, out var forward, out var upward, out _);
        //����λ����Ϣ
        UpdatePosition(player, point, upward);

        m_backwards = Vector3.Dot(player.transform.forward, forward) < 0;
        m_speed = Mathf.Max(player.lateralVelocity.magnitude,
            player.stats.current.minGrindInitialSpeed);
        player.velocity = Vector3.zero;

        player.UseCustomCollision(player.stats.current.useCustomCollision);
    }

    protected override void OnStep(Player player)
    {
        player.Jump();//���ֻ����

        if (player.onRails)
        {
            Evaluate(player, out var point, out var forward, out var upward, out var t);

            Vector3 direction = m_backwards ? -forward : forward;//��������(ǰ��)
            float factor = Vector3.Dot(Vector3.up, direction);//��������(����)
            float multiplier = factor <= 0 ?
                player.stats.current.slopeDownwardForce ://����
                player.stats.current.slopeUpwardForce;//����

            HandleDeceleration(player);//����(��������)
            HandleDash(player);//����(����)

            //б�¼Ӽ���
            if (player.stats.current.applyGrindingSlopeFactor)
                m_speed -= factor * multiplier * Time.deltaTime;
            //����
            m_speed = Mathf.Clamp(m_speed,
                player.stats.current.minGrindSpeed,
                player.stats.current.grindTopSpeed);

            Rotate(player, direction, upward);
            player.velocity = direction * m_speed;

            //����λ����Ϣ
            //Tip�����ڷǱպ����ߣ���Ҫ����һ��������ֹ���̶��ڹ����
            if (player.rails.Spline.Closed || (t > 0.05f && t < 0.95f))
                UpdatePosition(player, point, upward);
        }
        else
        {
            player.states.Change<FallPlayerState>();
        }
    }

    protected override void OnExit(Player player)
    {
        player.ExitRail();
        player.UseCustomCollision(false);
    }

    public override void OnContact(Player player, Collider other) { }

    protected virtual void Evaluate(Player player, out Vector3 point,
        out Vector3 forward, out Vector3 upward, out float t)
    {
        //playerԭ��(��railsΪ��׼)
        Vector3 origin = player.rails.transform.InverseTransformPoint(player.transform.position);

        //��ȡ�����(�������ŵ�)
        //Tip���ڲ�����ʹ�þֲ�����
        SplineUtility.GetNearestPoint(player.rails.Spline, origin, out var nearest, out t);

        //��ŵ�(��������)
        point = player.rails.transform.TransformPoint(nearest);

        forward = Vector3.Normalize(player.rails.EvaluateTangent(t));//ǰ����
        upward = Vector3.Normalize(player.rails.EvaluateUpVector(t));//�Ϸ���
    }

    protected virtual void HandleDeceleration(Player player)
    {
        //����������
        //1.����canGrindBrake 2.��סGrindBrake��
        if (player.stats.current.canGrindBrake && player.inputs.GetGrindBrake())
        {
            float decelerationDelta = player.stats.current.grindBrakeDeceleration * Time.deltaTime;
            m_speed = Mathf.MoveTowards(m_speed, 0, decelerationDelta);
        }
    }

    protected virtual void HandleDash(Player player)
    {
        //��̼���������
        //1.����canGrindDash 2.����Dash�� 3.��ȴʱ���ѹ�
        if (player.stats.current.canGrindDash &&
            player.inputs.GetDashDown() &&
            Time.time >= m_lastDashTime + player.stats.current.grindDashCoolDown)
        {
            m_lastDashTime = Time.time;
            m_speed = player.stats.current.grindDashForce;
            player.playerEvents.OnDashStarted.Invoke();
        }
    }

    protected virtual void UpdatePosition(Player player, Vector3 point, Vector3 upward)
    {
        player.transform.position = point + upward * GetDistanceToRail(player);
    }
    protected virtual float GetDistanceToRail(Player player)
    {
        return player.originalHeight * 0.5f + player.stats.current.grindRadiusOffset;
    }

    protected virtual void Rotate(Player player, Vector3 forward, Vector3 upward)
    {
        if (forward != Vector3.zero)
            player.transform.rotation = Quaternion
                .LookRotation(forward, player.transform.up);

        player.transform.rotation = Quaternion
            .FromToRotation(player.transform.up, upward) * player.transform.rotation;
    }

}