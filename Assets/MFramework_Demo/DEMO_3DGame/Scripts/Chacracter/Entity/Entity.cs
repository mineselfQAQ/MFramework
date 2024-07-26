using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// ʵ�巺���࣬����Ļ���
/// </summary>
[RequireComponent(typeof(CharacterController))]
public abstract class Entity<T> : Entity where T : Entity<T>
{
    protected IEntityContact[] m_contacts;

    public EntityStateManager<T> states { get; protected set; }

    protected virtual void Awake()
    {
        InitializeController();//��ʼ��CharacterController
        InitializePenetratorCollider();//��ʼ��BoxCollider
        InitializeStateManager();//��ȡEntityStateManager
    }

    protected virtual void Update()
    {
        if (controller.enabled || m_collider != null)
        {
            HandleStates();//״̬��
            HandleController();//CharacterController
            HandleSpline();//���
            HandleGround();//������
            HandleContacts();//����
            OnUpdate();//�����¼�
        }
    }

    protected virtual void LateUpdate()
    {
        if (controller.enabled)
        {
            HandlePosition();//λ����Ϣ
            HandlePenetration();//�ص�����
        }
    }

    #region ��ʼ������
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

    #region ��֡���²���
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

        //�䵽����
        if (SphereCast(Vector3.down, distance, out var hit) && verticalVelocity.y <= 0)
        {
            if (!isGrounded)//��ǰ���ڵ���
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
            else if (IsPointUnderStep(hit.point))//������ͣ�����
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
        else//û���䵽
        {
            //˵���·�û�����壬Ҳ�����뿪����
            ExitGround();
        }
    }

