using System;

namespace MFramework.Tween
{
    public class MCurve
    {
        public CurveType CurveType { get; }
        public CurveDir CurveDir { get; }
        public Func<float, float> Func { get; }

        public static MCurve Linear => new MCurve(CurveType.Linear);
        public static MCurve SineIn => new MCurve(CurveType.SineIn);
        public static MCurve SineOut => new MCurve(CurveType.SineOut);
        public static MCurve SineInOut => new MCurve(CurveType.SineInOut);
        public static MCurve QuadIn => new MCurve(CurveType.QuadIn);
        public static MCurve QuadOut => new MCurve(CurveType.QuadOut);
        public static MCurve QuadInOut => new MCurve(CurveType.QuadInOut);
        public static MCurve CubicIn => new MCurve(CurveType.CubicIn);
        public static MCurve CubicOut => new MCurve(CurveType.CubicOut);
        public static MCurve CubicInOut => new MCurve(CurveType.CubicInOut);
        public static MCurve QuartIn => new MCurve(CurveType.QuartIn);
        public static MCurve QuartOut => new MCurve(CurveType.QuartOut);
        public static MCurve QuartInOut => new MCurve(CurveType.QuartInOut);
        public static MCurve QuintIn => new MCurve(CurveType.QuintIn);
        public static MCurve QuintOut => new MCurve(CurveType.QuintOut);
        public static MCurve QuintInOut => new MCurve(CurveType.QuintInOut);
        public static MCurve ExpoIn => new MCurve(CurveType.ExpoIn);
        public static MCurve ExpoOut => new MCurve(CurveType.ExpoOut);
        public static MCurve ExpoInOut => new MCurve(CurveType.ExpoInOut);
        public static MCurve CircIn => new MCurve(CurveType.CircIn);
        public static MCurve CircOut => new MCurve(CurveType.CircOut);
        public static MCurve CircInOut => new MCurve(CurveType.CircInOut);
        public static MCurve BackIn => new MCurve(CurveType.BackIn);
        public static MCurve BackOut => new MCurve(CurveType.BackOut);
        public static MCurve BackInOut => new MCurve(CurveType.BackInOut);
        public static MCurve ElasticIn => new MCurve(CurveType.ElasticIn);
        public static MCurve ElasticOut => new MCurve(CurveType.ElasticOut);
        public static MCurve ElasticInOut => new MCurve(CurveType.ElasticInOut);
        public static MCurve BounceIn => new MCurve(CurveType.BounceIn);
        public static MCurve BounceOut => new MCurve(CurveType.BounceOut);
        public static MCurve BounceInOut => new MCurve(CurveType.BounceInOut);

        public MCurve(CurveType curveType, CurveDir curveDir = CurveDir.Increment)
        {
            CurveType = curveType;
            CurveDir = curveDir;
        }

        public MCurve(Func<float, float> func, CurveDir curveDir = CurveDir.Increment)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
            CurveDir = curveDir;
        }

        public static MCurve FromType(CurveType type)
        {
            return new MCurve(type);
        }
    }

    public static class MCurveExtension
    {
        public static MCurve Reverse(this MCurve curve)
        {
            if (curve == null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            CurveDir reverseDir = curve.CurveDir == CurveDir.Increment
                ? CurveDir.Decrement
                : CurveDir.Increment;

            return curve.Func != null
                ? new MCurve(curve.Func, reverseDir)
                : new MCurve(curve.CurveType, reverseDir);
        }
    }
}
