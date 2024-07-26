using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 实体泛型类，生物的基类
/// </summary>
[RequireComponent(typeof(CharacterController))]
public abstract class Entity<T> : Entity where T : Entity<T>
{
    protected IEntityContact[] m_contacts;

    public EntityStateManager<T> states { get; protected set; }

    protected virtual void Awake()
    {
        InitializeController();//初始化CharacterController
        InitializePenetratorCollider();//初始化BoxCollider
        InitializeStateManager();//获取EntityStateManager
    }

    protected virtual void Update()
    {
        if (controller.enabled || m_collider != null)
        {
            HandleStates();//状态机
            HandleController();//CharacterController
            HandleSpline();//轨道
            HandleGround();//地面检测
            HandleContacts();//交互
            OnUpdate();//其它事件
        }
    }

    protected virtual void LateUpdate()
    {
        if (controller.enabled)
        {
            HandlePosition();//位置信息
            HandlePenetration();//重叠处理
        }
    }

    #region 初始化操作
    protected virtual void InitializeController()
    {
        controller = GetComponent<CharacterController>();

        controller.skinWidth = 0.005f;
        controller.minMoveDistance = 0;
        originalHeight = controller.height;
    }

    protected virtual void InitializePenetratorCollider()
    {
        var xzSize = radius * 2f - controller.skinWidth;
        m_penetratorCollider = gameObject.AddComponent<BoxCollider>();
        m_penetratorCollider.size = new Vector3(xzSize, height - controller.stepOffset, xzSize);
        m_penetratorCollider.center = center + Vector3.up * controller.stepOffset * 0.5f;
        m_penetratorCollider.isTrigger = true;
    }

    protected virtual void InitializeCollider()
    {
        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_collider.height = controller.height;
        m_collider.radius = controller.radius;
        m_collider.center = controller.center;
        m_collider.isTrigger = true;
        m_collider.enabled = false;
    }

    protected virtual void InitializeRigidbody()
    {
        m_rigidbody = gameObject.AddComponent<Rigidbody>();
        m_rigidbody.isKinematic = true;
    }

    protected virtual void InitializeStateManager()
    {
        states = GetComponent<EntityStateManager<T>>();
    }
    #endregion

    #region 逐帧更新操作
    protected virtual void HandleStates()
    {
        states.Step();
    }