    protected virtual void HandleContacts()
    {
        int overlapNum = OverlapEntity(m_contactBuffer);//����m_contactBuffer

        for (int i = 0; i < overlapNum; i++)
        {
            //�Ӵ��������������(�Ǵ�����)�Ҳ��Ǹ�Entity
            if (!m_contactBuffer[i].isTrigger && m_contactBuffer[i].transform != transform)
            {
                //״̬��(EntityStateManager)�е�״̬(EntityState)������
                OnContact(m_contactBuffer[i]);

                //����IEntityContact���ܵ�����Ľ�������
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
        positionDelta = (position - lastPosition).magnitude;//��֡�ƶ�����
        lastPosition = position;//����
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
                //���¾��ǣ�
                //Entity���ܴ��嵽ĳ�������У���Ҫ����������
                //ע�⣺�������ƽ�����ƶ��ٶȹ�������
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
    /// ����
    /// </summary>
    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        //Tip: directionӦ���ǹ�һ����
        if (direction.sqrMagnitude > 0)//������
        {
            float speed = Vector3.Dot(direction, lateralVelocity);//����Խһ�£��ٶ�Խ��
            Vector3 velocity = direction * speed;//�����ٶ�(������)=·��*�ٶ�
            Vector3 turningVelocity = lateralVelocity - velocity;//�����ٶ�(������)
            float turningDelta = turningDrag * turningDragMultiplier * Time.deltaTime;//ÿ֡�Ĳ���������(Ϊ�˼�Сֵ�Ĵ�С)
            float targetTopSpeed = topSpeed * topSpeedMultiplier;//Ŀ������ٶ�

            //������ȷ���ٶ�Ҫ������������һֱ��������ٶ�
            if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
            {
                speed += acceleration * accelerationMultiplier * Time.deltaTime;
                speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
            }

            velocity = direction * speed;//�ؼ����ٶ�
            //�ؼ��㲹���ٶ�(��turningVelocity��ԭ������(��turningDelta���ƴ�С��Ĭ������仯���Ժ��Բ���))
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            lateralVelocity = velocity + turningVelocity;//�����ٶ�(����Ϊ���뷽����֡����ƫ��)
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public virtual void Decelerate(float deceleration)
    {
        var delta = deceleration * decelerationMultiplier * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);//��ԭ������delta
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
        //������б����
        if (!isGrounded || !OnSlopingGround()) return;

        float factor = Vector3.Dot(Vector3.up, groundNormal);//ԽƽԽ��
        bool downwards = Vector3.Dot(localSlopeDirection, lateralVelocity) > 0;
        float multiplier = downwards ? downwardForce : upwardForce;
        float delta = factor * multiplier * Time.deltaTime;
        lateralVelocity += localSlopeDirection * delta;
    }

    /// <summary>
    /// ��֤�ڵ�����������ڵ���
    /// </summary>
    public virtual void SnapToGround(float force)
    {
        if (isGrounded && (verticalVelocity.y <= 0))
        {
            verticalVelocity = Vector3.down * force;
        }
    }

    /// <summary>
    /// ��Entityֱ��ת��direction
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
    /// ��Entity����ת��direction
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
/// ʵ���࣬Entity<T>���࣬���ռ���ϢΪ��
/// </summary>
public abstract class Entity : MonoBehaviour
{
    #region ��������
    public EntityEvents entityEvents;
    #endregion

    #region ˽�б���
    protected Collider[] m_contactBuffer = new Collider[10];
    protected Collider[] m_penetrationBuffer = new Collider[32];

    protected readonly float m_groundOffset = 0.1f;
    protected readonly float m_penetrationOffset = -0.1f;
    protected readonly float m_slopingGroundAngle = 20f;

    protected CapsuleCollider m_collider;
    protected BoxCollider m_penetratorCollider;

    protected Rigidbody m_rigidbody;
    #endregion

    #region ��������
    public CharacterController controller { get; protected set; }
    /// <summary>
    /// Entity������߶�
    /// </summary>
    public float height => controller.height;
    /// <summary>
    /// Entity������뾶
    /// </summary>
    public float radius => controller.radius;
    public Vector3 center => controller.center;

    public Vector3 velocity { get; set; }
    /// <summary>
    /// �����ٶ�(Entity������Ϊ��)
    /// </summary>
    public Vector3 lateralVelocity
    {
        get { return new Vector3(velocity.x, 0, velocity.z); }
        set { velocity = new Vector3(value.x, velocity.y, value.z); }
    }
    /// <summary>
    /// �����ٶ�
    /// </summary>
    public Vector3 verticalVelocity
    {
        get { return new Vector3(0, velocity.y, 0); }
        set { velocity = new Vector3(velocity.x, value.y, velocity.z); }
    }

    public Vector3 lastPosition { get; set; }
    public Vector3 position => transform.position + center;
    /// <summary>
    /// ԭʼposition(��ʹ����Ҳ�ܵõ�վ�ŵ�λ��)
    /// </summary>
    public Vector3 unsizedPosition => position - transform.up * height * 0.5f + transform.up * originalHeight * 0.5f;
    /// <summary>
    /// ���ݿ�����λ��(���ڸ�λ�õ�Entity����ֱ������ȥ)
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
    /// ȷ���õ���¥������Χ�»�����
    /// </summary>
    /// <returns>TrueΪ���ڽ����£�FalseΪ���ڽ�����</returns>
    public virtual bool IsPointUnderStep(Vector3 point) => stepPosition.y > point.y;

    /// <summary>
    /// �Ƿ���б����
    /// </summary>
    public virtual bool OnSlopingGround()
    {
        //����Ҫ��
        //1.�ڵ�����
        //2.����Ƕȴ���m_slopingGroundAngle(˵������һ��б��)
        if (isGrounded && groundAngle > m_slopingGroundAngle)
        {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, height * 2f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return Vector3.Angle(hit.normal, Vector3.up) > m_slopingGroundAngle;
            else//��б�����䲻��������
                return true;
        }

        return false;
    }

    /// <summary>
    /// �ؼ���Collider��С
    /// </summary>
    public virtual void ResizeCollider(float height)
    {
        //ƫ����
        //����ǰ�߶�>height��Ϊ��(�½�)����֮
        var delta = height - this.height;

        controller.height = height;
        controller.center += Vector3.up * delta * 0.5f;
    }

    /// <summary>
    /// Ͷ�佺���μ��(�����RaycastHit)
    /// </summary>
    public virtual bool CapsuleCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        return CapsuleCast(direction, distance, out _, layer, queryTriggerInteraction);
    }
    /// <summary>
    /// Ͷ�佺���μ��
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
    /// Ͷ�����μ��(�����RaycastHit)
    /// </summary>
    public virtual bool SphereCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        return SphereCast(direction, distance, out _, layer, queryTriggerInteraction);
    }
    /// <summary>
    /// Ͷ�����μ��
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
    /// ��ȡ������Entity�ص�������
    /// </summary>
    /// Tip��result�ǽ���ֵ
    public virtual int OverlapEntity(Collider[] result, float inputOffset = 0)
    {
        float contactOffset = inputOffset + controller.skinWidth + Physics.defaultContactOffset;
        float overlapsRadius = radius + contactOffset;//�뾶
        float offset = (height + contactOffset) * 0.5f - overlapsRadius;
        Vector3 top = position + Vector3.up * offset;//�ϲ�Բ��
        Vector3 bottom = position + Vector3.down * offset;//�²�Բ��
        return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
    }

    public virtual void ApplyDamage(int damage, Vector3 origin) { }
}