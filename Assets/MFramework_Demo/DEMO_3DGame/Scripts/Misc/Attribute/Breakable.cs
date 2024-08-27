using MFramework;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ¿É»÷ÆÆ¹¦ÄÜ
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Breakable : MonoBehaviour
{
    public GameObject model;
    public AudioClip clip;

    public UnityEvent OnBreak;

    protected Collider m_collider;
    protected AudioSource m_audio;
    protected Rigidbody m_rigidBody;

    public bool broken { get; protected set; }

    protected virtual void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_collider = GetComponent<Collider>();
        TryGetComponent(out m_rigidBody);
    }

    /// <summary>
    /// »÷ÆÆ
    /// </summary>
    public virtual void Break()
    {
        if (!broken)
        {
            if (m_rigidBody)
            {
                m_rigidBody.isKinematic = true;
            }

            broken = true;
            model.SetActive(false);
            m_collider.enabled = false;
            m_audio.PlayOneShot(clip);
            OnBreak?.Invoke();
        }
    }
}
