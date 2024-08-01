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
        InitializeFollower();//Follower---摄像机所注视的物体
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
    /// 将相机重置
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
            MLog.Print($"{typeof(PlayerCamera)}：未选择Player，请拖入", MLogType.Error);
        }

        m_camera = GetComponent<CinemachineVirtualCamera>();
        //强制将Body更改为3rd Person Follow模式
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
        //Tip：Body为3rd Person Follow模式时只需要传入Follow即可
        m_camera.Follow = m_target.transform;
        m_camera.LookAt = player.transform;//不必要，只是表明主体

        ResetCamera();
    }

    /// <summary>
    /// 更新Target(Follow)位置信息(通过视角移动)
    /// </summary>
    protected virtual void HandleOrbit()
    {
        //要求：1.开启canOrbit
        if (canOrbit)
        {
            Vector3 direction = player.inputs.GetLookDirection();

            if (direction.sqrMagnitude > 0)
            {
                //使用鼠标时根据当前timeScale决定是否可移动，不使用时直接使用deltaTime
                bool usingMouse = player.inputs.IsLookingWithMouse();
                float deltaTimeMultiplier = usingMouse ? Time.timeScale : Time.deltaTime;

                //y轴---左右旋转
                m_cameraTargetYaw += direction.x * deltaTimeMultiplier;
                //x轴---上下旋转
                m_cameraTargetPitch -= direction.z * deltaTimeMultiplier;
                m_cameraTargetPitch = ClampAngle(m_cameraTargetPitch, verticalMinRotation, verticalMaxRotation);
            }
        }
    }

    /// <summary>
    /// 更新Target(Follow)位置信息(通过人物移动)
    /// </summary>
    protected virtual void HandleVelocityOrbit()
    {
        //要求：1.开启canOrbitWithVelocity 2.在地面
        if (canOrbitWithVelocity && player.isGrounded)
        {
            //向左向右移动时，轻微旋转
            Vector3 localVelocity = m_target.InverseTransformVector(player.velocity);
            m_cameraTargetYaw += localVelocity.x * orbitVelocityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// 更新Target(Follow)位置信息
    /// </summary>
    protected virtual void HandleOffset()
    {
        Vector3 target = player.unsizedPosition + Vector3.up * heightOffset;//标准位置(人物头上)
        Vector3 previousPosition = m_cameraTargetPosition;
        float targetHeight = previousPosition.y;

        //---高度更新---
        //地面或特殊状态时更新
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
        //空中时更新
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
    /// 设置Target(Follow)属性
    /// </summary>
    protected virtual void MoveTarget()
    {
        m_target.position = m_cameraTargetPosition;
        m_target.rotation = Quaternion.Euler(m_cameraTargetPitch, m_cameraTargetYaw, 0.0f);
        m_cameraBody.CameraDistance = m_cameraDistance;//摄像机距离Follow距离
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