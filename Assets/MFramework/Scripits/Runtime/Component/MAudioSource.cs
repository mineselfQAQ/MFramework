using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace MFramework
{
    /// <summary>
    /// AudioSource扩展，使用包装类实现
    /// </summary>
    [DisallowComponent(typeof(AudioSource))]
    public class MAudioSource : MonoBehaviour
    {
        public UnityEvent OnStart;

        public AudioClip audioClip;
        public AudioMixerGroup audioMixerGroup;

        [Header("Initial Setup")]
        public bool mute;
        public bool playOnAwake;
        public bool loop;

        [Space(10)]

        [Range(0, 256)]
        public int priority = 128;
        [Range(0, 1)]
        public float volume = 1;
        [Range(0, 3)]
        public float pitch = 1;

        /// <summary>
        /// 更改Output设置
        /// </summary>
        public static Func<string, AudioMixerGroup> OnSetOutput;

        protected AudioSource audioSource;

        private void Awake()
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
    }
}
