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

    public Pole pole { get; protected set; }//��ǰ��������
    public Collider water { get; protected set; }//��ǰ����ˮ��
    public Pickable pickable { get; protected set; }//��ǰ��������

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
        if (other.CompareTag(GameTags.VolumeWater))//�������ˮ
        {
            //��ˮ������
            //1.������ˮ�� 2.Player��ˮ�ķ�Χ��
            if (!onWater && other.bounds.Contains(unsizedPosition))
            {
                EnterWater(other);
            }
            //��ˮ������
            //1.��ˮ�� 2.Player����ˮ�ķ�Χ��
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
    /// ����б�����(�ṩһ����ǰ�·�����)
    /// </summary>
    protected override void HandleHighLedge(RaycastHit hit)
    {
        if (onWater) return;

        //���㻬�䷽��
        //Tip������ʹ�õ���SphereCast()������hit����б�������Ȼ����Player���·�����бһ���λ��
        Vector3 edgeNormal = hit.point - position;
        Vector3 edgePushDirection = Vector3.Cross(edgeNormal, Vector3.Cross(edgeNormal, Vector3.up));

        controller.Move(edgePushDirection * stats.current.gravity * Time.deltaTime);
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Respawn()
    {
        health.Reset();
        transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
        states.Change<IdlePlayerState>();
    }
    /// <summary>
    /// ��������λ��
    /// </summary>
    public virtual void SetRespawnPos(Vector3 position, Quaternion rotation)
    {
        m_respawnPosition = position;
        m_respawnRotation = rotation;
    }

    /// <summary>
    /// ����˺�
    /// </summary>
    /// <param Name="origin">����˺������ԭ��</param>
    public override void ApplyDamage(int amount, Vector3 origin)
    {
        //Player��������δ�����޵�״̬
        if (!health.isEmpty && !health.recovering)
        {
            health.Damage(amount);

            Vector3 damageDir = origin - transform.position;//���˷���
            damageDir.y = 0;
            damageDir = damageDir.normalized;
            FaceDirection(damageDir);

            lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;

            if (!onWater)//����ˮ��
            {
                verticalVelocity = Vector3.up * stats.current.hurtUpwardForce;//���ϻ���
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
    /// ��ˮ
    /// </summary>
    public virtual void EnterWater(Collider water)
    {
        //��ˮ������
        //1.��û��ˮ�� 2.������
        if (!onWater && !health.isEmpty)
        {
            Throw();
            onWater = true;
            this.water = water;
            states.Change<SwimPlayerState>();
        }
    }
    /// <summary>
    /// ��ˮ
    /// </summary>
    public virtual void ExitWater()
    {
        if (onWater)
        {
            onWater = false;
        }
    }

    /// <summary>
    /// ץ��
    /// </summary>
    public virtual void GrabPole(Collider other)
    {
        //ץ��������
        //1.����canPoleClimb 2.���½� 3.δ���� 4.��ײ����Ϊ����
        if (stats.current.canPoleClimb && velocity.y <= 0
            && !holding && other.TryGetComponent(out Pole pole))
        {
            this.pole = pole;
            states.Change<PoleClimbingPlayerState>();
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Accelerate(Vector3 direction)
    {
        //��תǣ��---����ڵ����ܲ����ͻ�ʹ��runningTurnningDrag������ΪturningDrag
        float turningDrag = isGrounded && inputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
        //����ٶ�---������ܲ����ͻ�ʹ��runningTopSpeed������ΪtopSpeed
        float topSpeed = inputs.GetRun() ? stats.current.runningTopSpeed : stats.current.topSpeed;
        //���ٶ�---����ڵ����ܲ����ͻ�ʹ��runningAcceleration������Ϊacceleration
        float acceleration = isGrounded && inputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
        //���ռ��ٶ�---����ڵ��棬�ͻ�ʹ��acceleration������ΪairAcceleration
        float finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;

        Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

        if (inputs.GetRunUp())
        {
            lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
        }
    }
    /// <summary>
    /// �����������
    /// </summary>
    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = inputs.GetMovementCameraDirection();
        Accelerate(inputDirection);
    }

    /// <summary>
    /// ˮ�м���
    /// </summary>
    public virtual void WaterAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.waterTurningDrag,
            stats.current.swimAcceleration, stats.current.swimTopSpeed);
    }
    /// <summary>
    /// ����ʱ����
    /// </summary>
    public virtual void CrawlingAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.crawlingTurningSpeed,
            stats.current.crawlingAcceleration, stats.current.crawlingTopSpeed);
    }
    /// <summary>
    /// ��շ�ʱ����
    /// </summary>
    public virtual void BackflipAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.backflipTurningDrag,
            stats.current.backflipAirAcceleration, stats.current.backflipTopSpeed);
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Decelerate() => Decelerate(stats.current.deceleration);

    /// <summary>
    /// Ӧ��б�¿���
    /// </summary>
    public virtual void RegularSlopeFactor()
    {
        if (stats.current.applySlopeFactor)//��Ҫ��������ʹ��
        {
            SlopeFactor(stats.current.slopeUpwardForce, stats.current.slopeDownwardForce);
        }
    }

    /// <summary>
    /// Ħ����
    /// </summary>
    public virtual void Friction()
    {
        if (OnSlopingGround())//б��ʱ
            Decelerate(stats.current.slopeFriction);
        else//��б��ʱ
            Decelerate(stats.current.friction);
    }

    /// <summary>
    /// ʩ��������verticalVelocity���ϴﵽ����
    /// </summary>
    public virtual void Gravity()
    {
        //�����ڵ���ʱ(���������)��ֻҪˤ���ٶ�û�дﵽ���ͻ���Ҫ���е���
        if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
        {
            var speed = verticalVelocity.y;
            var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
            speed -= force * gravityMultiplier * Time.deltaTime;//���µ��ٶ�Խ��Խ��
            speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);//������-gravityTopSpeed
            verticalVelocity = new Vector3(0, speed, 0);
        }
    }

    /// <summary>
    /// ��֤�ڵ�����������ڵ���
    /// </summary>
    public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

    /// <summary>
    /// ��Player����ת��direction
    /// </summary>
    public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);
    /// <summary>
    /// ��Player����ת��direction(ˮ��)
    /// </summary>
    public virtual void WaterFaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.waterRotationSpeed);

    /// <summary>
    /// ׹��
    /// </summary>
    public virtual void Fall()
    {
        if (!isGrounded)
        {
            states.Change<FallPlayerState>();
        }
    }

    /// <summary>
    /// ��Ծ���ṩһ�����ϵ�verticalVelocity
    /// </summary>
    public virtual void Jump()
    {
        //�����
        //Ҫ�󣺵�n����Ծ����multiJumps������
        bool canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
        //������(�ڿ���)
        //Ҫ�󣺵�һ����Ծ���뿪����coyoteJumpThresholdʱ����
        bool canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
        //������Ծ
        //Ҫ�󣺲��ܳ�����ǿ���canJumpWhileHolding
        bool holdJump = !holding || stats.current.canJumpWhileHolding;

        //��Ծ������
        //1.����holdJump
        //2.�ڵ��� �� �ڹ�� �� ����canMultiJump �� ���� canCoyoteJump
        if ((isGrounded || onRails || canMultiJump || canCoyoteJump) && holdJump)
        {
            if (inputs.GetJumpDown())
            {
                Jump(stats.current.maxJumpHeight);
            }
        }
        //������Ծ�߶�(���¾��ɾͻ���ԭ����maxJumpHeight��ΪminJumpHeight)
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
    /// ����(�����������ж�)
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
    /// ��ת����
    /// </summary>
    public virtual void Spin()
    {
        //������תҪ��
        //1.!canAirSpin---��Ҫ�ڵ��� canAirSpin---���Ͽ��ж�����
        //2.û�дﵽ��߿�����ת����
        var canAirSpin = (isGrounded || stats.current.canAirSpin) && airSpinCounter < stats.current.allowedAirSpins;

        //�Ƿ������תҪ��
        //1.������ת��
        //2.����canSpin
        //3.���ܳ���
        //4.��������canAirSpin
        if (stats.current.canSpin && canAirSpin && !holding && inputs.GetSpinDown())
        {
            if (!isGrounded)
            {
                airSpinCounter++;//������ת����
            }

            states.Change<SpinPlayerState>();
            playerEvents.OnSpin?.Invoke();
        }
    }

    /// <summary>
    /// ʰȡ����
    /// </summary>
    public virtual void PickAndThrow()
    {
        //����Ҫ��
        //1.����canPickUp
        //2.����ʰȡ������
        if (stats.current.canPickUp && inputs.GetPickAndDropDown())
        {
            if (!holding)//���Ｔʰȡ
            {
                if (CapsuleCast(transform.forward, stats.current.pickDistance, out var hit))//���ǰ������
                {
                    if (hit.transform.TryGetComponent(out Pickable pickable))//����ΪPickable����
                    {
                        PickUp(pickable);
                    }
                }
            }
            else//���Ｔ����
            {
                Throw();
            }
        }
    }
    public virtual void PickUp(Pickable pickable)
    {
        //ʰȡ������
        //1.����
        //2.!canPickUpOnAirʱֻ���ڵ��棬canPickUpOnAir���ڵ������
        if (!holding && (isGrounded || stats.current.canPickUpOnAir))
        {
            holding = true;
            this.pickable = pickable;
            pickable.PickUp(pickableSlot);
            pickable.onRespawn.AddListener(RemovePickable);//����ʱ����ʰȡ
            playerEvents.OnPickUp?.Invoke();
        }
    }
    public virtual void Throw()
    {
        //����������
        //1.����
        if (holding)
        {
            float force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
            pickable.Release(transform.forward, force);//��ǰ����
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
    /// ���и���
    /// </summary>
    public virtual void AirDive()
    {
        //���и���Ҫ��
        //1.����canAirDive 2.�ڿ��� 3.δ���� 4.���¿��и��尴��
        if (stats.current.canAirDive && !isGrounded && !holding && inputs.GetAirDiveDown())
        {
            states.Change<AirDivePlayerState>();
            playerEvents.OnAirDive?.Invoke();
        }
    }

    /// <summary>
    /// �زȹ���
    /// </summary>
    public virtual void StompAttack()
    {
        //�زȹ���Ҫ��
        //1.����canStompAttack 2.�ڿ��� 3.δ���� 4.�����زȹ�����
        if (stats.current.canStompAttack && !isGrounded && !holding && inputs.GetStompDown())
        {
            states.Change<StompPlayerState>();
        }
    }

    /// <summary>
    /// ץס��Ե
    /// </summary>
    public virtual void LedgeGrab()
    {
        //ץ��Ҫ��
        //1.����canLedgeHang 2.���½� 3.δ����
        //4.�����LedgeHangingPlayerState
        //5.�Ƿ��ڱ�Ե
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
                transform.position = hit.point - lateralOffset + verticalOffset;//��ײ���������
                Debug.Log(transform.position);
                states.Change<LedgeHangingPlayerState>();
                playerEvents.OnLedgeGrabbed?.Invoke();
            }
        }
    }

    /// <summary>
    /// ��շ�
    /// </summary>
    public virtual void Backflip(float force)
    {
        //��շ�Ҫ��
        //1.����canBackflip 2.���ܳ���
        if (stats.current.canBackflip && !holding)
        {
            verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;//����
            lateralVelocity = -transform.forward * force;//���
            states.Change<BackflipPlayerState>();
            playerEvents.OnBackflip.Invoke();
        }
    }

    /// <summary>
    /// ���
    /// </summary>
    public virtual void Dash()
    {
        //���г��������
        //1.����canAirDash 2.�ڿ��� 3.���г�̴���δ�ﵽallowedAirDashes
        bool canAirDash = stats.current.canAirDash && !isGrounded &&
            airDashCounter < stats.current.allowedAirDashes;
        //������������
        //1.����canGroundDash 2.�ڵ��� 3.��ȴʱ���ѹ�
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
    /// ����
    /// </summary>
    public virtual void Glide()
    {
        //����������
        //1.����canGlide
        //2.���»��谴��
        //3.�ڿ���
        //4.δ����
        //5.������
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
    /// ץǽ(ץ��ǽ���ϻ���)
    /// </summary>
    public virtual void WallDrag(Collider other)
    {
        //ץǽ������
        //1.����canWallDrag 2.������ 3.δ���� 4.��ײ�����Rigidbody����(Ҫ��ǽ���ִ������壬��������������)
        if (stats.current.canWallDrag && velocity.y <= 0 &&
            !holding && !other.TryGetComponent<Rigidbody>(out _))
        {
            //���忿��ǽ����û��ץס��Ե�����ɽ���WallDrag״̬
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
    /// �ƶ�����Rigidbody����
    /// </summary>
    public virtual void PushRigidbody(Collider other)
    {
        //Ҫ������ȽŸ�(�����Ӧ�ò���ȥ��)
        if (!IsPointUnderStep(other.bounds.max) &&
            other.TryGetComponent(out Rigidbody rigidbody))
        {
            //�ṩ����lateralVelocity�������
            Vector3 force = lateralVelocity * stats.current.pushForce;
            rigidbody.velocity += force / rigidbody.mass * Time.deltaTime;
        }
    }

    /// <summary>
    /// ����Ƿ��ڱ�Ե
    /// </summary>
    /// <param Name="ledgeHit">��Ե����(��Ե�ϲ�)</param>
    protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
    {
        float contactOffset = Physics.defaultContactOffset + positionDelta;
        float ledgeHeightOffset = height * 0.5f + contactOffset;//�ϼ��
        float ledgeMaxDistance = radius + forwardDistance;//ǰ���
        Vector3 upwardOffset = transform.up * ledgeHeightOffset;
        Vector3 forwardOffset = transform.forward * ledgeMaxDistance;

        //���Playerλ�ü���ƫ�ƻ������䵽��˵��Player��ǽ��̫Զ���߹���ץ������Ե
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
        //�ڱ�Ե���Ϸ����¼��
        return Physics.Raycast(origin, Vector3.down, out ledgeHit, distance,
            stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore);
    }

    public virtual void StartGrind()
    {
        states.Change<RailGrindPlayerState>();
    }
}