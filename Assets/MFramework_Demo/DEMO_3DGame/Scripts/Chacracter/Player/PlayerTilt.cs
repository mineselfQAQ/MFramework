using UnityEngine;

/// <summary>
/// ���������˶�ʱ������б(������Roll)
/// </summary>
public class PlayerTilt : MonoBehaviour
{
    public Transform target;//Ӧ����бĿ��
    public float maxTiltAngle = 15;//�����б�Ƕ�
    public float tiltSmoothTime = 0.2f;

    protected Player m_player;
    protected Quaternion m_initialRotation;

    protected float m_velocity;

    /// <summary>
    /// �Ƿ������б
    /// </summary>
    public virtual bool CanLean()
    {
        //��бҪ��
        //Walk/Swim/Gliding״̬��֮һ
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
        //���������Լ���ǰ�ٶȵó�������б�Ƕ�
        Vector3 inputDirection = m_player.inputs.GetMovementCameraDirection();
        Vector3 moveDirection = m_player.lateralVelocity.normalized;
        float angle = Vector3.SignedAngle(inputDirection, moveDirection, Vector3.up);
        float targetAngle = CanLean() ? Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle) : 0;

        //Ӧ����б
        Vector3 rotation = target.localEulerAngles;
        rotation.z = Mathf.SmoothDampAngle(rotation.z, targetAngle, ref m_velocity, tiltSmoothTime);
        target.localEulerAngles = rotation;
    }
}