using System;

namespace MFramework
{
    public class MCurve
    {
        public CurveType curveType;
        public CurveDir curveDir;

        public Func<float, float> func { get; private set; }//曲线函数

        /// <summary>
        /// 线性递增
        /// </summary>
        public static MCurve Linear { get { return new MCurve(CurveType.Linear); } }

        /// <summary>
        /// 慢进快出(正弦函数)
        /// </summary>
        public static MCurve SineIn { get { return new MCurve(CurveType.SineIn); } }
        /// <summary>
        /// 快进慢出(正弦函数)
        /// </summary>
        public static MCurve SineOut { get { return new MCurve(CurveType.SineOut); } }
        /// <summary>
        /// 慢快慢(正弦函数)
        /// </summary>
        public static MCurve SineInOut { get { return new MCurve(CurveType.SineInOut); } }

        /// <summary>
        /// 慢进快出(2次函数)
        /// </summary>
        public static MCurve QuadIn { get { return new MCurve(CurveType.QuadIn); } }
        /// <summary>
        /// 快进慢出(2次函数)
        /// </summary>
        public static MCurve QuadOut { get { return new MCurve(CurveType.QuadOut); } }
        /// <summary>
        /// 慢快慢(2次函数)
        /// </summary>
        public static MCurve QuadInOut { get { return new MCurve(CurveType.QuadInOut); } }

        /// <summary>
        /// 慢进快出(3次函数)
        /// </summary>
        public static MCurve CubicIn { get { return new MCurve(CurveType.CubicIn); } }
        /// <summary>
        /// 快进慢出(3次函数)
        /// </summary>
        public static MCurve CubicOut { get { return new MCurve(CurveType.CubicOut); } }
        /// <summary>
        /// 慢快慢(3次函数)
        /// </summary>
        public static MCurve CubicInOut { get { return new MCurve(CurveType.CubicInOut); } }

        /// <summary>
        /// 慢进快出(4次函数)
        /// </summary>
        public static MCurve QuartIn { get { return new MCurve(CurveType.QuartIn); } }
        /// <summary>
        /// 快进慢出(4次函数)
        /// </summary>
        public static MCurve QuartOut { get { return new MCurve(CurveType.QuartOut); } }
        /// <summary>
        /// 慢快慢(4次函数)
        /// </summary>
        public static MCurve QuartInOut { get { return new MCurve(CurveType.QuartInOut); } }

        /// <summary>
        /// 慢进快出(5次函数)
        /// </summary>
        public static MCurve QuintIn { get { return new MCurve(CurveType.QuintIn); } }
        /// <summary>
        /// 快进慢出(5次函数)
        /// </summary>
        public static MCurve QuintOut { get { return new MCurve(CurveType.QuintOut); } }
        /// <summary>
        /// 慢快慢(5次函数)
        /// </summary>
        public static MCurve QuintInOut { get { return new MCurve(CurveType.QuintInOut); } }

        /// <summary>
        /// 慢进快出(指数函数)
        /// </summary>
        public static MCurve ExpoIn { get { return new MCurve(CurveType.ExpoIn); } }
        /// <summary>
        /// 快进慢出(指数函数)
        /// </summary>
        public static MCurve ExpoOut { get { return new MCurve(CurveType.ExpoOut); } }
        /// <summary>
        /// 慢快慢(指数函数)
        /// </summary>
        public static MCurve ExpoInOut { get { return new MCurve(CurveType.ExpoInOut); } }

        /// <summary>
        /// 慢进快出(圆形曲线函数)
        /// </summary>
        public static MCurve CircIn { get { return new MCurve(CurveType.CircIn); } }
        /// <summary>
        /// 快进慢出(圆形曲线函数)
        /// </summary>
        public static MCurve CircOut { get { return new MCurve(CurveType.CircOut); } }
        /// <summary>
        /// 慢快慢(圆形曲线函数)
        /// </summary>
        public static MCurve CircInOut { get { return new MCurve(CurveType.CircInOut); } }

        /// <summary>
        /// 慢弹入(反方向慢入后快出)
        /// </summary>
        public static MCurve BackIn { get { return new MCurve(CurveType.BackIn); } }
        /// <summary>
        /// 快弹入(过量快入后慢出)
        /// </summary>
        public static MCurve BackOut { get { return new MCurve(CurveType.BackOut); } }
        /// <summary>
        /// 弹入弹出(|0|->|-0.1|->|1.1|->|1|，慢快慢)
        /// </summary>
        public static MCurve BackInOut { get { return new MCurve(CurveType.BackInOut); } }


