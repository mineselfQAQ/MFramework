using MFramework;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Checkpoint : MonoBehaviour
{
    public Transform respawn;
    public AudioClip clip;
    public Transform flag;

    public UnityEvent OnActivate;

    protected Collider m_collider;
    protected AudioSource m_audio;

    public bool activated { get; protected set; }

    protected virtual void Awake()
    {
        m_audio = GetComponent<AudioSource>();

        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag(GameTags.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                Activate(player);
            }
        }
    }

    public virtual void Activate(Player player)
    {
        if (!activated)
        {
            activated = true;
            m_audio.PlayOneShot(clip);
            player.SetRespawnPos(respawn.position, respawn.rotation);//更改Player重生地
            OnActivate?.Invoke();
        }
    }

    public virtual void ShowFlag()
    {
        Vector3 from = flag.position;
        Vector3 to = from + Vector3.up * 0.5f;
        MTween.DoTween01NoRecord((f) =>
        {
           flag.position = Vector3.Lerp(from, to, f);
        }, MCurve.Linear, 0.5f);
    }
}
