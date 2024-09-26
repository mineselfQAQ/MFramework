using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace MFramework
{
    public enum AudioSourceMode
    {
        Music,
        SFX,
        Custom
    }

    /// <summary>
    /// AudioSource��չ��ʹ�ð�װ��ʵ��
    /// </summary>
    [DisallowComponent(typeof(AudioSource))]
    public class MAudioSource : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent OnStart;

        [SerializeField]
        public AudioSourceMode mode = AudioSourceMode.SFX;

        [SerializeField]
        public AudioClip audioClip;
        [SerializeField]
        public AudioMixerGroup audioMixerGroup;

        [Header("Initial Setup")]
        [SerializeField] public bool mute;
        [SerializeField] public bool playOnAwake;
        [SerializeField] public bool loop;
        [SerializeField] public bool fadeInOut;
        [SerializeField] public float fadeInTime = 5.0f;
        [SerializeField] public float fadeOutTime = 5.0f;

        [Space(10)]

        [SerializeField][Range(0, 256)]
        public int priority = 128;
        [SerializeField][Range(0, 1)]
        public float volume = 1;
        [SerializeField][Range(0, 3)]
        public float pitch = 1;

        /// <summary>
        /// ����Output����
        /// </summary>
        public static Func<string, AudioMixerGroup> OnSetOutput;

        protected AudioSource audioSource;

        protected bool trigger;

        protected virtual void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.mute = mute;
            audioSource.playOnAwake = false;
            audioSource.loop = loop;
            audioSource.priority = priority;
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            if (fadeInOut && fadeInTime + fadeOutTime > audioClip.length) 
            {
                MLog.Print($"{typeof(MAudioSource)}��{name}��AudioClipʱ������{fadeInTime + fadeOutTime}�룬�޷����뽥��", MLogType.Warning);
                fadeInOut = false;
            }

            if (playOnAwake)
            {
                audioSource.Play();
            }

            OnStart?.Invoke();
            if (audioMixerGroup)
            {
                var group = OnSetOutput?.Invoke(audioMixerGroup.name);
                if (group) audioSource.outputAudioMixerGroup = group;
            }
        }

        protected virtual void Update()
        {
            if (fadeInOut)
            {
                if (fadeInTime > 0 && audioSource.time <= fadeInTime - 0.1f)//[0, fadeInTime - 0.1f]---0.1Ϊ��������ֹ�ٴν���
                {
                    if (!trigger)
                    {
                        trigger = true;
                        float max = volume;//[0,1]
                        //����
                        MTween.UnscaledDoTween01NoRecord((f) =>
                        {
                            audioSource.volume = f * max;
                        }, MCurve.Linear, fadeInTime, () => 
                        {
                            trigger = false;
                        });
                    }
                }
                else if (fadeOutTime > 0 && audioSource.time >= audioClip.length - fadeOutTime)//[audioClip.length - fadeOutTime, audioClip.length]
                {
                    if (!trigger)
                    {
                        trigger = true;
                        float max = volume;//[0,1]
                        //����
                        MTween.UnscaledDoTween01NoRecord((f) =>
                        {
                            audioSource.volume = 1 - (f * max);
                        }, MCurve.Linear, fadeOutTime, () =>
                        {
                            trigger = false;
                        });
                    }
                }
            }
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (audioSource)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
