using UnityEngine;
using UnityEngine.InputSystem;

public enum InputActionName
{
    movement,
    run,
    jump,
    dive,
    spin,
    pickAndDrop,
    crouch,
    airDive,
    stomp,
    releaseLedge,
    pause,
    look,
    glide,
    dash,
    grindBrake,
    interact
}

public class PlayerInputManager : MonoBehaviour
{
    public InputActionAsset actions;

    protected InputAction m_movement;
    protected InputAction m_run;
    protected InputAction m_jump;
    protected InputAction m_dive;
    protected InputAction m_spin;
    protected InputAction m_pickAndDrop;
    protected InputAction m_crouch;
    protected InputAction m_airDive;
    protected InputAction m_stomp;
    protected InputAction m_releaseLedge;
    protected InputAction m_pause;
    protected InputAction m_look;
    protected InputAction m_glide;
    protected InputAction m_dash;
    protected InputAction m_grindBrake;
    protected InputAction m_interact;

    protected Camera m_camera;

    protected float m_movementDirectionUnlockTime;
    protected float? m_lastJumpTime;

    protected const string k_mouseDeviceName = "Mouse";

    protected const float k_jumpBuffer = 0.15f;

    protected virtual void Awake()
    {
        CacheActions();
    }

    protected virtual void Start()
    {
        m_camera = Camera.main;
        actions.Enable();
    }

    protected virtual void Update()
    {
        if (m_jump.WasPressedThisFrame())
        {
            m_lastJumpTime = Time.time;
        }
    }

    protected virtual void OnEnable()
    {
        actions?.Enable();
    }
    protected virtual void OnDisable()
    {
        actions?.Disable();
    }

    protected virtual void CacheActions()
    {
        m_movement = actions["Movement"];
        m_run = actions["Run"];
        m_jump = actions["Jump"];
        m_dive = actions["Dive"];
        m_spin = actions["Spin"];
        m_pickAndDrop = actions["PickAndDrop"];
        m_crouch = actions["Crouch"];
        m_airDive = actions["AirDive"];
        m_stomp = actions["Stomp"];
        m_releaseLedge = actions["ReleaseLedge"];
        m_pause = actions["Pause"];
        m_look = actions["Look"];
        m_glide = actions["Glide"];
        m_dash = actions["Dash"];
        m_grindBrake = actions["Grind Brake"];
        m_interact = actions["Interact"];
    }

    /// <summary>
    /// 输入方向
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetMovementDirection()
    {
        //限制移动启动时间(没到时间不可以进行移动操作)
        if (Time.time < m_movementDirectionUnlockTime) return Vector3.zero;
        
        //获取移动向量,取值范围---归一化向量((0,0)为原点，半径为1的圆)
        //如果是键盘，只有(-1,0)(1,0)(0,1)(0,-1)(0.71,0,71)(0.71,-0.71)(-0.71,0.71)(-0.71,-0.71)共8种可能
        //如果是手柄，那么在圆范围内都是可能的
        Vector2 value = m_movement.ReadValue<Vector2>();
        return GetAxisWithCrossDeadZone(value);
    }

    public virtual Vector3 GetLookDirection()
    {
        Vector2 value = m_look.ReadValue<Vector2>();

        if (IsLookingWithMouse())
        {
            return new Vector3(value.x, 0, value.y);
        }

        return GetAxisWithCrossDeadZone(value);
    }

    /// <summary>
    /// 根据Camera输入方向(归一化过)
    /// </summary>
    public virtual Vector3 GetMovementCameraDirection()
    {
        Vector3 direction = GetMovementDirection();

        if (direction.sqrMagnitude > 0)//有输入(因为添加过死区，手柄必须输入一定量才能进入)
        {
            //进行摄像头偏移旋转(人物的移动以摄像头的方向为准)
            //大致就是：
            //理论上摄像头应该和人物视线是同一方向的，但是如果摄像头看向右侧，其实就是人物看向左侧，
            //此时如果还是一样的输出的话，向前走人物会越走越远，但是如果以摄像机为准的话，人物还是向前走(摄像机的前)
            var rotation = Quaternion.AngleAxis(m_camera.transform.eulerAngles.y, Vector3.up);
            direction = rotation * direction;
            direction = direction.normalized;
        }

        return direction;
    }

