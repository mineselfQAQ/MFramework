using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 文字特效所需的所有信息(无论是否用到)
    /// </summary>
    public class MTextEffect
    {
        public MTextAnimMode mode = MTextAnimMode.OneWay;
        public MTextEffectType type = MTextEffectType.None;
        public MCurve curve = MCurve.Linear;
        public int startIndex = 0, endIndex = int.MaxValue;

        public float time = 1.0f;
        public float timePerChar = 1.0f;
        public float interval = 1.0f;
        public float loopIntervalFactor = 1.0f;
        public bool loop = false;

        public Vector3 translation_Delta = Vector3.zero;
        public Vector3 rotation_Center = -Vector3.one;
        public float rotation_Degree = 45;
        public float scale = 1.5f;
        public Color32 color;
        public float alpha = 0.0f;

        public Vector3 wave_Amplitude = Vector3.up;
        public float twinkle_Frequency = 0.5f;
        public float colorFade_Frequency = 0.5f;
        public float shake_Amplitude = 0.5f;
        public float shake_Frequency = 50f;
    }

    public enum MTextEffectType
    {
        None,
        Scale,
        Translation,
        Rotation,
        Color,
        Alpha,
        Shake,
        Wave,
        Twinkle,
        ColorFade
    }

    public enum MTextAnimMode
    {
        OneWay,
        PingPong
    }
}