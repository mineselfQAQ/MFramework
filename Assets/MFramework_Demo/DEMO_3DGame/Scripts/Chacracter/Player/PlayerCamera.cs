using Cinemachine;
using MFramework;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Player player;
    public float maxDistance = 15f;
    public float initialAngle = 20f;
    public float heightOffset = 1f;

    [Header("Following Settings")]
    public Transform follow;
    public float verticalUpDeadZone = 0.15f;
    public float verticalDownDeadZone = 0.15f;
    public float verticalAirUpDeadZone = 4f;
    public float verticalAirDownDeadZone = 0;
    public float maxVerticalSpeed = 10f;
    public float maxAirVerticalSpeed = 100f;

    [Header("Orbit Settings")]
    public bool canOrbit = true;
    public bool canOrbitWithVelocity = true;
    public float orbitVelocityMultiplier = 5;

    [Range(0, 90)]
    public float verticalMaxRotation = 80;

    [Range(-90, 0)]
    public float verticalMinRotation = -20;

    protected float m_cameraDistance;
    protected float m_cameraTargetYaw;
    protected float m_cameraTargetPitch;

    protected Vector3 m_cameraTargetPosition;

    protected CinemachineVirtualCamera m_camera;
    protected Cinemachine3rdPersonFollow m_cameraBody;
    protected CinemachineBrain m_brain;

    protected Transform m_target;

    protected string k_targetName = "PlayerFollow";

    protected virtual void Start()
    {
        InitializeComponents();
        InitializeFollower();//Follower---�������ע�ӵ�����
        InitializeCamera();
    }

    protected virtual void LateUpdate()
    {
        HandleOrbit();
        HandleVelocityOrbit();
        HandleOffset();

        MoveTarget();
    }

    /// <summary>
    /// ���������
    /// </summary>
    public virtual void ResetCamera()
    {
        m_cameraDistance = maxDistance;
        m_cameraTargetPitch = initialAngle;
        m_cameraTargetYaw = player.transform.rotation.eulerAngles.y;
        m_cameraTargetPosition = player.unsizedPosition + Vector3.up * heightOffset;

        MoveTarget();
        m_brain.ManualUpdate();
    }

    protected virtual void InitializeComponents()
    {
        if (!player)
        {
            MLog.Print($"{typeof(PlayerCamera)}��δѡ��Player��������", MLogType.Error);
        }

        m_camera = GetComponent<CinemachineVirtualCamera>();
        //ǿ�ƽ�Body����Ϊ3rd Person Followģʽ
        m_cameraBody = m_camera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    protected virtual void InitializeFollower()
    {
        if (follow)
        {
            m_target = follow;
        }
        else
        {
            m_target = new GameObject(k_targetName).transform;
        }

        m_target.position = player.transform.position;
    }

    protected virtual void InitializeCamera()
    {
        //Tip��BodyΪ3rd Person Followģʽʱֻ��Ҫ����Follow����
        m_camera.Follow = m_target.transform;
        m_camera.LookAt = player.transform;//����Ҫ��ֻ�Ǳ�������

        ResetCamera();
    }

    /// <summary>
    /// ����Target(Follow)λ����Ϣ(ͨ���ӽ��ƶ�)
    /// </summary>
    protected virtual void HandleOrbit()
    {
        //Ҫ��1.����canOrbit
        if (canOrbit)
        {
            Vector3 direction = player.inputs.GetLookDirection();

            if (direction.sqrMagnitude > 0)
            {
                //ʹ�����ʱ���ݵ�ǰtimeScale�����Ƿ���ƶ�����ʹ��ʱֱ��ʹ��deltaTime
                bool usingMouse = player.inputs.IsLookingWithMouse();
                float deltaTimeMultiplier = usingMouse ? Time.timeScale : Time.deltaTime;

                //y��---������ת
                m_cameraTargetYaw += direction.x * deltaTimeMultiplier;
                //x��---������ת
                m_cameraTargetPitch -= direction.z * deltaTimeMultiplier;
                m_cameraTargetPitch = ClampAngle(m_cameraTargetPitch, verticalMinRotation, verticalMaxRotation);
            }
        }
    }

    /// <summary>
    /// ����Target(Follow)λ����Ϣ(ͨ�������ƶ�)
    /// </summary>
    protected virtual void HandleVelocityOrbit()
    {
        //Ҫ��1.����canOrbitWithVelocity 2.�ڵ���
        if (canOrbitWithVelocity && player.isGrounded)
        {
            //���������ƶ�ʱ����΢��ת
            Vector3 localVelocity = m_target.InverseTransformVector(player.velocity);
            m_cameraTargetYaw += localVelocity.x * orbitVelocityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// ����Target(Follow)λ����Ϣ
    /// </summary>
    protected virtual void HandleOffset()
    {
        Vector3 target = player.unsizedPosition + Vector3.up * heightOffset;//��׼λ��(����ͷ��)
        Vector3 previousPosition = m_cameraTargetPosition;
        float targetHeight = previousPosition.y;

        //---�߶ȸ���---
        //���������״̬ʱ����
        if (player.isGrounded || VerticalFollowingStates())
        {
            if (target.y > previousPosition.y + verticalUpDeadZone)
            {
                float offset = target.y - previousPosition.y - verticalUpDeadZone;
                targetHeight += Mathf.Min(offset, maxVerticalSpeed * Time.deltaTime);
            }
            else if (target.y < previousPosition.y - verticalDownDeadZone)
            {
                float offset = target.y - previousPosition.y + verticalDownDeadZone;
                targetHeight += Mathf.Max(offset, -maxVerticalSpeed * Time.deltaTime);
            }
        }
        //����ʱ����
        else if (target.y > previousPosition.y + verticalAirUpDeadZone)
        {
            float offset = target.y - previousPosition.y - verticalAirUpDeadZone;
            targetHeight += Mathf.Min(offset, maxAirVerticalSpeed * Time.deltaTime);
        }
        else if (target.y < previousPosition.y - verticalAirDownDeadZone)
        {
            float offset = target.y - previousPosition.y + verticalAirDownDeadZone;
            targetHeight += Mathf.Max(offset, -maxAirVerticalSpeed * Time.deltaTime);
        }

        m_cameraTargetPosition = new Vector3(target.x, targetHeight, target.z);
    }

    /// <summary>
    /// ����Target(Follow)����
    /// </summary>
    protected virtual void MoveTarget()
    {
        m_target.position = m_cameraTargetPosition;
        m_target.rotation = Quaternion.Euler(m_cameraTargetPitch, m_cameraTargetYaw, 0.0f);
        m_cameraBody.CameraDistance = m_cameraDistance;//���������Follow����
    }

    protected virtual float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    protected virtual bool VerticalFollowingStates()
    {
        return player.states.IsCurrentOfType(typeof(SwimPlayerState)) ||
               player.states.IsCurrentOfType(typeof(PoleClimbingPlayerState)) ||
               player.states.IsCurrentOfType(typeof(WallDragPlayerState)) ||
               player.states.IsCurrentOfType(typeof(LedgeHangingPlayerState)) ||
               player.states.IsCurrentOfType(typeof(LedgeClimbingPlayerState)) ||
               player.states.IsCurrentOfType(typeof(RailGrindPlayerState));
    }
}