using MFramework;
using UnityEngine.Audio;

public class SoundController : ComponentSingleton<SoundController>
{
    public AudioMixer mixer;

    public string MASTER = "MASTER";
    public string MUSIC = "MUSIC";
    public string SFX = "SFX";

    protected float curMusic;
    public float CurMusic 
    {
        get { return curMusic; }
        set
        {
            curMusic = value;
            mixer.SetFloat(MUSIC, MMath.LinearToDecibel(curMusic));
        }
    }
    protected float curSFX;
    public float CurSFX
    {
        get { return curSFX; }
        set
        {
            curSFX = value;
            mixer.SetFloat(SFX, MMath.LinearToDecibel(curSFX));
        }
    }

    protected string settingsPath => MSerializationManager.Instance.settingsPath;

    protected override void Awake()
    {
        base.Awake();
        
        var settings = MSerializationManager.Instance.coreSettings;
        CurMusic = settings.MusicSound;
        CurSFX = settings.SFXSound;

        MAudioSource.onSetOutput += SetOutput;
    }

    protected void OnApplicationQuit()
    {
        SaveSoundJson();
    }

    public void SaveSoundJson()
    {
        var settings = MSerializationManager.Instance.coreSettings;
        settings.MusicSound = CurMusic;
        settings.SFXSound = CurSFX;
        MSerializationUtility.SaveToJson<CoreSettings>(settingsPath, settings);
    }

    protected AudioMixerGroup SetOutput(string name)
    {
        return mixer.FindMatchingGroups(name)[0];
    }
}
