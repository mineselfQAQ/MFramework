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
    /// AudioSource扩展，使用包装类实现
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
        /// 更改Output设置
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
                MLog.Print($"{typeof(MAudioSource)}：{name}的AudioClip时长不足{fadeInTime + fadeOutTime}秒，无法渐入渐出", MLogType.Warning);
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
                if (fadeInTime > 0 && audioSource.time <= fadeInTime - 0.1f)//[0, fadeInTime - 0.1f]---0.1为余量，防止再次进行
                {
                    if (!trigger)
                    {
                        trigger = true;
                        float max = volume;//[0,1]
                        //渐入
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
                        //渐出
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
