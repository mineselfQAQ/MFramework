using UnityEngine;

namespace MFramework
{
    public static class MMath
    {
        public static float SmoothStep(float a, float b, float x)
        {
            float t = Mathf.Clamp((x - a) / (b - a), 0.0f, 1.0f);
            return t * t * (3.0f - 2.0f * t);
        }
    }
}