        /// <summary>
        /// 震动弹出(弹性曲线)
        /// </summary>
        public static MCurve ElasticIn { get { return new MCurve(CurveType.ElasticIn); } }
        /// <summary>
        /// 震动弹入(弹性曲线)
        /// </summary>
        public static MCurve ElasticOut { get { return new MCurve(CurveType.ElasticOut); } }
        /// <summary>
        /// 震动弹入弹出(弹性曲线)
        /// </summary>
        public static MCurve ElasticInOut { get { return new MCurve(CurveType.ElasticInOut); } }

        /// <summary>
        /// 慢弹至快弹
        /// </summary>
        public static MCurve BounceIn { get { return new MCurve(CurveType.BounceIn); } }
        /// <summary>
        /// 快弹至慢弹
        /// </summary>
        public static MCurve BounceOut { get { return new MCurve(CurveType.BounceOut); } }
        /// <summary>
        /// 慢弹快弹慢弹
        /// </summary>
        public static MCurve BounceInOut { get { return new MCurve(CurveType.BounceInOut); } }

        public MCurve(CurveType curveType, CurveDir curveDir = CurveDir.Increment)//内置曲线
        {
            this.curveType = curveType;
            this.curveDir = curveDir;
        }

        public MCurve(Func<float, float> func, CurveDir curveDir = CurveDir.Increment)//自定义曲线
        {
            this.func = func;
            this.curveDir = curveDir;
        }

        public static MCurve GetMCurve(CurveType type)
        {
            switch (type)
            {
                case CurveType.Linear:
                    return MCurve.Linear;
                case CurveType.QuadIn:
                    return MCurve.QuadIn;
                case CurveType.QuadOut:
                    return MCurve.QuadOut;
                case CurveType.QuadInOut:
                    return MCurve.QuadInOut;
                case CurveType.CubicIn:
                    return MCurve.CubicIn;
                case CurveType.CubicOut:
                    return MCurve.CubicOut;
                case CurveType.CubicInOut:
                    return MCurve.CubicInOut;
                case CurveType.QuartIn:
                    return MCurve.QuartIn;
                case CurveType.QuartOut:
                    return MCurve.QuartOut;
                case CurveType.QuartInOut:
                    return MCurve.QuartInOut;
                case CurveType.QuintIn:
                    return MCurve.QuintIn;
                case CurveType.QuintOut:
                    return MCurve.QuintOut;
                case CurveType.QuintInOut:
                    return MCurve.QuintInOut;
                case CurveType.SineIn:
                    return MCurve.SineIn;
                case CurveType.SineOut:
                    return MCurve.SineOut;
                case CurveType.SineInOut:
                    return MCurve.SineInOut;
                case CurveType.ExpoIn:
                    return MCurve.ExpoIn;
                case CurveType.ExpoOut:
                    return MCurve.ExpoOut;
                case CurveType.ExpoInOut:
                    return MCurve.ExpoInOut;
                case CurveType.ElasticIn:
                    return MCurve.ElasticIn;
                case CurveType.ElasticOut:
                    return MCurve.ElasticOut;
                case CurveType.ElasticInOut:
                    return MCurve.ElasticInOut;
                case CurveType.CircIn:
                    return MCurve.CircIn;
                case CurveType.CircOut:
                    return MCurve.CircOut;
                case CurveType.CircInOut:
                    return MCurve.CircInOut;
                case CurveType.BackIn:
                    return MCurve.BackIn;
                case CurveType.BackOut:
                    return MCurve.BackOut;
                case CurveType.BackInOut:
                    return MCurve.BackInOut;
                case CurveType.BounceIn:
                    return MCurve.BounceIn;
                case CurveType.BounceOut:
                    return MCurve.BounceOut;
                case CurveType.BounceInOut:
                    return MCurve.BounceInOut;
                default:
                    return null;
            }
        }
    }

    public static class MCurveExtension
    {
        /// <summary>
        /// 反转(0起变为1起)
        /// </summary>
        /// <param playerName="curve"></param>
        /// <returns></returns>
        public static MCurve Reverse(this MCurve curve)
        {
            return new MCurve(curve.curveType, curve.curveDir == CurveDir.Increment ? CurveDir.Decrement : CurveDir.Increment);
        }
    }
}