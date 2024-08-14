using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Volume : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public AudioClip enterClip;
    public AudioClip exitClip;

    protected AudioSource m_audio;
    protected Collider m_collider;

    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    protected virtual void InitializeAudioSource()
    {
        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }

        m_audio.spatialBlend = 0.5f;
    }

    protected virtual void Start()
    {
        InitializeCollider();
        InitializeAudioSource();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!m_collider.bounds.Contains(other.bounds.max) ||
            !m_collider.bounds.Contains(other.bounds.min))
        {
            m_audio.PlayOneShot(enterClip);
            onEnter?.Invoke();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!m_collider.bounds.Contains(other.transform.position))
        {
            m_audio.PlayOneShot(exitClip);
            onExit?.Invoke();
        }
    }
}
