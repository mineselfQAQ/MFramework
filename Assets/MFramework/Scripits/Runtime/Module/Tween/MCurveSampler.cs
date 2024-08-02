using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public static class MCurveSampler
    {
        public static readonly Dictionary<CurveType, Func<float, float>> curveFuncs = new Dictionary<CurveType, Func<float, float>>()
        {
            //=====Linear=====
            {
                CurveType.Linear,
                (x)=>{ return x; } 
            },
            //=====QuadIn=====
            {
                CurveType.QuadIn,
                (x)=>{ return x*x; }
            },
            //=====QuadOut=====
            {
                CurveType.QuadOut, 
                (x)=>{ return 1 - (1-x)*(1-x); } 
            },
            //=====QuadInOut=====
            {
                CurveType.QuadInOut,
                (x)=>{ return x<0.5f ? 2*x*x : 2*x*(2-x)-1; }
            },
            //=====CubicIn=====
            {
                CurveType.CubicIn,
                (x)=>{ return x*x*x; }
            },
            //=====CubicOut=====
            {
                CurveType.CubicOut,
                (x)=>{ return 1 - (1-x)*(1-x)*(1-x); } 
            },
            //=====CubicInOut=====
            {
                CurveType.CubicInOut,
                (x)=>{ return x<0.5f ? 4*x*x*x : -4*(1-x)*(1-x)*(1-x)+1; }
            },
            //=====QuartIn=====
            {
                CurveType.QuartIn,
                (x)=>{ return x*x*x*x; }
            },
            //=====QuartOut=====
            {
                CurveType.QuartOut,
                (x)=>{ return 1 - (1-x)*(1-x)*(1-x)*(1-x); } 
            },
            //=====QuartInOut=====
            {
                CurveType.QuartInOut,
                (x)=>{ return x<0.5f ? 8*x*x*x*x : -8*(1-x)*(1-x)*(1-x)*(1-x)+1; }
            },
            //=====QuintIn=====
            {
                CurveType.QuintIn,
                (x)=>{ return x*x*x*x*x; }
            },
            //=====QuintOut=====
            {
                CurveType.QuintOut,
                (x)=>{ return 1 - (1-x)*(1-x)*(1-x)*(1-x)*(1-x); }
            },
            //=====QuintInOut=====
            {
                CurveType.QuintInOut, 
                (x)=>{ return x<0.5f ? 16*x*x*x*x*x : -16*(1-x)*(1-x)*(1-x)*(1-x)*(1-x)+1; }
            },
            //=====SineIn=====
            {
                CurveType.SineIn, 
                (x)=>{ return 1.0f - Mathf.Cos((x * Mathf.PI) / 2); }
            },
            //=====SineOut=====
            {
                CurveType.SineOut,
                (x)=>{ return Mathf.Sin((x * Mathf.PI) / 2); }
            },
            //=====SineInOut=====
            { 
                CurveType.SineInOut,
                (x)=>{ return -(Mathf.Cos(x * Mathf.PI) - 1) / 2; }
            },
            //=====ExpoIn=====
            {
                CurveType.ExpoIn,
                (x)=>{ return x==0 ? 0 : Mathf.Pow(2.0f, 10.0f * (x - 1.0f)); }
            },
            //=====ExpoOut=====
            {
                CurveType.ExpoOut,
                (x)=>{ return x==1 ? 1 : 1 - Mathf.Pow(2.0f, -10.0f * x); }
            },
            //=====ExpoInOut=====
            {
                CurveType.ExpoInOut, 
                (x)=>{ return 
                    x==0 ? 0 : 
                    x==1 ? 1 :
                    x<0.5f ? 0.5f * Mathf.Pow(2.0f, 20.0f * x - 10.0f) : 
                        0.5f * (2.0f - Mathf.Pow( 2.0f,-20.0f * x + 10.0f)) ; }
            },
            //=====ElasticIn=====
            {
                CurveType.ElasticIn,
                (x)=>{ return 
                    x==0 ? 0 : 
                    x==1 ? 1 :
                        -Mathf.Pow(2.0f, 10.0f * x - 10.0f) * Mathf.Sin((3.33f * x - 3.58f) * Mathf.PI * 2); }
            },
            //=====ElasticOut=====
            {
                CurveType.ElasticOut,
                (x)=>{ return
                    x==0 ? 0 :
                    x==1 ? 1 :
                        Mathf.Pow(2.0f, -10.0f * x) * Mathf.Sin((6.67f * x - 0.25f) * Mathf.PI) + 1; }
            },
            //=====ElasticInOut=====
            { 
                CurveType.ElasticInOut,
                (x)=>{ return 
                    x==0 ? 0 :
                    x==1 ? 1 : 
                    x<0.5f ? -0.5f * Mathf.Pow(2.0f, 20.0f * x - 10.0f) * Mathf.Sin((4.45f * x - 2.475f) * Mathf.PI * 2) :
                        Mathf.Pow(2.0f, -20.0f * x + 10.0f) * Mathf.Sin((4.45f * x - 2.475f) * Mathf.PI * 2) * 0.5f + 1.0f; }
            },
            //=====CircIn=====
            {
                CurveType.CircIn, 
                (x)=>{ return 1.0f - Mathf.Sqrt(1.0f - x * x); }
            },
            //=====CircOut=====
            {
                CurveType.CircOut,
                (x)=>{ return Mathf.Sqrt(1.0f - (x-1)*(x-1)); }
            },
            //=====CircInOut=====
            {
                CurveType.CircInOut, 
                (x)=>{ return x<0.5 ? 0.5f * (1.0f - Mathf.Sqrt(1.0f - 4.0f * x * x)) :
                    0.5f * (Mathf.Sqrt(1.0f - (x*2-2) * (x*2-2)) + 1 ); }
            },
            //=====BlackIn=====
            {
                CurveType.BackIn, 
                (x)=>{ return x * x * (2.70158f * x - 1.70158f); }
            },
            //=====BackOut=====
            {
                CurveType.BackOut,
                (x)=>{ return 1 + 2.70158f * (x-1)*(x-1)*(x-1) + 1.70158f * (x-1)*(x-1); }
            },
            //=====BackInOut=====
            {
                CurveType.BackInOut,
                (x)=>{ return x<0.5 ? x * x * (14.379636f * x - 5.189818f) : 
                    ((x-1)*(x-1)  * (14.379636f * (x-1) + 5.189818f) + 1.0f); }
            },
            //=====BounceIn=====
            {
                CurveType.BounceIn,
                (x)=>{ return 1 - BounceOutFunc(1 - x); }
            },
            //=====BounceOut=====
            {
                CurveType.BounceOut,
                (x)=>{ return BounceOutFunc(x); }
            },
            //=====BounceInOut=====
            {
                CurveType.BounceInOut, 
                (x)=>
                {
                    if (x < 0.5f)
                    {
                        if (x > 0.318182f)
                        {
                            x = 1.0f - x * 2.0f;
                            return  (0.5f - 3.78125f * x * x);
                        }
                        else if (x > 0.136365f)
                        {
                            x = 0.454546f - x * 2.0f;
                            return  (0.125f - 3.78125f * x * x);
                        }
                        else if (x > 0.045455f)
                        {
                            x = 0.181818f - x * 2.0f;
                            return  (0.03125f - 3.78125f * x * x);
                        }
                        else
                        {
                            x = 0.045455f - x * 2.0f;
                            return  (0.007813f - 3.78125f * x * x);
                        }
                    }

                    if (x < 0.681818f)
                    {
                        x = x * 2.0f - 1.0f;
                        return  (3.78125f * x * x + 0.5f);
                    }
                    else if (x < 0.863635f)
                    {
                        x = x * 2.0f - 1.545454f;
                        return  (3.78125f * x * x + 0.875f);
                    }
                    else if (x < 0.954546f)
                    {
                        x = x * 2.0f - 1.818182f;
                        return  (3.78125f * x * x + 0.96875f);
                    }
                    else
                    {
                        x = x * 2.0f - 1.954545f;
                        return  (3.78125f * x * x + 0.992188f);
                    }
                }
            },
        };

        public static float BounceOutFunc(float x)
        {
            if (x < 0.363636f)
            {
                return 7.5625f * x * x;
            }
            else if (x < 0.72727f)
            {
                x -= 0.545454f;
                return (7.5625f * x * x + 0.75f);
            }
            else if (x < 0.909091f)
            {
                x -= 0.818182f;
                return (7.5625f * x * x + 0.9375f);
            }
            else
            {
                x -= 0.954545f;
                return (7.5625f * x * x + 0.984375f);
            }
        }

        public static float Sample(MCurve curve, float x)
        {
            x = Mathf.Clamp01(x);

            Func<float, float> func;
            if (curve.func != null)//自定义函数
            {
                func = curve.func;
            }
            else//内置函数
            {
                CurveType type = curve.curveType;
                func = curveFuncs[type];//取出Directory中的Func<>
            }

            switch (curve.curveDir)
            {
                //使用Func<>
                case CurveDir.Increment:
                    return func(x);

                case CurveDir.Decrement:
                    return 1 - func(x);

                default:
                    return -1;
            }
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
        BounceInOut
    }

    public enum CurveDir
    {
        Increment,
        Decrement,
    }
}