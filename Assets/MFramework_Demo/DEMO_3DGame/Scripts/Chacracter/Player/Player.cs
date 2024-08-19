using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerStatsManager))]
[RequireComponent(typeof(PlayerStateManager))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerAudio))]
[RequireComponent(typeof(PlayerTilt))]
[RequireComponent(typeof(PlayerFootsteps))]
[RequireComponent(typeof(PlayerParticles))]
[RequireComponent(typeof(HurtEffect))]
[RequireComponent(typeof(PlayerLevelPause))]
[RequireComponent(typeof(Health))]
public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;

    public Transform pickableSlot;
    public Transform skin;

    protected Vector3 m_respawnPosition;
    protected Quaternion m_respawnRotation;

    protected Vector3 m_skinInitialPosition;
    protected Quaternion m_skinInitialRotation;

    public PlayerInputManager inputs { get; protected set; }
    public PlayerStatsManager stats { get; protected set; }

    public Health health { get; protected set; }

    public bool onWater { get; protected set; }

    public bool holding { get; protected set; }

    public int jumpCounter { get; protected set; }
    public int airSpinCounter { get; protected set; }
    public int airDashCounter { get; protected set; }

    public float lastDashTime { get; protected set; }

    public Vector3 lastWallNormal { get; protected set; }

    public Pole pole { get; protected set; }//当前所抱柱子
    public Collider water { get; protected set; }//当前所在水体
    public Pickable pickable { get; protected set; }//当前所持物体

    public virtual bool isAlive => !health.isEmpty;

    public virtual bool canStandUp => !SphereCast(Vector3.up, originalHeight);

    protected const float k_waterExitOffset = 0.25f;

    protected override void Awake()
    {
        base.Awake();
        InitializeInputs();
        InitializeStats();
        InitializeHealth();
        InitializeTag();
        InitializeRespawn();

        entityEvents.OnGroundEnter.AddListener(() =>
        {
            ResetJumps();
            ResetAirSpins();
            ResetAirDash();
        });

        entityEvents.OnRailsEnter.AddListener(() =>
        {
            ResetJumps();
            ResetAirSpins();
            ResetAirDash();
            StartGrind();
        });
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTags.VolumeWater))//碰到体积水
        {
            //入水条件：
            //1.还不在水中 2.Player在水的范围中
            if (!onWater && other.bounds.Contains(unsizedPosition))
            {
                EnterWater(other);
            }
            //出水条件：
            //1.在水中 2.Player不在水的范围中
            else if (onWater)
            {
                var exitPoint = position + Vector3.down * k_waterExitOffset;

                if (!other.bounds.Contains(exitPoint))
                {
                    ExitWater();
                }
            }
        }
    }

    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
    protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
    protected virtual void InitializeHealth() => health = GetComponent<Health>();
    protected virtual void InitializeTag() => tag = GameTags.Player;
    protected virtual void InitializeRespawn()
    {
        m_respawnPosition = transform.position;
        m_respawnRotation = transform.rotation;
    }
    protected virtual void InitializeSkin()
    {
        if (skin)
        {
            m_skinInitialPosition = skin.localPosition;
            m_skinInitialRotation = skin.localRotation;
        }
    }

    protected override bool EvaluateLanding(RaycastHit hit)
    {
        return base.EvaluateLanding(hit) && !hit.collider.CompareTag(GameTags.Spring);
    }

    /// <summary>
    /// 处理斜坡情况(提供一个向前下方的力)
    /// </summary>
    protected override void HandleHighLedge(RaycastHit hit)
    {
        if (onWater) return;

        //计算滑落方向
        //Tip：由于使用的是SphereCast()，所以hit点在斜坡情况必然不在Player正下方而是斜一点的位置
        Vector3 edgeNormal = hit.point - position;
        Vector3 edgePushDirection = Vector3.Cross(edgeNormal, Vector3.Cross(edgeNormal, Vector3.up));

        controller.Move(edgePushDirection * stats.current.gravity * Time.deltaTime);
    }

    /// <summary>
    /// 重生
    /// </summary>
    public virtual void Respawn()
    {
        health.Reset();
        transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
        states.Change<IdlePlayerState>();
    }
    /// <summary>
    /// 设置重生位置
    /// </summary>
    public virtual void SetRespawnPos(Vector3 position, Quaternion rotation)
    {
        m_respawnPosition = position;
        m_respawnRotation = rotation;
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    /// <param Name="origin">造成伤害物体的原点</param>
    public override void ApplyDamage(int amount, Vector3 origin)
    {
        //Player还活着且未进入无敌状态
        if (!health.isEmpty && !health.recovering)
        {
            health.Damage(amount);

            Vector3 damageDir = origin - transform.position;//击退方向
            damageDir.y = 0;
            damageDir = damageDir.normalized;
            FaceDirection(damageDir);

            lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;

            if (!onWater)//不在水中
            {
                verticalVelocity = Vector3.up * stats.current.hurtUpwardForce;//向上击退
                states.Change<HurtPlayerState>();
            }

            playerEvents.OnHurt?.Invoke();

            if (health.isEmpty)
            {
                Throw();
                playerEvents.OnDie?.Invoke();
            }
        }
    }

    public virtual void Die()
    {
        health.Set(0);
        playerEvents.OnDie?.Invoke();
    }

    /// <summary>
    /// 入水
    /// </summary>
    public virtual void EnterWater(Collider water)
    {
        //入水条件：
        //1.还没在水中 2.还活着
        if (!onWater && !health.isEmpty)
        {
            Throw();
            onWater = true;
            this.water = water;
            states.Change<SwimPlayerState>();
        }
    }
    /// <summary>
    /// 出水
    /// </summary>
    public virtual void ExitWater()
    {
        if (onWater)
        {
            onWater = false;
        }
    }

    /// <summary>
    /// 抓杆
    /// </summary>
    public virtual void GrabPole(Collider other)
    {
        //抓杆条件：
        //1.开启canPoleClimb 2.在下降 3.未持物 4.碰撞物体为杆子
        if (stats.current.canPoleClimb && velocity.y <= 0
            && !holding && other.TryGetComponent(out Pole pole))
        {
            this.pole = pole;
            states.Change<PoleClimbingPlayerState>();
        }
    }

    /// <summary>
    /// 加速
    /// </summary>
    public virtual void Accelerate(Vector3 direction)
    {
        //旋转牵引---如果在地面跑步，就会使用runningTurnningDrag，否则为turningDrag
        float turningDrag = isGrounded && inputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
        //最高速度---如果在跑步，就会使用runningTopSpeed，否则为topSpeed
        float topSpeed = inputs.GetRun() ? stats.current.runningTopSpeed : stats.current.topSpeed;
        //加速度---如果在地面跑步，就会使用runningAcceleration，否则为acceleration
        float acceleration = isGrounded && inputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
        //最终加速度---如果在地面，就会使用acceleration，否则为airAcceleration
        float finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;

        Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

        if (inputs.GetRunUp())
        {
            lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
        }
    }
    /// <summary>
    /// 根据输入加速
    /// </summary>
    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = inputs.GetMovementCameraDirection();
        Accelerate(inputDirection);
    }

    /// <summary>
    /// 水中加速
    /// </summary>
    public virtual void WaterAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.waterTurningDrag,
            stats.current.swimAcceleration, stats.current.swimTopSpeed);
    }
    /// <summary>
    /// 爬行时加速
    /// </summary>
    public virtual void CrawlingAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.crawlingTurningSpeed,
            stats.current.crawlingAcceleration, stats.current.crawlingTopSpeed);
    }
    /// <summary>
    /// 后空翻时加速
    /// </summary>
    public virtual void BackflipAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.backflipTurningDrag,
            stats.current.backflipAirAcceleration, stats.current.backflipTopSpeed);
    }

    /// <summary>
    /// 减速
    /// </summary>
    public virtual void Decelerate() => Decelerate(stats.current.deceleration);

    /// <summary>
    /// 应用斜坡控制
    /// </summary>
    public virtual void RegularSlopeFactor()
    {
        if (stats.current.applySlopeFactor)//需要开启才能使用
        {
            SlopeFactor(stats.current.slopeUpwardForce, stats.current.slopeDownwardForce);
        }
    }

    /// <summary>
    /// 摩擦力
    /// </summary>
    public virtual void Friction()
    {
        if (OnSlopingGround())//斜坡时
            Decelerate(stats.current.slopeFriction);
        else//非斜坡时
            Decelerate(stats.current.friction);
    }

    /// <summary>
    /// 施加重力，verticalVelocity不断达到极限
    /// </summary>
    public virtual void Gravity()
    {
        //当不在地面时(跳起或下落)，只要摔落速度没有达到最大就还需要进行调整
        if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
        {
            var speed = verticalVelocity.y;
            var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
            speed -= force * gravityMultiplier * Time.deltaTime;//向下的速度越来越快
            speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);//限制在-gravityTopSpeed
            verticalVelocity = new Vector3(0, speed, 0);
        }
    }

    /// <summary>
    /// 保证在地面情况下贴在地上
    /// </summary>
    public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

    /// <summary>
    /// 将Player慢慢转向direction
    /// </summary>
    public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);
    /// <summary>
    /// 将Player慢慢转向direction(水中)
    /// </summary>
    public virtual void WaterFaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.waterRotationSpeed);

    /// <summary>
    /// 坠落
    /// </summary>
    public virtual void Fall()
    {
        if (!isGrounded)
        {
            states.Change<FallPlayerState>();
        }
    }

    /// <summary>
    /// 跳跃，提供一个向上的verticalVelocity
    /// </summary>
    public virtual void Jump()
    {
        //多段跳
        //要求：第n次跳跃且在multiJumps次数内
        bool canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
        //土狼跳(腾空跳)
        //要求：第一次跳跃且离开地面coyoteJumpThreshold时间内
        bool canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
        //持物跳跃
        //要求：不能持物，除非开启canJumpWhileHolding
        bool holdJump = !holding || stats.current.canJumpWhileHolding;

        //跳跃条件：
        //1.满足holdJump
        //2.在地面 或 在轨道 或 满足canMultiJump 或 满足 canCoyoteJump
        if ((isGrounded || onRails || canMultiJump || canCoyoteJump) && holdJump)
        {
            if (inputs.GetJumpDown())
            {
                Jump(stats.current.maxJumpHeight);
            }
        }
        //控制跳跃高度(按下就松就会由原来的maxJumpHeight变为minJumpHeight)
        if (inputs.GetJumpUp() && (jumpCounter > 0) && (verticalVelocity.y > stats.current.minJumpHeight))
        {
            verticalVelocity = Vector3.up * stats.current.minJumpHeight;
        }
    }
    public virtual void Jump(float height)
    {
        jumpCounter++;
        verticalVelocity = Vector3.up * height;
        states.Change<FallPlayerState>();
        playerEvents.OnJump?.Invoke();
    }

    /// <summary>
    /// 起跳(不经过额外判断)
    /// </summary>
    public virtual void DirectionalJump(Vector3 direction, float height, float distance)
    {
        jumpCounter++;
        verticalVelocity = Vector3.up * height;
        lateralVelocity = direction * distance;
        playerEvents.OnJump?.Invoke();
    }

    public virtual void ResetAirDash() => airDashCounter = 0;

    public virtual void ResetJumps() => jumpCounter = 0;

    public virtual void SetJumps(int amount) => jumpCounter = amount;

    public virtual void ResetAirSpins() => airSpinCounter = 0;

    /// <summary>
    /// 旋转动作
    /// </summary>
    public virtual void Spin()
    {
        //空中旋转要求：
        //1.!canAirSpin---需要在地上 canAirSpin---地上空中都可以
        //2.没有达到最高空中旋转次数
        var canAirSpin = (isGrounded || stats.current.canAirSpin) && airSpinCounter < stats.current.allowedAirSpins;

        //是否可以旋转要求：
        //1.按下旋转键
        //2.开启canSpin
        //3.不能持物
        //4.必须满足canAirSpin
        if (stats.current.canSpin && canAirSpin && !holding && inputs.GetSpinDown())
        {
            if (!isGrounded)
            {
                airSpinCounter++;//空中旋转计数
            }

            states.Change<SpinPlayerState>();
            playerEvents.OnSpin?.Invoke();
        }
    }

    /// <summary>
    /// 拾取或丢弃
    /// </summary>
    public virtual void PickAndThrow()
    {
        //基础要求：
        //1.开启canPickUp
        //2.按下拾取丢弃键
        if (stats.current.canPickUp && inputs.GetPickAndDropDown())
        {
            if (!holding)//无物即拾取
            {
                if (CapsuleCast(transform.forward, stats.current.pickDistance, out var hit))//检测前方物体
                {
                    if (hit.transform.TryGetComponent(out Pickable pickable))//必须为Pickable物体
                    {
                        PickUp(pickable);
                    }
                }
            }
            else//有物即丢弃
            {
                Throw();
            }
        }
    }
    public virtual void PickUp(Pickable pickable)
    {
        //拾取条件：
        //1.无物
        //2.!canPickUpOnAir时只可在地面，canPickUpOnAir可在地面空中
        if (!holding && (isGrounded || stats.current.canPickUpOnAir))
        {
            holding = true;
            this.pickable = pickable;
            pickable.PickUp(pickableSlot);
            pickable.onRespawn.AddListener(RemovePickable);//重置时不再拾取
            playerEvents.OnPickUp?.Invoke();
        }
    }
    public virtual void Throw()
    {
        //丢弃条件：
        //1.持物
        if (holding)
        {
            float force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
            pickable.Release(transform.forward, force);//向前丢出
            pickable = null;
            holding = false;
            playerEvents.OnThrow?.Invoke();
        }
    }
    public virtual void RemovePickable()
    {
        if (holding)
        {
            pickable = null;
            holding = false;
        }
    }

    /// <summary>
    /// 空中俯冲
    /// </summary>
    public virtual void AirDive()
    {
        //空中俯冲要求：
        //1.开启canAirDive 2.在空中 3.未持物 4.按下空中俯冲按键
        if (stats.current.canAirDive && !isGrounded && !holding && inputs.GetAirDiveDown())
        {
            states.Change<AirDivePlayerState>();
            playerEvents.OnAirDive?.Invoke();
        }
    }

    /// <summary>
    /// 重踩攻击
    /// </summary>
    public virtual void StompAttack()
    {
        //重踩攻击要求：
        //1.开启canStompAttack 2.在空中 3.未持物 4.按下重踩攻击键
        if (stats.current.canStompAttack && !isGrounded && !holding && inputs.GetStompDown())
        {
            states.Change<StompPlayerState>();
        }
    }

    /// <summary>
    /// 抓住边缘
    /// </summary>
    public virtual void LedgeGrab()
    {
        //抓沿要求：
        //1.开启canLedgeHang 2.在下降 3.未持物
        //4.添加了LedgeHangingPlayerState
        //5.是否在边缘
        if (stats.current.canLedgeHang && velocity.y < 0 && !holding &&
            states.ContainsStateOfType(typeof(LedgeHangingPlayerState)) &&
            DetectingLedge(stats.current.ledgeMaxForwardDistance, stats.current.ledgeMaxDownwardDistance, out var hit))
        {
            if (!(hit.collider is CapsuleCollider) && !(hit.collider is SphereCollider))
            {
                float ledgeDistance = radius + stats.current.ledgeMaxForwardDistance;
                Vector3 lateralOffset = transform.forward * ledgeDistance;
                Vector3 verticalOffset = Vector3.down * height * 0.5f - center;

                velocity = Vector3.zero;
                //transform.parent = hit.collider.CompareTag(GameTags.MovingPlatform) ? hit.transform : null;
                onPlatform = hit.collider.CompareTag(GameTags.MovingPlatform) ? true : false;
                transform.position = hit.point - lateralOffset + verticalOffset;//碰撞点向后向下
                Debug.Log(transform.position);
                states.Change<LedgeHangingPlayerState>();
                playerEvents.OnLedgeGrabbed?.Invoke();
            }
        }
    }

    /// <summary>
    /// 后空翻
    /// </summary>
    public virtual void Backflip(float force)
    {
        //后空翻要求：
        //1.开启canBackflip 2.不能持物
        if (stats.current.canBackflip && !holding)
        {
            verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;//向上
            lateralVelocity = -transform.forward * force;//向后
            states.Change<BackflipPlayerState>();
            playerEvents.OnBackflip.Invoke();
        }
    }

    /// <summary>
    /// 冲刺
    /// </summary>
    public virtual void Dash()
    {
        //空中冲刺条件：
        //1.开启canAirDash 2.在空中 3.空中冲刺次数未达到allowedAirDashes
        bool canAirDash = stats.current.canAirDash && !isGrounded &&
            airDashCounter < stats.current.allowedAirDashes;
        //地面冲刺条件：
        //1.开启canGroundDash 2.在地面 3.冷却时间已过
        bool canGroundDash = stats.current.canGroundDash && isGrounded &&
            Time.time - lastDashTime > stats.current.groundDashCoolDown;

        if (inputs.GetDashDown() && (canAirDash || canGroundDash))
        {
            if (!isGrounded) airDashCounter++;

            lastDashTime = Time.time;
            states.Change<DashPlayerState>();
        }
    }

    /// <summary>
    /// 滑翔
    /// </summary>
    public virtual void Glide()
    {
        //滑翔条件：
        //1.开启canGlide
        //2.按下滑翔按键
        //3.在空中
        //4.未持物
        //5.在下落
        if (stats.current.canGlide && !isGrounded && !holding && verticalVelocity.y <= 0 && inputs.GetGlide()) 
            states.Change<GlidingPlayerState>();
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void SetSkinParent(Transform parent)
    {
        if (skin)
        {
            skin.parent = parent;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void ResetSkinParent()
    {
        if (skin)
        {
            skin.parent = transform;
            skin.localPosition = m_skinInitialPosition;
            skin.localRotation = m_skinInitialRotation;
        }
    }

    /// <summary>
    /// 抓墙(抓着墙不断滑落)
    /// </summary>
    public virtual void WallDrag(Collider other)
    {
        //抓墙条件：
        //1.开启canWallDrag 2.下落中 3.未持物 4.碰撞物体非Rigidbody物体(要是墙这种大型物体，不能是箱子这种)
        if (stats.current.canWallDrag && velocity.y <= 0 &&
            !holding && !other.TryGetComponent<Rigidbody>(out _))
        {
            //物体靠近墙面且没有抓住边缘，即可进入WallDrag状态
            if (CapsuleCast(transform.forward, 0.25f, out var hit, stats.current.wallDragLayers)
                && !DetectingLedge(0.25f, height, out _))
            {
                if (hit.collider.CompareTag(GameTags.MovingPlatform))
                    onPlatform = true;
                    //transform.parent = hit.transform;

                lastWallNormal = hit.normal;
                states.Change<WallDragPlayerState>();
            }
        }
    }

    /// <summary>
    /// 推动其它Rigidbody物体
    /// </summary>
    public virtual void PushRigidbody(Collider other)
    {
        //要求：物体比脚高(否则就应该踩上去了)
        if (!IsPointUnderStep(other.bounds.max) &&
            other.TryGetComponent(out Rigidbody rigidbody))
        {
            //提供朝向lateralVelocity方向的力
            Vector3 force = lateralVelocity * stats.current.pushForce;
            rigidbody.velocity += force / rigidbody.mass * Time.deltaTime;
        }
    }

    /// <summary>
    /// 检测是否在边缘
    /// </summary>
    /// <param Name="ledgeHit">边缘检测点(边缘上侧)</param>
    protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
    {
        float contactOffset = Physics.defaultContactOffset + positionDelta;
        float ledgeHeightOffset = height * 0.5f + contactOffset;//上间隔
        float ledgeMaxDistance = radius + forwardDistance;//前间隔
        Vector3 upwardOffset = transform.up * ledgeHeightOffset;
        Vector3 forwardOffset = transform.forward * ledgeMaxDistance;

        //如果Player位置加上偏移还是能射到，说明Player离墙面太远或者过低抓不到边缘
        if (Physics.Raycast(position + upwardOffset, transform.forward, ledgeMaxDistance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(position + forwardOffset * .01f, transform.up, ledgeHeightOffset,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            ledgeHit = new RaycastHit();
            return false;
        }

        Vector3 origin = position + upwardOffset + forwardOffset;
        float distance = downwardDistance + contactOffset;
        //在边缘的上方向下检测
        return Physics.Raycast(origin, Vector3.down, out ledgeHit, distance,
            stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore);
    }

    public virtual void StartGrind()
    {
        states.Change<RailGrindPlayerState>();
    }
}