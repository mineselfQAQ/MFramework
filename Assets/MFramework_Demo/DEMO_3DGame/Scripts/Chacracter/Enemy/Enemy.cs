using MFramework;
using UnityEngine;

[RequireComponent(typeof(EnemyStatsManager))]
[RequireComponent(typeof(EnemyStateManager))]
[RequireComponent(typeof(EnemyAudio))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(EnemyParticles))]
[RequireComponent(typeof(Health))]
public class Enemy : Entity<Enemy>
{
    [Header("Editor Settings")]
    public bool drawDetectGizmos = true;

    [Space(10)]

    public bool friendly;
    public EnemyEvents enemyEvents;

    protected Collider[] m_sightOverlaps = new Collider[1024];
    protected Collider[] m_contactAttackOverlaps = new Collider[1024];

    protected GameObject m_skin;

    public EnemyStatsManager stats { get; protected set; }
    public Waypoint waypoints { get; protected set; }
    public Health health { get; protected set; }

    public Player player { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        tag = GameTags.Enemy;

        stats = GetComponent<EnemyStatsManager>();
        waypoints = GetComponent<Waypoint>();
        health = GetComponent<Health>();

        m_skin = transform.Find("Skin").gameObject;
    }

    protected override void OnUpdate()
    {
        ContactAttack();
        HandleSight();

        if (player)
        {
            enemyEvents.OnPlayerStay?.Invoke();
        }
    }

    protected void OnDrawGizmos()
    {
        if (Application.isPlaying && drawDetectGizmos)
        {
            if (states.ContainsStateOfType(typeof(FollowEnemyState)) || //׷��ģʽ
                states.ContainsStateOfType(typeof(IdleLookEnemyState))) //ע��ģʽ(NPC)
            {
                if (!player)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(position, stats.current.patrolEnterRange);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(position, stats.current.patrolExitRange);
                }
            }
        }
        //TODO:δ��ʼ��Ϸʱ����δ��ʼ���޷����л���
    }

    /// <summary>
    /// �յ��˺�
    /// </summary>
    public override void ApplyDamage(int amount, Vector3 origin)
    {
        if (friendly) return;

        if (!health.isEmpty && !health.recovering)
        {
            health.Damage(amount);
            enemyEvents.OnDamage?.Invoke();

            if (health.isEmpty)
            {
                controller.enabled = false;
                enemyEvents.OnDie?.Invoke();

                MCoroutineManager.Instance.DelayWithTimeScaleNoRecord(() =>
                {
                    m_skin.SetActive(false);
                    enemyEvents.OnDead?.Invoke();
                }, stats.current.cleanDuration);
            }
        }
    }

    /// <summary>
    /// �������ײ(��ҽ�������)
    /// </summary>
    public virtual void ContactAttack()
    {
        if (friendly) return;

        if (stats.current.canAttackOnContact)
        {
            int overlapNum = OverlapEntity(m_contactAttackOverlaps, stats.current.contactOffset);

            for (int i = 0; i < overlapNum; i++)
            {
                if (m_contactAttackOverlaps[i].CompareTag(GameTags.Player) &&
                    m_contactAttackOverlaps[i].TryGetComponent<Player>(out var player))
                {
                    Vector3 stepping = controller.bounds.max + Vector3.down * stats.current.contactSteppingTolerance;

                    //Player��Enemy��ײ(���ܽӴ����Ƕ���)
                    if (!player.IsPointUnderStep(stepping))
                    {
                        if (stats.current.contactPushback)
                        {
                            //����
                            lateralVelocity = -transform.forward * stats.current.contactPushBackForce;
                        }
                        //����
                        player.ApplyDamage(stats.current.contactDamage, transform.position);
                        enemyEvents.OnPlayerContact?.Invoke();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Ѳ��
    /// </summary>
    protected virtual void HandleSight()
    {
        if (!player)//δ����Player״̬
        {
            int overlapNum = Physics.OverlapSphereNonAlloc(position, stats.current.patrolEnterRange, m_sightOverlaps);

            for (int i = 0; i < overlapNum; i++)
            {
                if (m_sightOverlaps[i].CompareTag(GameTags.Player))
                {
                    if (m_sightOverlaps[i].TryGetComponent<Player>(out var player))
                    {
                        //Player���뷶Χ
                        this.player = player;
                        enemyEvents.OnPlayerEnter?.Invoke();
                        return;
                    }
                }
            }
        }
        else//����Player״̬
        {
            float distance = Vector3.Distance(position, player.position);

            //����׷��״̬Ҫ��
            //Player���� �� ��Ҿ����Զ(����viewRange)
            if ((player.health.current == 0) || (distance > stats.current.patrolExitRange))
            {
                player = null;
                enemyEvents.OnPlayerExit?.Invoke();
            }
        }
    }

    public virtual void Accelerate(Vector3 direction, float acceleration, float topSpeed)
    {
        Accelerate(direction, stats.current.turningDrag, acceleration, topSpeed);
    }

    public virtual void FaceDirectionSmooth(Vector3 direction)
    {
        FaceDirection(direction, stats.current.rotationSpeed);
    } 

    public virtual void Decelerate()
    {
        Decelerate(stats.current.deceleration);
    } 

    public virtual void Friction()
    {
        Decelerate(stats.current.friction);
    } 

    public virtual void Gravity()
    {
        Gravity(stats.current.gravity);
    }

    public virtual void SnapToGround()
    {
        SnapToGround(stats.current.snapForce);
    }

    /// <summary>
    /// ����Player
    /// </summary>
    public virtual void LookPlayer(bool smooth = true)
    {
        Vector3 direction = player.transform.position - transform.position;

        Vector3 currentForward = transform.forward;
        currentForward.y = 0;
        currentForward.Normalize();
        Vector3 targetForward = direction;
        targetForward.y = 0;
        targetForward.Normalize();

        //����ˮƽ��ת��
        float horizontalAngle = Vector3.Angle(currentForward, targetForward);

        //���㴹ֱ��ת��
        float verticalAngle = Vector3.SignedAngle(currentForward, direction.normalized, transform.right);
        verticalAngle = Mathf.Clamp(verticalAngle, -stats.current.maxVerticalAngle, stats.current.maxVerticalAngle);

        Quaternion horizontalRotation = Quaternion.LookRotation(targetForward);
        Quaternion verticalRotation = Quaternion.AngleAxis(verticalAngle, transform.right);
        Quaternion targetRotation = verticalRotation * horizontalRotation;

        if (smooth)
        {
            float rotationSpeed = stats.current.smoothRotateSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}