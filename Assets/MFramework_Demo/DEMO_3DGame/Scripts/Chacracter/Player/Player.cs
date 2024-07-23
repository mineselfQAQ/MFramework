using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerStatsManager))]
[RequireComponent(typeof(PlayerStateManager))]
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
        if (other.CompareTag(GameTags.VolumeWater))
        {
            if (!onWater && other.bounds.Contains(unsizedPosition))
            {
                EnterWater(other);
            }
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

    protected override void HandleSlopeLimit(RaycastHit hit)
    {
        if (onWater) return;

        var slopeDirection = Vector3.Cross(hit.normal, Vector3.Cross(hit.normal, Vector3.up));
        slopeDirection = slopeDirection.normalized;
        controller.Move(slopeDirection * stats.current.slideForce * Time.deltaTime);
    }

    protected override void HandleHighLedge(RaycastHit hit)
    {
        if (onWater) return;

        var edgeNormal = hit.point - position;
        var edgePushDirection = Vector3.Cross(edgeNormal, Vector3.Cross(edgeNormal, Vector3.up));
        controller.Move(edgePushDirection * stats.current.gravity * Time.deltaTime);
    }

    public virtual void Respawn()
    {
        health.Reset();
        transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
        states.Change<IdlePlayerState>();
    }

    public virtual void SetRespawn(Vector3 position, Quaternion rotation)
    {
        m_respawnPosition = position;
        m_respawnRotation = rotation;
    }

    public override void ApplyDamage(int amount, Vector3 origin)
    {
        if (!health.isEmpty && !health.recovering)
        {
            health.Damage(amount);
            var damageDir = origin - transform.position;
            damageDir.y = 0;
            damageDir = damageDir.normalized;
            FaceDirection(damageDir);
            lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;

            if (!onWater)
            {
                verticalVelocity = Vector3.up * stats.current.hurtUpwardForce;
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

    public virtual void EnterWater(Collider water)
    {
        if (!onWater && !health.isEmpty)
        {
            Throw();
            onWater = true;
            this.water = water;
            states.Change<SwimPlayerState>();
        }
    }

    public virtual void ExitWater()
    {
        if (onWater)
        {
            onWater = false;
        }
    }

    public virtual void GrabPole(Collider other)
    {
        if (stats.current.canPoleClimb && velocity.y <= 0
            && !holding && other.TryGetComponent(out Pole pole))
        {
            this.pole = pole;
            states.Change<PoleClimbingPlayerState>();
        }
    }

    public virtual void Accelerate(Vector3 direction)
    {
        var turningDrag = isGrounded && inputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
        var acceleration = isGrounded && inputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
        var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
        var topSpeed = inputs.GetRun() ? stats.current.runningTopSpeed : stats.current.topSpeed;

        Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

        if (inputs.GetRunUp())
        {
            lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
        }
    }

    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = inputs.GetMovementCameraDirection();
        Accelerate(inputDirection);
    }

    public virtual void RegularSlopeFactor()
    {
        if (stats.current.applySlopeFactor)
        {
            SlopeFactor(stats.current.slopeUpwardForce, stats.current.slopeDownwardForce);
        }
    }

    public virtual void WaterAcceleration(Vector3 direction) =>
        Accelerate(direction, stats.current.waterTurningDrag, stats.current.swimAcceleration, stats.current.swimTopSpeed);

    public virtual void CrawlingAccelerate(Vector3 direction) =>
        Accelerate(direction, stats.current.crawlingTurningSpeed, stats.current.crawlingAcceleration, stats.current.crawlingTopSpeed);

    public virtual void BackflipAcceleration()
    {
        var direction = inputs.GetMovementCameraDirection();
        Accelerate(direction, stats.current.backflipTurningDrag, stats.current.backflipAirAcceleration, stats.current.backflipTopSpeed);
    }

    public virtual void Decelerate() => Decelerate(stats.current.deceleration);

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

    public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);

    public virtual void WaterFaceDirection(Vector3 direction) => FaceDirection(direction, stats.current.waterRotationSpeed);

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

        //TODO:???????????感觉没意义
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

    public virtual void AirDive()
    {
        if (stats.current.canAirDive && !isGrounded && !holding && inputs.GetAirDiveDown())
        {
            states.Change<AirDivePlayerState>();
            playerEvents.OnAirDive?.Invoke();
        }
    }

    public virtual void StompAttack()
    {
        if (!isGrounded && !holding && stats.current.canStompAttack && inputs.GetStompDown())
        {
            states.Change<StompPlayerState>();
        }
    }

    public virtual void LedgeGrab()
    {
        if (stats.current.canLedgeHang && velocity.y < 0 && !holding &&
            states.ContainsStateOfType(typeof(LedgeHangingPlayerState)) &&
            DetectingLedge(stats.current.ledgeMaxForwardDistance, stats.current.ledgeMaxDownwardDistance, out var hit))
        {
            if (!(hit.collider is CapsuleCollider) && !(hit.collider is SphereCollider))
            {
                var ledgeDistance = radius + stats.current.ledgeMaxForwardDistance;
                var lateralOffset = transform.forward * ledgeDistance;
                var verticalOffset = Vector3.down * height * 0.5f - center;
                velocity = Vector3.zero;
                transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : null;
                transform.position = hit.point - lateralOffset + verticalOffset;
                states.Change<LedgeHangingPlayerState>();
                playerEvents.OnLedgeGrabbed?.Invoke();
            }
        }
    }

    public virtual void Backflip(float force)
    {
        if (stats.current.canBackflip && !holding)
        {
            verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;
            lateralVelocity = -transform.forward * force;
            states.Change<BackflipPlayerState>();
            playerEvents.OnBackflip.Invoke();
        }
    }

    public virtual void Dash()
    {
        var canAirDash = stats.current.canAirDash && !isGrounded &&
            airDashCounter < stats.current.allowedAirDashes;
        var canGroundDash = stats.current.canGroundDash && isGrounded &&
            Time.time - lastDashTime > stats.current.groundDashCoolDown;

        if (inputs.GetDashDown() && (canAirDash || canGroundDash))
        {
            if (!isGrounded) airDashCounter++;

            lastDashTime = Time.time;
            states.Change<DashPlayerState>();
        }
    }

    public virtual void Glide()
    {
        if (!isGrounded && inputs.GetGlide() &&
            verticalVelocity.y <= 0 && stats.current.canGlide)
            states.Change<GlidingPlayerState>();
    }

    public virtual void SetSkinParent(Transform parent)
    {
        if (skin)
        {
            skin.parent = parent;
        }
    }

    public virtual void ResetSkinParent()
    {
        if (skin)
        {
            skin.parent = transform;
            skin.localPosition = m_skinInitialPosition;
            skin.localRotation = m_skinInitialRotation;
        }
    }

    public virtual void WallDrag(Collider other)
    {
        if (stats.current.canWallDrag && velocity.y <= 0 &&
            !holding && !other.TryGetComponent<Rigidbody>(out _))
        {
            if (CapsuleCast(transform.forward, 0.25f, out var hit,
                stats.current.wallDragLayers) && !DetectingLedge(0.25f, height, out _))
            {
                if (hit.collider.CompareTag(GameTags.Platform))
                    transform.parent = hit.transform;

                lastWallNormal = hit.normal;
                states.Change<WallDragPlayerState>();
            }
        }
    }

    public virtual void PushRigidbody(Collider other)
    {
        if (!IsPointUnderStep(other.bounds.max) &&
            other.TryGetComponent(out Rigidbody rigidbody))
        {
            var force = lateralVelocity * stats.current.pushForce;
            rigidbody.velocity += force / rigidbody.mass * Time.deltaTime;
        }
    }

    protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
    {
        var contactOffset = Physics.defaultContactOffset + positionDelta;
        var ledgeMaxDistance = radius + forwardDistance;
        var ledgeHeightOffset = height * 0.5f + contactOffset;
        var upwardOffset = transform.up * ledgeHeightOffset;
        var forwardOffset = transform.forward * ledgeMaxDistance;

        if (Physics.Raycast(position + upwardOffset, transform.forward, ledgeMaxDistance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(position + forwardOffset * .01f, transform.up, ledgeHeightOffset,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            ledgeHit = new RaycastHit();
            return false;
        }

        var origin = position + upwardOffset + forwardOffset;
        var distance = downwardDistance + contactOffset;

        return Physics.Raycast(origin, Vector3.down, out ledgeHit, distance,
            stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore);
    }

    public virtual void StartGrind() => states.Change<RailGrindPlayerState>();
}