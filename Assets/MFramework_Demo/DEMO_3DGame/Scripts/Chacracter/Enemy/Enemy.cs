using MFramework;
using UnityEngine;

[RequireComponent(typeof(EnemyStatsManager))]
[RequireComponent(typeof(EnemyStateManager))]
[RequireComponent(typeof(EnemyAudio))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(EnemyParticles))]
[RequireComponent(typeof(Waypoint))]
[RequireComponent(typeof(Health))]
public class Enemy : Entity<Enemy>
{
    [Header("Editor Settings")]
    public bool drawDetectGizmos = true;

    [Space(10)]

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
    }

    protected void OnDrawGizmos()
    {
        if (Application.isPlaying && drawDetectGizmos)
        {
            if (states.ContainsStateOfType(typeof(FollowEnemyState)))//具有追踪功能
            {
                if(player == null) Gizmos.color = Color.yellow;
                else Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(position, stats.current.patrolEnterRange);
            }
        }
    }

    /// <summary>
    /// 收到伤害
    /// </summary>
    public override void ApplyDamage(int amount, Vector3 origin)
    {
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
    /// 被玩家碰撞(玩家将会受伤)
    /// </summary>
    public virtual void ContactAttack()
    {
        if (stats.current.canAttackOnContact)
        {
            int overlapNum = OverlapEntity(m_contactAttackOverlaps, stats.current.contactOffset);

            for (int i = 0; i < overlapNum; i++)
            {
                if (m_contactAttackOverlaps[i].CompareTag(GameTags.Player) &&
                    m_contactAttackOverlaps[i].TryGetComponent<Player>(out var player))
                {
                    Vector3 stepping = controller.bounds.max + Vector3.down * stats.current.contactSteppingTolerance;

                    //Player与Enemy相撞(四周接触而非顶部)
                    if (!player.IsPointUnderStep(stepping))
                    {
                        if (stats.current.contactPushback)
                        {
                            //减速
                            lateralVelocity = -transform.forward * stats.current.contactPushBackForce;
                        }
                        //受伤
                        player.ApplyDamage(stats.current.contactDamage, transform.position);
                        enemyEvents.OnPlayerContact?.Invoke();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 巡逻
    /// </summary>
    protected virtual void HandleSight()
    {
        if (!player)//未发现Player状态
        {
            int overlapNum = Physics.OverlapSphereNonAlloc(position, stats.current.patrolEnterRange, m_sightOverlaps);

            for (int i = 0; i < overlapNum; i++)
            {
                if (m_sightOverlaps[i].CompareTag(GameTags.Player))
                {
                    if (m_sightOverlaps[i].TryGetComponent<Player>(out var player))
                    {
                        //Player进入范围
                        this.player = player;
                        enemyEvents.OnPlayerSpotted?.Invoke();
                        return;
                    }
                }
            }
        }
        else//发现Player状态
        {
            float distance = Vector3.Distance(position, player.position);

            //脱离追踪状态要求：
            //Player死亡 或 玩家距离过远(超过viewRange)
            if ((player.health.current == 0) || (distance > stats.current.patrolExitRange))
            {
                player = null;
                enemyEvents.OnPlayerScaped?.Invoke();
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
}