    protected virtual void HandleController()
    {
        if (controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

    protected virtual void HandleSpline()
    {
        var distance = (height * 0.5f) + height * 0.5f;

        if (SphereCast(-transform.up, distance, out var hit) &&
            hit.collider.CompareTag(GameTags.InteractiveRail))
        {
            if (!onRails && verticalVelocity.y <= 0)
            {
                EnterRail(hit.collider.GetComponent<SplineContainer>());
            }
        }
        else
        {
            ExitRail();
        }
    }

    protected virtual void HandleGround()
    {
        if (onRails) return;

        var distance = (height * 0.5f) + m_groundOffset;

        //射到东西
        if (SphereCast(Vector3.down, distance, out var hit) && verticalVelocity.y <= 0)
        {
            if (!isGrounded)//当前不在地面
            {
                if (EvaluateLanding(hit))
                {
                    EnterGround(hit);
                }
                else
                {
                    HandleHighLedge(hit);
                }
            }
            else if (IsPointUnderStep(hit.point))//物体过低？？？
            {
                UpdateGround(hit);

                if (Vector3.Angle(hit.normal, Vector3.up) >= controller.slopeLimit)
                {
                    HandleSlopeLimit(hit);
                }
            }
            else
            {
                HandleHighLedge(hit);
            }
        }
        else//没有射到
        {
            //说明下方没有物体，也就是离开地面
            ExitGround();
        }
    }

    protected virtual void HandleContacts()
    {
        int overlapNum = OverlapEntity(m_contactBuffer);//更新m_contactBuffer

        for (int i = 0; i < overlapNum; i++)
        {
            //接触物体必须是物体(非触发器)且不是该Entity
            if (!m_contactBuffer[i].isTrigger && m_contactBuffer[i].transform != transform)
            {
                //状态机(EntityStateManager)中的状态(EntityState)处理交互
                OnContact(m_contactBuffer[i]);

                //具有IEntityContact功能的物体的交互处理
                var listeners = m_contactBuffer[i].GetComponents<IEntityContact>();
                foreach (var contact in listeners)
                {
                    contact.OnEntityContact((T)this);
                }

                //TODO:??????????????????????????????????
                if (m_contactBuffer[i].bounds.min.y > controller.bounds.max.y)
                {
                    verticalVelocity = Vector3.Min(verticalVelocity, Vector3.zero);
                }
            }
        }
    }

    protected virtual void HandlePosition()
    {
        positionDelta = (position - lastPosition).magnitude;//两帧移动距离
        lastPosition = position;//更新
    }

    protected virtual void HandlePenetration()
    {
        var xzSize = m_penetratorCollider.size.x * 0.5f;
        var ySize = (height - controller.stepOffset * 0.5f) * 0.5f;
        var origin = position + Vector3.up * controller.stepOffset * 0.5f;
        var halfExtents = new Vector3(xzSize, ySize, xzSize);
        var overlapNum = Physics.OverlapBoxNonAlloc(origin, halfExtents, m_penetrationBuffer,
            Quaternion.identity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < overlapNum; i++)
        {
            if (!m_penetrationBuffer[i].isTrigger && m_penetrationBuffer[i].transform != transform &&
                (lateralVelocity.sqrMagnitude == 0 || m_penetrationBuffer[i].CompareTag(GameTags.Platform)))
            {
                //大致就是：
                //Entity可能穿插到某个物体中，需要将其拉回来
                //注意：处理的是平面上移动速度过快的情况
                if (Physics.ComputePenetration(m_penetratorCollider, position, Quaternion.identity,
                    m_penetrationBuffer[i], m_penetrationBuffer[i].transform.position,
                    m_penetrationBuffer[i].transform.rotation, out var direction, out float distance))
                {
                    var pushDirection = new Vector3(direction.x, 0, direction.z).normalized;
                    transform.position += pushDirection * distance;
                }
            }
        }
    }

    protected virtual void OnContact(Collider other)
    {
        if (other)
        {
            states.OnContact(other);
        }
    }

    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        return IsPointUnderStep(hit.point) && Vector3.Angle(hit.normal, Vector3.up) < controller.slopeLimit;
    }

    protected virtual void EnterGround(RaycastHit hit)
    {
        if (!isGrounded)
        {
            groundHit = hit;
            isGrounded = true;
            entityEvents.OnGroundEnter?.Invoke();
        }
    }
    protected virtual void UpdateGround(RaycastHit hit)
    {
        if (isGrounded)
        {
            groundHit = hit;
            groundNormal = groundHit.normal;
            groundAngle = Vector3.Angle(Vector3.up, groundHit.normal);
            localSlopeDirection = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
            transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : null;
        }
    }
    protected virtual void ExitGround()
    {
        if (isGrounded)
        {
            isGrounded = false;
            transform.parent = null;
            lastGroundTime = Time.time;
            verticalVelocity = Vector3.Max(verticalVelocity, Vector3.zero);
            entityEvents.OnGroundExit?.Invoke();
        }
    }

    protected virtual void EnterRail(SplineContainer rails)
    {
        if (!onRails)
        {
            onRails = true;
            this.rails = rails;
            entityEvents.OnRailsEnter.Invoke();
        }
    }
    public virtual void ExitRail()
    {
        if (onRails)
        {
            onRails = false;
            entityEvents.OnRailsExit.Invoke();
        }
    }

    protected virtual void HandleSlopeLimit(RaycastHit hit) { }
    protected virtual void HandleHighLedge(RaycastHit hit) { }
    protected virtual void OnUpdate() { }
    #endregion

    /// <summary>
    /// 加速
    /// </summary>
    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        //Tip: direction应该是归一化的
        if (direction.sqrMagnitude > 0)//有输入
        {
            float speed = Vector3.Dot(direction, lateralVelocity);//方向越一致，速度越快
            Vector3 velocity = direction * speed;//输入速度(带方向)=路程*速度
            Vector3 turningVelocity = lateralVelocity - velocity;//补正速度(带方向)
            float turningDelta = turningDrag * turningDragMultiplier * Time.deltaTime;//每帧的补正的修正(为了减小值的大小)
            float targetTopSpeed = topSpeed * topSpeedMultiplier;//目标最大速度

            //方向不正确，速度要慢慢地起来，一直到达最高速度
            if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
            {
                speed += acceleration * accelerationMultiplier * Time.deltaTime;
                speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
            }

            velocity = direction * speed;//重计算速度
            //重计算补正速度(将turningVelocity向原点拉回(由turningDelta限制大小，默认情况变化可以忽略不计))
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            lateralVelocity = velocity + turningVelocity;//更新速度(大致为输入方向靠上帧方向偏移)
        }
    }