    public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axis)
    {
        //用于手柄：
        //大概率是手柄可能会漂移，那么会导致如(0.02,-0.01)的输入，
        //那么如果能够设置一个半径大约0.1的圆为死区，就不会发生这种事情
        float deadzone = InputSystem.settings.defaultDeadzoneMin;
        axis.x = Mathf.Abs(axis.x) > deadzone ? RemapToDeadzone(axis.x, deadzone) : 0;
        axis.y = Mathf.Abs(axis.y) > deadzone ? RemapToDeadzone(axis.y, deadzone) : 0;
        return new Vector3(axis.x, 0, axis.y);
    }

    public virtual bool IsLookingWithMouse()
    {
        if (m_look.activeControl == null)
        {
            return false;
        }

        return m_look.activeControl.device.name.Equals(k_mouseDeviceName);
    }

    public virtual bool GetRun() => m_run.IsPressed();
    public virtual bool GetRunUp() => m_run.WasReleasedThisFrame();

    public virtual bool GetJumpDown()
    {
        if (m_lastJumpTime != null &&
            Time.time - m_lastJumpTime < k_jumpBuffer)
        {
            m_lastJumpTime = null;
            return true;
        }

        return false;
    }

    public virtual bool GetJumpUp() => m_jump.WasReleasedThisFrame();
    public virtual bool GetDive() => m_dive.IsPressed();
    public virtual bool GetSpinDown() => m_spin.WasPressedThisFrame();
    public virtual bool GetPickAndDropDown() => m_pickAndDrop.WasPressedThisFrame();
    public virtual bool GetCrouchAndCraw() => m_crouch.IsPressed();
    public virtual bool GetAirDiveDown() => m_airDive.WasPressedThisFrame();
    public virtual bool GetStompDown() => m_stomp.WasPressedThisFrame();
    public virtual bool GetReleaseLedgeDown() => m_releaseLedge.WasPressedThisFrame();
    public virtual bool GetGlide() => m_glide.IsPressed();
    public virtual bool GetDashDown() => m_dash.WasPressedThisFrame();
    public virtual bool GetGrindBrake() => m_grindBrake.IsPressed();
    public virtual bool GetPauseDown() => m_pause.WasPressedThisFrame();
    public virtual bool GetInteractDown() => m_interact.WasPressedThisFrame();
    
    public virtual InputAction GetInputAction(InputActionName name)
    {
        switch (name)
        {
            case InputActionName.movement:
                return m_movement;
            case InputActionName.run:
                return m_run;
            case InputActionName.jump:
                return m_jump;
            case InputActionName.dive:
                return m_dive;
            case InputActionName.spin:
                return m_spin;
            case InputActionName.pickAndDrop:
                return m_pickAndDrop;
            case InputActionName.crouch:
                return m_crouch;
            case InputActionName.airDive:
                return m_airDive;
            case InputActionName.stomp:
                return m_stomp;
            case InputActionName.releaseLedge:
                return m_releaseLedge;
            case InputActionName.pause:
                return m_pause;
            case InputActionName.look:
                return m_look;
            case InputActionName.glide:
                return m_glide;
            case InputActionName.dash:
                return m_dash;
            case InputActionName.grindBrake:
                return m_grindBrake;
            case InputActionName.interact:
                return m_interact;
            default:
                return null;
        }
    }

    public void EnableInputAction(InputActionName name)
    {
        var inputAction = GetInputAction(name);
        inputAction.Enable();
    }
    public void DisableInputAction(InputActionName name)
    {
        var inputAction = GetInputAction(name);
        inputAction.Disable();
    }

    /// <summary>
    /// 根据控制方案(键盘/手柄)获取相应按键名
    /// </summary>
    public string GetKey(InputAction action, string deviceType)
    {
        foreach (var binding in action.bindings)
        {
            if (binding.effectivePath.Contains(deviceType))
            {
                return binding.ToDisplayString();
            }
        }
        return null;
    }


    protected float RemapToDeadzone(float value, float deadzone)
    {
        //取值范围[0.125,1]--->[0,1]
        //Tip: axis.x和axis.y虽然可以取[0,1]，但是由于死区的限制，小于0.125直接设为0，所以变为[0.125,1]
        return (value - deadzone) / (1 - deadzone);
    }

    /// <summary>
    /// 禁止方向输入一定时间
    /// </summary>
    public virtual void LockMovementDirection(float duration = 0.25f)
    {
        m_movementDirectionUnlockTime = Time.time + duration;
    }
}