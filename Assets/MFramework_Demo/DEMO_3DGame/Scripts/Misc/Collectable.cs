using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [Header("General Settings")]
    public bool collectOnContact = true;
    public int times = 1;
    public float ghostingDuration = 0.5f;
    public GameObject display;
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
    public float minForceToStopPhysics = 3f;
    public float collisionRadius = 0.5f;
    public float gravity = 15f;
    public float bounciness = 0.98f;
    public float maxBounceYVelocity = 10f;
    public bool randomizeInitialDirection = true;
    public Vector3 initialVelocity = new Vector3(0, 12, 0);
    public AudioClip collisionClip;

    [Space(15)]

    public PlayerEvent onCollect;



    protected Collider m_collider;
    protected AudioSource m_audio;

    protected bool m_vanished;
    protected bool m_ghosting = true;
    protected float m_elapsedLifeTime;
    protected float m_elapsedGhostingTime;
    protected Vector3 m_velocity;

    protected const int k_verticalMinRotation = 0;
    protected const int k_verticalMaxRotation = 30;
    protected const int k_horizontalMinRotation = 0;
    protected const int k_horizontalMaxRotation = 360;

    protected virtual void Awake()
    {
        //初始化获取protected变量
        InitializeAudio();
        InitializeCollider();
        InitializeTransform();
        InitializeDisplay();
        InitializeVelocity();
    }

    protected virtual void InitializeAudio()
    {
        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }
    }
    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    protected virtual void InitializeTransform()
    {
        transform.parent = null;//设为根物体  TODO:为什么？
        transform.rotation = Quaternion.identity;
    }
    protected virtual void InitializeDisplay()
    {
        display.SetActive(!hidden);
    }
    protected virtual void InitializeVelocity()
    {
        var direction = initialVelocity.normalized;//方向
        var force = initialVelocity.magnitude;//力

        //随机方向
        if (randomizeInitialDirection)
        {
            var randomZ = Random.Range(k_verticalMinRotation, k_verticalMaxRotation);
            var randomY = Random.Range(k_horizontalMinRotation, k_horizontalMaxRotation);
            direction = Quaternion.Euler(0, 0, randomZ) * direction;
            direction = Quaternion.Euler(0, randomY, 0) * direction;
        }

        m_velocity = direction * force;
    }
}