    /// <summary>
    /// 减速
    /// </summary>
    public virtual void Decelerate(float deceleration)
    {
        var delta = deceleration * decelerationMultiplier * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);//向原点拉回delta
    }

    public virtual void Gravity(float gravity)
    {
        if (!isGrounded)
        {
            verticalVelocity += Vector3.down * gravity * gravityMultiplier * Time.deltaTime;
        }
    }

    //TODO:???????????????????????
    public virtual void SlopeFactor(float upwardForce, float downwardForce)
    {
        //必须在斜坡上
        if (!isGrounded || !OnSlopingGround()) return;

        float factor = Vector3.Dot(Vector3.up, groundNormal);//越平越大
        bool downwards = Vector3.Dot(localSlopeDirection, lateralVelocity) > 0;
        float multiplier = downwards ? downwardForce : upwardForce;
        float delta = factor * multiplier * Time.deltaTime;
        lateralVelocity += localSlopeDirection * delta;
    }

    /// <summary>
    /// 保证在地面情况下贴在地上
    /// </summary>
    public virtual void SnapToGround(float force)
    {
        if (isGrounded && (verticalVelocity.y <= 0))
        {
            verticalVelocity = Vector3.down * force;
        }
    }

    /// <summary>
    /// 将Entity直接转向direction
    /// </summary>
    public virtual void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = rotation;
        }
    }
    /// <summary>
    /// 将Entity慢慢转向direction
    /// </summary>
    public virtual void FaceDirection(Vector3 direction, float degreesPerSecond)
    {
        if (direction != Vector3.zero)
        {
            var rotation = transform.rotation;
            float rotationDelta = degreesPerSecond * Time.deltaTime;
            var target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(rotation, target, rotationDelta);
        }
    }

    public virtual bool FitsIntoPosition(Vector3 position)
    {
        var bounds = controller.bounds;
        var radius = controller.radius - controller.skinWidth;
        var offset = height * 0.5f - radius;
        var top = position + Vector3.up * offset;
        var bottom = position - Vector3.up * offset;

        return !Physics.CheckCapsule(top, bottom, radius,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }

    public virtual void UseCustomCollision(bool value)
    {
        controller.enabled = !value;

        if (value)
        {
            InitializeCollider();
            InitializeRigidbody();
        }
        else
        {
            Destroy(m_collider);
            Destroy(m_rigidbody);
        }
    }
}

/// <summary>
/// 实体类，Entity<T>父类，以收集信息为主
/// </summary>
public abstract class Entity : MonoBehaviour
{
    #region 公开变量
    public EntityEvents entityEvents;
    #endregion

    #region 私有变量
    protected Collider[] m_contactBuffer = new Collider[10];
    protected Collider[] m_penetrationBuffer = new Collider[32];

    protected readonly float m_groundOffset = 0.1f;
    protected readonly float m_penetrationOffset = -0.1f;
    protected readonly float m_slopingGroundAngle = 20f;

    protected CapsuleCollider m_collider;
    protected BoxCollider m_penetratorCollider;

    protected Rigidbody m_rigidbody;
    #endregion

    #region 公开属性
    public CharacterController controller { get; protected set; }
    /// <summary>
    /// Entity胶囊体高度
    /// </summary>
    public float height => controller.height;
    /// <summary>
    /// Entity胶囊体半径
    /// </summary>
    public float radius => controller.radius;
    public Vector3 center => controller.center;

    public Vector3 velocity { get; set; }
    /// <summary>
    /// 横向速度(Entity面向方向为正)
    /// </summary>
    public Vector3 lateralVelocity
    {
        get { return new Vector3(velocity.x, 0, velocity.z); }
        set { velocity = new Vector3(value.x, velocity.y, value.z); }
    }
    /// <summary>
    /// 纵向速度
    /// </summary>
    public Vector3 verticalVelocity
    {
        get { return new Vector3(0, velocity.y, 0); }
        set { velocity = new Vector3(velocity.x, value.y, velocity.z); }
    }

