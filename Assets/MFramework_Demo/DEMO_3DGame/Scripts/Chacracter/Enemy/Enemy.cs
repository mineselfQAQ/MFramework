using MFramework;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(EnemyStatsManager))]
[RequireComponent(typeof(EnemyStateManager))]
[RequireComponent(typeof(EnemyAudio))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(Waypoint))]
[RequireComponent(typeof(Health))]
public class Enemy : Entity<Enemy>
{
    public EnemyEvents enemyEvents;

    protected Collider[] m_sightOverlaps = new Collider[1024];
    protected Collider[] m_contactAttackOverlaps = new Collider[1024];

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
    }

    protected override void OnUpdate()
    {
        ContactAttack();
        HandleSight();
    }

    protected void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, stats.current.spotRange);
        }
    }

    /// <summary>
    /// �յ��˺�
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
                    gameObject.SetActive(false);
                    //TODO:����LandingSmokeParticle
                }, stats.current.cleanDuration);
            }
        }
    }

    /// <summary>
    /// �������ײ(��ҽ�������)
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
            int overlapNum = Physics.OverlapSphereNonAlloc(position, stats.current.spotRange, m_sightOverlaps);

            for (int i = 0; i < overlapNum; i++)
            {
                if (m_sightOverlaps[i].CompareTag(GameTags.Player))
                {
                    if (m_sightOverlaps[i].TryGetComponent<Player>(out var player))
                    {
                        //Player���뷶Χ
                        this.player = player;
                        enemyEvents.OnPlayerSpotted?.Invoke();
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
            if ((player.health.current == 0) || (distance > stats.current.viewRange))
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
