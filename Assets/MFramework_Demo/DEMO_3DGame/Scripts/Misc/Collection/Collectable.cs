using MFramework;
using System.Collections;
using UnityEngine;

/// <summary>
/// 可获取功能
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MAudioSource))]
public abstract class Collectable : MonoBehaviour
{
    [Header("General Settings")]
    public int times = 1;//触发次数
    public float ghostingDuration = 0.5f;//进场后的"不可拾取时间"
    public GameObject model;
    public AudioClip clip;
    public ParticleSystem particle;

    [Header("Visibility Settings")]
    public bool hidden;
    public float quickShowHeight = 2f;
    public float quickShowDuration = 0.25f;
    public float hideDuration = 0.5f;

    [Header("Life Time")]
    public bool hasLifeTime;
    public float lifeTimeDuration = 5f;

    [Header("Physics Settings")]
    public bool usePhysics;
    public float collisionRadius = 0.5f;
    public float gravity = 30f;
    public float bounciness = 0.98f;
    public float maxBounceYVelocity = 10f;
    public bool randomizeInitialDirection = true;
    public Vector2Int randomVerticalMinMax = new Vector2Int(0, 30);
    public Vector3 initialVelocity = new Vector3(0, 12, 0);
    public AudioClip collisionClip;

    [Space(15)]
    public PlayerEvent onCollect;


    /// <summary>
    /// 是否可以通过触碰自动拾取
    /// </summary>
    internal bool autoCollect = true;

    protected Collider m_collider;
    protected AudioSource m_audio;

    protected bool m_vanished;
    protected bool m_ghosting = true;
    protected float m_elapsedLifeTime;
    protected float m_elapsedGhostingTime;
    protected Vector3 m_velocity;

    //protected const int VERTICALMINROTATION = 0;
    //protected const int VERTICALMAXROTATION = 30;
    protected const int HORIZONTALMINROTATION = 0;
    protected const int HORIZONTALMAXROTATION = 360;

    protected virtual void Awake()
    {
        InitializeAudio();
        InitializeCollider();
        InitializeTransform();
        InitializeDisplay();
        InitializeVelocity();
    }

    protected virtual void Update()
    {
        if (!m_vanished)
        {
            HandleGhosting();
            HandleLifeTime();

            if (usePhysics)
            {
                HandleMovement();
                HandleSweep();
            }
        }
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        if (autoCollect && other.CompareTag(GameTags.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                Collect(player);
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (usePhysics)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collisionRadius);
        }
    }

    protected virtual void InitializeAudio()
    {
        m_audio = GetComponent<AudioSource>();
    }
    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    protected virtual void InitializeTransform()
    {
        transform.rotation = Quaternion.identity;
    }
    protected virtual void InitializeDisplay()
    {
        model.SetActive(!hidden);
    }
    protected virtual void InitializeVelocity()
    {
        Vector3 direction = initialVelocity.normalized;//方向
        float force = initialVelocity.magnitude;//力
        
        //开启随机方向
        if (randomizeInitialDirection)
        {
            int randomZ = Random.Range(randomVerticalMinMax.x, randomVerticalMinMax.y);
            int randomY = Random.Range(HORIZONTALMINROTATION, HORIZONTALMAXROTATION);
            direction = Quaternion.Euler(0, 0, randomZ) * direction;
            direction = Quaternion.Euler(0, randomY, 0) * direction;
        }

        m_velocity = direction * force;//最终初始速度
    }

    /// <summary>
    /// 处理幽灵时间---出现后幽灵时间内不可拾取
    /// </summary>
    protected virtual void HandleGhosting()
    {
        if (m_ghosting)
        {
            m_elapsedGhostingTime += Time.deltaTime;

            if (m_elapsedGhostingTime >= ghostingDuration)
            {
                m_elapsedGhostingTime = 0;
                m_ghosting = false;
            }
        }
    }

    /// <summary>
    /// 处理生命周期---经过一定时间后自动消失
    /// </summary>
    protected virtual void HandleLifeTime()
    {
        if (hasLifeTime)
        {
            m_elapsedLifeTime += Time.deltaTime;

            if (m_elapsedLifeTime >= lifeTimeDuration)
            {
                Vanish();
            }
        }
    }

    /// <summary>
    /// 处理移动---上下向速度随时间变慢
    /// </summary>
    protected virtual void HandleMovement()
    {
        m_velocity.y -= gravity * Time.deltaTime;
    }
    /// <summary>
    /// 沿目标方向"弹跳"
    /// </summary>
    protected virtual void HandleSweep()
    {
        var direction = m_velocity.normalized;
        var magnitude = m_velocity.magnitude;
        var distance = magnitude * Time.deltaTime;

        //物体碰撞到障碍物，进行反弹
        if (Physics.SphereCast(transform.position, collisionRadius, direction,
            out var hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag(GameTags.Player))
            {
                var bounceDirection = Vector3.Reflect(direction, hit.normal);
                m_velocity = bounceDirection * magnitude * bounciness;
                m_velocity.y = Mathf.Min(m_velocity.y, maxBounceYVelocity);
                m_audio.Stop();
                m_audio.PlayOneShot(collisionClip);
            }
        }

        //如果未发生碰撞，沿初始方向运动
        //如果发生碰撞，沿反弹方向运动
        transform.position += m_velocity * Time.deltaTime;
    }

    public virtual void Collect(Player player)
    {
        if (!m_vanished && !m_ghosting)
        {
            if (!hidden)//一般物体
            {
                Vanish();

                if (particle != null)
                {
                    particle.Play();
                }
            }
            else//隐藏物体(指的是Secret)
            {
                StartCoroutine(QuickShowRoutine());
            }

            StartCoroutine(CollectRoutine(player));
        }
    }
    protected virtual IEnumerator CollectRoutine(Player player)
    {
        for (int i = 0; i < times; i++)
        {
            m_audio.Stop();
            m_audio.PlayOneShot(clip);
            onCollect?.Invoke(player);//执行拾取回调
            OnCollectInternal(player);//执行拾取回调
            yield return new WaitForSeconds(0.1f);
        }
    }
    /// <summary>
    /// 隐藏物体被拾取显示动画
    /// </summary>
    protected virtual IEnumerator QuickShowRoutine()
    {
        var initialPosition = transform.position;
        var targetPosition = initialPosition + Vector3.up * quickShowHeight;

        //显示隐藏物体
        model.SetActive(true);
        m_collider.enabled = false;

        //initialPosition->targetPosition(向上移动)
        MTween.FixedDoTween01NoRecord((f) =>
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, f);
        }, MCurve.Linear, quickShowDuration);

        //等待<hideDuration>秒
        yield return new WaitForSeconds(hideDuration);
        //复原
        transform.position = initialPosition;
        Vanish();
    }

    public virtual void Vanish()
    {
        if (!m_vanished)
        {
            m_vanished = true;
            m_elapsedLifeTime = 0;
            model.SetActive(false);
            m_collider.enabled = false;
        }
    }

    protected abstract void OnCollectInternal(Player player);
}