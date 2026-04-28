using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework.Tween
{
    public static class MCurveSampler
    {
        private static readonly Dictionary<CurveType, Func<float, float>> CurveFuncs =
            new Dictionary<CurveType, Func<float, float>>
            {
                { CurveType.Linear, x => x },
                { CurveType.QuadIn, x => x * x },
                { CurveType.QuadOut, x => 1f - (1f - x) * (1f - x) },
                { CurveType.QuadInOut, x => x < 0.5f ? 2f * x * x : 2f * x * (2f - x) - 1f },
                { CurveType.CubicIn, x => x * x * x },
                { CurveType.CubicOut, x => 1f - (1f - x) * (1f - x) * (1f - x) },
                { CurveType.CubicInOut, x => x < 0.5f ? 4f * x * x * x : -4f * (1f - x) * (1f - x) * (1f - x) + 1f },
                { CurveType.QuartIn, x => x * x * x * x },
                { CurveType.QuartOut, x => 1f - (1f - x) * (1f - x) * (1f - x) * (1f - x) },
                { CurveType.QuartInOut, x => x < 0.5f ? 8f * x * x * x * x : -8f * (1f - x) * (1f - x) * (1f - x) * (1f - x) + 1f },
                { CurveType.QuintIn, x => x * x * x * x * x },
                { CurveType.QuintOut, x => 1f - (1f - x) * (1f - x) * (1f - x) * (1f - x) * (1f - x) },
                { CurveType.QuintInOut, x => x < 0.5f ? 16f * x * x * x * x * x : -16f * (1f - x) * (1f - x) * (1f - x) * (1f - x) * (1f - x) + 1f },
                { CurveType.SineIn, x => 1f - Mathf.Cos(x * Mathf.PI / 2f) },
                { CurveType.SineOut, x => Mathf.Sin(x * Mathf.PI / 2f) },
                { CurveType.SineInOut, x => -(Mathf.Cos(x * Mathf.PI) - 1f) / 2f },
                { CurveType.ExpoIn, x => x == 0f ? 0f : Mathf.Pow(2f, 10f * (x - 1f)) },
                { CurveType.ExpoOut, x => x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x) },
                { CurveType.ExpoInOut, ExpoInOut },
                { CurveType.ElasticIn, ElasticIn },
                { CurveType.ElasticOut, ElasticOut },
                { CurveType.ElasticInOut, ElasticInOut },
                { CurveType.CircIn, x => 1f - Mathf.Sqrt(1f - x * x) },
                { CurveType.CircOut, x => Mathf.Sqrt(1f - (x - 1f) * (x - 1f)) },
                { CurveType.CircInOut, x => x < 0.5f ? 0.5f * (1f - Mathf.Sqrt(1f - 4f * x * x)) : 0.5f * (Mathf.Sqrt(1f - (x * 2f - 2f) * (x * 2f - 2f)) + 1f) },
                { CurveType.BackIn, x => x * x * (2.70158f * x - 1.70158f) },
                { CurveType.BackOut, x => 1f + 2.70158f * (x - 1f) * (x - 1f) * (x - 1f) + 1.70158f * (x - 1f) * (x - 1f) },
                { CurveType.BackInOut, x => x < 0.5f ? x * x * (14.379636f * x - 5.189818f) : (x - 1f) * (x - 1f) * (14.379636f * (x - 1f) + 5.189818f) + 1f },
                { CurveType.BounceIn, x => 1f - BounceOutFunc(1f - x) },
                { CurveType.BounceOut, BounceOutFunc },
                { CurveType.BounceInOut, BounceInOut },
            };

        public static float Sample(MCurve curve, float x)
        {
            if (curve == null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            x = Mathf.Clamp01(x);
            Func<float, float> func = curve.Func ?? CurveFuncs[curve.CurveType];

            switch (curve.CurveDir)
            {
                case CurveDir.Increment:
                    return func(x);

                case CurveDir.Decrement:
                    return 1f - func(x);

                default:
                    throw new ArgumentOutOfRangeException(nameof(curve.CurveDir));
            }
        }

        public static float BounceOutFunc(float x)
        {
            if (x < 0.363636f)
            {
                return 7.5625f * x * x;
            }

            if (x < 0.72727f)
            {
                x -= 0.545454f;
                return 7.5625f * x * x + 0.75f;
            }

            if (x < 0.909091f)
            {
                x -= 0.818182f;
                return 7.5625f * x * x + 0.9375f;
            }

            x -= 0.954545f;
            return 7.5625f * x * x + 0.984375f;
        }

        private static float ExpoInOut(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;
            return x < 0.5f
                ? 0.5f * Mathf.Pow(2f, 20f * x - 10f)
                : 0.5f * (2f - Mathf.Pow(2f, -20f * x + 10f));
        }

        private static float ElasticIn(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;
            return -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((3.33f * x - 3.58f) * Mathf.PI * 2f);
        }

        private static float ElasticOut(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;
            return Mathf.Pow(2f, -10f * x) * Mathf.Sin((6.67f * x - 0.25f) * Mathf.PI) + 1f;
        }

        private static float ElasticInOut(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;
            return x < 0.5f
                ? -0.5f * Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((4.45f * x - 2.475f) * Mathf.PI * 2f)
                : Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((4.45f * x - 2.475f) * Mathf.PI * 2f) * 0.5f + 1f;
        }

        private static float BounceInOut(float x)
        {
            return x < 0.5f
                ? (1f - BounceOutFunc(1f - 2f * x)) * 0.5f
                : BounceOutFunc(2f * x - 1f) * 0.5f + 0.5f;
        }
    }

    public enum CurveType
    {
        Linear,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        SineIn,
        SineOut,
        SineInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        CircIn,
        CircOut,
        CircInOut,
        BackIn,
        BackOut,
        BackInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
    }

    public enum CurveDir
    {
        Increment,
        Decrement,
    }
}
