using System;

[Serializable]
public class SoundSettings
{
    public float SFXSound;
    public float MusicSound;

    public SoundSettings()
    {
        SFXSound = 0.5f;
        MusicSound = 0.5f;
    }
}
