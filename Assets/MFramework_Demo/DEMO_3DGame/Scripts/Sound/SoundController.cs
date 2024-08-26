using MFramework;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : ComponentSingleton<SoundController>
{
    public AudioMixer mixer;

    public string MASTER = "MASTER";
    public string MUSIC = "MUSIC";
    public string SFX = "SFX";

    public float curMusic { get; set; }
    public float curSFX { get; set; }

    protected string settingsPath => MSerializationManager.Instance.settingsPath;

    protected override void Awake()
    {
        base.Awake();

        var settings = MSerializationManager.Instance.coreSettings;
        curMusic = settings.MusicSound;
        curSFX = settings.SFXSound;
    }

    protected void OnApplicationQuit()
    {
        SaveSoundJson();
    }

    public void SaveSoundJson()
    {
        var settings = MSerializationManager.Instance.coreSettings;
        settings.MusicSound = curMusic;
        settings.SFXSound = curSFX;
        MSerializationUtility.SaveToJson<CoreSettings>(settingsPath, settings);
    }
}
