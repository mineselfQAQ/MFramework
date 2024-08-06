using UnityEngine;

/// <summary>
/// Player的Spin操作时的拖尾特效
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
public class PlayerSpinTrail : MonoBehaviour
{
    public Transform hand;

    protected Player m_player;
    protected TrailRenderer m_trail;

    protected virtual void Start()
    {
        InitializeTrail();
        InitializeTransform();
        InitializePlayer();
    }

    protected virtual void InitializeTrail()
    {
        m_trail = GetComponent<TrailRenderer>();
        m_trail.enabled = false;
    }
    protected virtual void InitializeTransform()
    {
        transform.parent = hand;
        transform.localPosition = Vector3.zero;
    }
    protected virtual void InitializePlayer()
    {
        m_player = GetComponentInParent<Player>();
        m_player.states.events.onChange.AddListener(HandleActive);
    }

    protected virtual void HandleActive()
    {
        if (m_player.states.IsCurrentOfType(typeof(SpinPlayerState)))
        {
            m_trail.enabled = true;//开启拖尾特效
        }
        else
        {
            m_trail.enabled = false;
        }
    }
}
