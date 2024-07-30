using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailGrindPlayerState : PlayerState
{
    protected bool m_backwards;//人物朝向是否反了
    protected float m_speed;
    protected float m_lastDashTime;

    protected override void OnEnter(Player player)
    {
        //估算信息
        Evaluate(player, out var point, out var forward, out var upward, out _);
        //更新位置信息
        UpdatePosition(player, point, upward);

        m_backwards = Vector3.Dot(player.transform.forward, forward) < 0;
        m_speed = Mathf.Max(player.lateralVelocity.magnitude,
            player.stats.current.minGrindInitialSpeed);
        player.velocity = Vector3.zero;

        player.UseCustomCollision(player.stats.current.useCustomCollision);
    }

    protected override void OnStep(Player player)
    {
        player.Jump();//轨道只能跳

        if (player.onRails)
        {
            Evaluate(player, out var point, out var forward, out var upward, out var t);

            Vector3 direction = m_backwards ? -forward : forward;//滑动方向(前后)
            float factor = Vector3.Dot(Vector3.up, direction);//方向因子(上下)
            float multiplier = factor <= 0 ?
                player.stats.current.slopeDownwardForce ://下坡
                player.stats.current.slopeUpwardForce;//上坡

            HandleDeceleration(player);//减速(持续按键)
            HandleDash(player);//加速(按键)

            //斜坡加减速
            if (player.stats.current.applyGrindingSlopeFactor)
                m_speed -= factor * multiplier * Time.deltaTime;
            //限速
            m_speed = Mathf.Clamp(m_speed,
                player.stats.current.minGrindSpeed,
                player.stats.current.grindTopSpeed);

            Rotate(player, direction, upward);
            player.velocity = direction * m_speed;

            //更新位置信息
            //Tip：对于非闭合曲线，需要给定一定余量防止被固定在轨道上
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
        //player原点(以rails为基准)
        Vector3 origin = player.rails.transform.InverseTransformPoint(player.transform.position);

        //获取最近点(所需的落脚点)
        //Tip：内部计算使用局部坐标
        SplineUtility.GetNearestPoint(player.rails.Spline, origin, out var nearest, out t);

        //落脚点(世界坐标)
        point = player.rails.transform.TransformPoint(nearest);

        forward = Vector3.Normalize(player.rails.EvaluateTangent(t));//前方向
        upward = Vector3.Normalize(player.rails.EvaluateUpVector(t));//上方向
    }

    protected virtual void HandleDeceleration(Player player)
    {
        //减速条件：
        //1.开启canGrindBrake 2.按住GrindBrake键
        if (player.stats.current.canGrindBrake && player.inputs.GetGrindBrake())
        {
            float decelerationDelta = player.stats.current.grindBrakeDeceleration * Time.deltaTime;
            m_speed = Mathf.MoveTowards(m_speed, 0, decelerationDelta);
        }
    }

    protected virtual void HandleDash(Player player)
    {
        //冲刺加速条件：
        //1.开启canGrindDash 2.按下Dash键 3.冷却时间已过
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