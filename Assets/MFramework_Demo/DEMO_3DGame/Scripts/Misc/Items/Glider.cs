using MFramework;
using UnityEngine;

[RequireComponent(typeof(MAudioSource))]
public class Glider : MonoBehaviour
{
    public Player player;
    public TrailRenderer[] trails;
    public float scaleDuration = 0.7f;

    [Header("Audio Settings")]
    public AudioClip openAudio;
    public AudioClip closeAudio;

    protected AudioSource m_audio;

    protected virtual void Start()
    {
        InitializePlayer();
        InitializeAudio();
        InitializeGlider();
        InitializeCallbacks();
    }

    protected virtual void InitializePlayer()
    {
        if (!player) player = GetComponentInParent<Player>();
    }
    protected virtual void InitializeAudio()
    {
        if (!TryGetComponent(out m_audio)) m_audio = gameObject.AddComponent<AudioSource>();
    }
    protected virtual void InitializeGlider()
    {
        SetTrailsEmitting(false);
        transform.localScale = Vector3.zero;
    }
    protected virtual void InitializeCallbacks()
    {
        player.playerEvents.OnGlidingStart.AddListener(ShowGlider);
        player.playerEvents.OnGlidingStop.AddListener(HideGlider);
    }

    protected virtual void ShowGlider()
    {
        //缩放0->1
        MTween.DoTween01NoRecord((f) =>
        {
            Vector3 scale = Vector3.Lerp(Vector3.zero, Vector3.one, f);
            transform.transform.localScale = scale;
        }, MCurve.Linear, scaleDuration);

        SetTrailsEmitting(true);
        m_audio.PlayOneShot(openAudio);
    }

    protected virtual void HideGlider()
    {
        //缩放1->0
        MTween.DoTween01NoRecord((f) =>
        {
            Vector3 scale = Vector3.Lerp(Vector3.one, Vector3.zero, f);
            transform.transform.localScale = scale;
        }, MCurve.Linear, scaleDuration);

        SetTrailsEmitting(false);
        m_audio.PlayOneShot(closeAudio);
    }
    
    /// <summary>
    /// 设置拖尾是否"开启"
    /// </summary>
    protected virtual void SetTrailsEmitting(bool value)
    {
        if (trails == null) return;

        foreach (var trail in trails)
        {
            trail.emitting = value;
        }
    }
}