    public Vector3 lastPosition { get; set; }
    public Vector3 position => transform.position + center;
    /// <summary>
    /// 原始position(即使蹲下也能得到站着的位置)
    /// </summary>
    public Vector3 unsizedPosition => position - transform.up * height * 0.5f + transform.up * originalHeight * 0.5f;
    /// <summary>
    /// 阶梯可允许位置(高于该位置的Entity不可直接走上去)
    /// </summary>
    public Vector3 stepPosition => position - transform.up * (height * 0.5f - controller.stepOffset);

    public float positionDelta { get; protected set; }
    public float lastGroundTime { get; protected set; }
    public bool isGrounded { get; protected set; } = true;
    public bool onRails { get; set; }

    public float accelerationMultiplier { get; set; } = 1f;
    public float gravityMultiplier { get; set; } = 1f;
    public float topSpeedMultiplier { get; set; } = 1f;
    public float turningDragMultiplier { get; set; } = 1f;
    public float decelerationMultiplier { get; set; } = 1f;

    public RaycastHit groundHit;
    public SplineContainer rails { get; protected set; }
    public float groundAngle { get; protected set; }
    public Vector3 groundNormal { get; protected set; }
    public Vector3 localSlopeDirection { get; protected set; }
    public float originalHeight { get; protected set; }
    #endregion

    /// <summary>
    /// 确定该点在楼梯允许范围下还是上
    /// </summary>
    /// <returns>True为点在阶梯下，False为点在阶梯上</returns>
    public virtual bool IsPointUnderStep(Vector3 point) => stepPosition.y > point.y;

    /// <summary>
    /// 是否在斜坡上
    /// </summary>
    public virtual bool OnSlopingGround()
    {
        //基础要求：
        //1.在地面上
        //2.地面角度大于m_slopingGroundAngle(说明这是一个斜坡)
        if (isGrounded && groundAngle > m_slopingGroundAngle)
        {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, height * 2f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return Vector3.Angle(hit.normal, Vector3.up) > m_slopingGroundAngle;
            else//过斜可能射不到，必是
                return true;
        }

        return false;
    }

    /// <summary>
    /// 重计算Collider大小
    /// </summary>
    public virtual void ResizeCollider(float height)
    {
        //偏移量
        //当当前高度>height，为负(下降)，反之
        var delta = height - this.height;

        controller.height = height;
        controller.center += Vector3.up * delta * 0.5f;
    }

    /// <summary>
    /// 投射胶囊形检测(不输出RaycastHit)
    /// </summary>
    public virtual bool CapsuleCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        return CapsuleCast(direction, distance, out _, layer, queryTriggerInteraction);
    }
    /// <summary>
    /// 投射胶囊形检测
    /// </summary>
    public virtual bool CapsuleCast(Vector3 direction, float distance,
        out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var origin = position - direction * radius + center;
        var offset = transform.up * (height * 0.5f - radius);
        var top = origin + offset;
        var bottom = origin - offset;
        return Physics.CapsuleCast(top, bottom, radius, direction,
            out hit, distance + radius, layer, queryTriggerInteraction);
    }
    /// <summary>
    /// 投射球形检测(不输出RaycastHit)
    /// </summary>
    public virtual bool SphereCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        return SphereCast(direction, distance, out _, layer, queryTriggerInteraction);
    }
    /// <summary>
    /// 投射球形检测
    /// </summary>
    public virtual bool SphereCast(Vector3 direction, float distance,
        out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var castDistance = Mathf.Abs(distance - radius);
        return Physics.SphereCast(position, radius, direction,
            out hit, castDistance, layer, queryTriggerInteraction);
    }

    /// <summary>
    /// 获取所有与Entity重叠的物体
    /// </summary>
    /// Tip：result是接收值
    public virtual int OverlapEntity(Collider[] result, float inputOffset = 0)
    {
        float contactOffset = inputOffset + controller.skinWidth + Physics.defaultContactOffset;
        float overlapsRadius = radius + contactOffset;//半径
        float offset = (height + contactOffset) * 0.5f - overlapsRadius;
        Vector3 top = position + Vector3.up * offset;//上侧圆心
        Vector3 bottom = position + Vector3.down * offset;//下侧圆心
        return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
    }

    public virtual void ApplyDamage(int damage, Vector3 origin) { }
}