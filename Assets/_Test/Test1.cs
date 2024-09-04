using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SoundController.Instance.mixer.FindMatchingGroups("Music")[0];
    }
}