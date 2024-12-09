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

    protected string settingsPath;
    protected SoundSettings settings;

    protected override void Awake()
    {
        base.Awake();
        
        settingsPath = $"{MSettings.PersistentDataPath}/SoundSettings.json";
        settings = MSerializationManager.Instance.CreateOrGetSettings<SoundSettings>(settingsPath);
        CurMusic = settings.MusicSound;
        CurSFX = settings.SFXSound;

        //开启AB时需要重新设置Output中的AudioMixerGroup
        if (ABController.Instance.enableAB)
        {
            MAudioSource.OnSetOutput += SetOutput;
        }
    }

    protected void OnApplicationQuit()
    {
        SaveSoundJson();
    }

    public void SaveSoundJson()
    {
        settings.MusicSound = CurMusic;
        settings.SFXSound = CurSFX;
        MSerializationUtility.SaveToJson(settingsPath, settings);
    }

    protected AudioMixerGroup SetOutput(string name)
    {
        return mixer.FindMatchingGroups(name)[0];
    }
}
