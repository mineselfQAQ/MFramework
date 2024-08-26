using UnityEngine;

namespace MFramework
{
    public static class MMath
    {
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        public static float SmoothStep(float a, float b, float x)
        {
            float t = Mathf.Clamp((x - a) / (b - a), 0.0f, 1.0f);
            return t * t * (3.0f - 2.0f * t);
        }

        /// <summary>
        /// ·Ö±´Ó³Éä
        /// </summary>
        public static float LinearToDecibel(float x)
        {
            if (x == 0) return -80f; // ¾²ÒôµÄdBÖµ
            return 20f * Mathf.Log10(x);
        }
    }
}
