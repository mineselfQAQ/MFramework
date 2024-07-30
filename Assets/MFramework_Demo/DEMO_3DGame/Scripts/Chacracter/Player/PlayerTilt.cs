using UnityEngine;

/// <summary>
/// 处理人物运动时左右倾斜(翻滚角Roll)
/// </summary>
public class PlayerTilt : MonoBehaviour
{
    public Transform target;//应用倾斜目标
    public float maxTiltAngle = 15;//最大倾斜角度
    public float tiltSmoothTime = 0.2f;

    protected Player m_player;
    protected Quaternion m_initialRotation;

    protected float m_velocity;

    /// <summary>
    /// 是否可以倾斜
    /// </summary>
    public virtual bool CanLean()
    {
        //倾斜要求：
        //Walk/Swim/Gliding状态中之一
        bool walking = m_player.states.IsCurrentOfType(typeof(WalkPlayerState));
        bool swimming = m_player.states.IsCurrentOfType(typeof(SwimPlayerState));
        bool gliding = m_player.states.IsCurrentOfType(typeof(GlidingPlayerState));
        return walking || swimming || gliding;
    }

    protected virtual void Awake()
    {
        m_player = GetComponent<Player>();
    }

    protected virtual void LateUpdate()
    {
        //对于输入以及当前速度得出所需倾斜角度
        Vector3 inputDirection = m_player.inputs.GetMovementCameraDirection();
        Vector3 moveDirection = m_player.lateralVelocity.normalized;
        float angle = Vector3.SignedAngle(inputDirection, moveDirection, Vector3.up);
        float targetAngle = CanLean() ? Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle) : 0;

        //应用倾斜
        Vector3 rotation = target.localEulerAngles;
        rotation.z = Mathf.SmoothDampAngle(rotation.z, targetAngle, ref m_velocity, tiltSmoothTime);
        target.localEulerAngles = rotation;
    }
}