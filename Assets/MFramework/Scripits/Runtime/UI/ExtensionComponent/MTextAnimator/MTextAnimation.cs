using MFramework.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFramework
{
    public class MTextAnimation
    {
        private TMP_Text text;
        private TMP_TextInfo textInfo;
        private MTextAnimator animator;

        internal MTextData textData;
        internal int curTypeWriterFinishIndex = -1;
        internal int curTypeWriterStartIndex = -1;
        internal bool fastFinish;

        private static float WAVE_AMPLITUDE = 12.0f;
        private static float MAXTIME = 16777216;//float所表示的最大整数(1<<24)，可以认为是无限时间

        public MTextAnimation(TMP_Text text, MTextAnimator animator)
        {
            this.text = text;
            textInfo = text.textInfo;
            this.animator = animator;
        }

        #region 内部函数
        internal void UpdateTextInfo()
        {
            textInfo = text.textInfo;
            InitializeData();
        }
        private void InitializeData()
        {
            textData = new MTextData();
            textData.ConstructCharData(text);
        }

        /// <summary>
        /// 更新所有字符的TextInfo(变为初始状态)
        /// </summary>
        internal void UpdateInfo()
        {
            TMP_TextInfo info = text.textInfo;
            for (int i = 0; i < info.meshInfo[0].vertexCount / 4; i++)
            {
                MTextCharData data = textData.charData[i];
                for (int j = 0; j < 4; j++)
                {
                    info.meshInfo[0].vertices[data.index * 4 + j] = data.oVertices[j];
                    info.meshInfo[0].colors32[data.index * 4 + j] = new Color32(data.oColors32[j].r, data.oColors32[j].g, data.oColors32[j].b, (byte)(data.oColors32[j].a * ((float)data.oColors32[j].a / 255.0f)));
                }
            }
        }
        #endregion

        #region 打字机效果
        /// <summary>
        /// 特殊的渐入，只进行部分渐入
        /// </summary>
        internal void TypeWriterFadePart(int startIndex, int endIndex, float speed = 1.0f, float intervalFactor = 0.15f, params MTextEffect[] effects)
        {
            TypeWriter_FadePart(startIndex, endIndex, text, textData, 0.3f / speed, intervalFactor / speed, effects);
        }
        
        internal void TypeWriterShow(float speed = 1.0f, float intervalFactor = 0.15f)
        {
            TypeWriter_Show(text, textData, 0.3f / speed, intervalFactor / speed);
        }
        /// <summary>
        /// 渐入(默认速度为1情况下，0.3秒一个字，间隔0.3*0.15=0.045秒后执行下一个字)
        /// </summary>
        internal void TypeWriterFade(float speed = 1.0f, float intervalFactor = 0.15f, params MTextEffect[] effects)
        {
            TypeWriter_Fade(text, textData, 0.3f / speed, intervalFactor / speed, effects);
        }
        /// <summary>
        /// 渐入+缩放(默认1.5倍->1倍，用时0.3秒)
        /// </summary>
        /// <param playerName="speed"></param>
        internal void TypeWriterScale(float speed = 1.0f, float intervalFactor = 0.15f, float startScale = 1.5f)
        {
            //大致为：
            //初始1.5倍大小，逐渐缩放至1倍
            //默认速度为1情况下，0.3秒字恢复完毕
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Scale,
                mode = MTextAnimMode.OneWay,
                curve = MCurve.Linear.Reverse(),

                scale = startScale,
            };
            TypeWriterFade(speed, intervalFactor, e);
        }
        /// <summary>
        /// 渐入+平移(默认向右10单位，用时0.3秒)
        /// </summary>
        internal void TypeWriterTranslation(float speed = 1.0f, float intervalFactor = 0.15f, float deltaFactor = 1.0f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Translation,
                mode = MTextAnimMode.OneWay,
                curve = MCurve.Linear,

                translation_Delta = Vector3.right * 10 * deltaFactor
            };
            TypeWriterFade(speed, intervalFactor, e);
        }
        /// <summary>
        /// 渐入+旋转(默认顺时针旋转60°)
        /// </summary>
        internal void TypeWriterRotation(float speed = 1.0f, float intervalFactor = 0.15f, float clockwiseRotation = 60.0f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Rotation,
                mode = MTextAnimMode.OneWay,
                curve = MCurve.QuadOut.Reverse(),

                rotation_Degree = clockwiseRotation
            };
            TypeWriterFade(speed, intervalFactor, e);
        }
        /// <summary>
        /// 渐入+波浪效果(上下起伏)
        /// </summary>
        internal void TypeWriterWave(float speed = 1.0f, float intervalFactor = 0.15f, float amplitudeFactor = 1.0f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Wave,
                mode = MTextAnimMode.PingPong,//本质上是TranslationPingPong
                curve = MCurve.QuadOut,

                wave_Amplitude = Vector3.up * WAVE_AMPLITUDE * amplitudeFactor
            };
            TypeWriterFade(speed, intervalFactor, e);
        }
        /// <summary>
        /// 渐入+震动
        /// </summary>
        internal void TypeWriterShake(float speed = 1.0f, float intervalFactor = 0.15f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Shake,
            };
            TypeWriterFade(speed, intervalFactor, e);
        }

        /// <summary>
        /// 打字机效果的基准---淡入效果，如有需要，可以添加MTextEffect进行额外的动画
        /// </summary>
        private void TypeWriter_Fade(TMP_Text text, MTextData data, float timePerChar, float interval = 1, params MTextEffect[] effects)
        {
            //Tip:该函数奠定了整体的思路，为：
            //每次拿出一个字符进行渐入动画以及特效动画，间隔一定时间(timePerChar*interval)后进行下一个字符的动画
            //关键在于：
            //1.内部操作的start和end必然是同一个数字
            //2.字与字的间隔取决于这里的设置(timePerChar为一单位时间，如果interval为1，那么两字符之间会间隔一单位时间)

            text.ForceMeshUpdate(true, true);//文字Mesh强制刷新
            SetAlpha(text, 0);//更改Mesh信息中的colors32值

            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            int index = 0;
            MTextAnimatorCoroutine.Repeat(id, text, () =>
            {
                //快速结束---不再使用此处的动画，而是通过另一个函数FinishTypeWriterInternalFastly()开启更快的动画
                if (fastFinish)
                {
                    MTextAnimatorCoroutine.StopCoroutines(text, id);
                }

                //Fade动画(核心---逐字修改Alpha值)
                AnimAlpha(text, data, index, index, 0, timePerChar, MTextAnimMode.OneWay, MCurve.Linear.Reverse(), true);

                for (int i = 0; i < effects.Length; i++)
                {
                    //startIndex=endIndex，所以time也就是timePerChar
                    effects[i].startIndex = Mathf.Clamp(index, 0, data.charData.Length - 1);
                    effects[i].endIndex = Mathf.Clamp(index, 0, data.charData.Length - 1);
                    effects[i].time = timePerChar;
                }
                ApplyTextEffect(text, data, effects);

                index++;
            }, true, data.charData.Length, timePerChar * interval);
        }
        private void TypeWriter_FadePart(int startIndex, int endIndex, TMP_Text text, MTextData data, float timePerChar, float interval = 1, params MTextEffect[] effects)
        {
            text.ForceMeshUpdate(true, true);//文字Mesh强制刷新
            SetAlpha(text, 0, startIndex, endIndex);//更改Mesh信息中的colors32值

            string id = MTextAnimatorCoroutine.GetAndAddID(text);

            int index = startIndex;
            MTextAnimatorCoroutine.Repeat(id, text, () =>
            {
                //Fade动画(核心---逐字修改Alpha值)
                AnimAlpha(text, data, index, index, 0, timePerChar, MTextAnimMode.OneWay, MCurve.Linear.Reverse(), true);

                for (int i = 0; i < effects.Length; i++)
                {
                    //startIndex=endIndex，所以time也就是timePerChar
                    effects[i].startIndex = Mathf.Clamp(index, 0, data.charData.Length - 1);
                    effects[i].endIndex = Mathf.Clamp(index, 0, data.charData.Length - 1);
                    effects[i].time = timePerChar;
                }
                ApplyTextEffect(text, data, effects);
                index++;
            }, true, endIndex - startIndex + 1, timePerChar * interval);
        }
        private void TypeWriter_Show(TMP_Text text, MTextData data, float timePerChar, float interval = 1)
        {
            text.ForceMeshUpdate(true, true);//文字Mesh强制刷新
            SetAlpha(text, 0);//更改Mesh信息中的colors32值

            string id = MTextAnimatorCoroutine.GetAndAddID(text);

            int index = 0;
            MTextAnimatorCoroutine.Repeat(id, text, () =>
            {
                if (fastFinish)
                {
                    MTextAnimatorCoroutine.StopCoroutines(text, id);
                }

                //Fade动画(核心---逐字修改Alpha值)
                AnimShow(text, data, index, index, timePerChar, MTextAnimMode.OneWay, true);
                index++;
            }, true, data.charData.Length, timePerChar * interval);
        }

        /// <summary>
        /// 使用MTextEffect添加额外的效果
        /// </summary>
        private void ApplyTextEffect(TMP_Text text, MTextData data, params MTextEffect[] effects)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                MTextEffect e = effects[i];
                switch (e.type)
                {
                    case MTextEffectType.None:
                        break;
                    case MTextEffectType.Scale:
                        AnimScale(text, data, e.startIndex, e.endIndex, e.scale, e.time, e.mode, e.curve);
                        break;
                    case MTextEffectType.Translation:
                        AnimTranslation(text, data, e.startIndex, e.endIndex, e.translation_Delta, e.time, e.mode, e.curve);
                        break;
                    case MTextEffectType.Rotation:
                        AnimRotation(text, data, e.startIndex, e.endIndex, e.rotation_Degree, e.time, e.mode, e.curve, e.rotation_Center);
                        break;
                    case MTextEffectType.Color:
                        AnimColor(text, data, e.startIndex, e.endIndex, e.color, e.time, e.mode, e.curve);
                        break;
                    case MTextEffectType.Alpha:
                        AnimAlpha(text, data, e.startIndex, e.endIndex, e.alpha, e.time, e.mode, e.curve);
                        break;
                    case MTextEffectType.Wave:
                        if (e.loop)
                            AnimWaveLoop(text, data, e.startIndex, e.endIndex, e.wave_Amplitude, e.time, e.interval, e.loopIntervalFactor);
                        else
                            AnimWave(text, data, e.startIndex, e.endIndex, e.wave_Amplitude, e.time);
                        break;
                    case MTextEffectType.Shake:
                        AnimShake(text, data, e.startIndex, e.endIndex, e.shake_Amplitude, e.shake_Frequency, e.time, e.mode, e.curve);
                        break;
                    case MTextEffectType.ColorFade:
                        AnimColorFade(text, data, e.startIndex, e.endIndex, e.colorFade_Frequency, e.color, e.timePerChar, e.curve);
                        break;
                    case MTextEffectType.Twinkle:
                        AnimTwinkle(text, data, e.startIndex, e.endIndex, e.twinkle_Frequency, e.curve);
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetAlpha(TMP_Text text, float alpha)
        {
            text.SetAlpha(alpha);
        }
        private void SetAlpha(TMP_Text text, float alpha, int startIndex, int endIndex)
        {
            text.SetAlpha(alpha, startIndex, endIndex);
        }
        private void SetColor(TMP_Text text, Color32 color)
        {
            text.SetColor(color);
        }
        private void SetColor(TMP_Text text, Color32 color, int startIndex, int endIndex)
        {
            text.SetColor(color, startIndex, endIndex);
        }

        private void SetOriginalColor(TMP_Text text, MTextData data)
        {
            for (int i = 0; i < data.charData.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    text.textInfo.meshInfo[0].colors32[i * 4 + j] = data.charData[i].colors32[j] = data.charData[i].oColors32[j];
                }
            }
            text.UpdateVertexData();
        }
        #endregion

        #region 动画---完整版
        private void AnimShow(TMP_Text text, MTextData data, int start, int end, float time, MTextAnimMode mode, bool typewriter = false)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            if (mode == MTextAnimMode.OneWay)
            {
                AnimShow_OneWay(text, data, start, end, time, typewriter);
            }
        }
        private void AnimAlpha(TMP_Text text, MTextData data, int start, int end, float alpha, float time, MTextAnimMode mode, MCurve curve, bool typewriter = false)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            Color32 c = data.charData[start].colors32[0];//颜色是一致的，取第一个即可
            //实际其实调用的是AnimColor(Alpha本质上是Color.a)
            AnimColor(text, data, start, end, new Color32(c.r, c.g, c.b, (byte)(alpha * 255)), time, mode, curve, typewriter);
        }
        private void AnimColor(TMP_Text text, MTextData data, int start, int end, Color32 color, float time, MTextAnimMode mode, MCurve curve, bool typewriter = false)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            if (mode == MTextAnimMode.OneWay)//渐入
            {
                AnimColor_OneWay(text, data, start, end, color, time, curve, typewriter);
            }
            else if (mode == MTextAnimMode.PingPong)//渐入渐出
            {
                AnimColor_PingPong(text, data, start, end, color, time, curve);
            }
        }
        private void AnimScale(TMP_Text text, MTextData data, int start, int end, float scale, float time, MTextAnimMode mode, MCurve curve)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            if (mode == MTextAnimMode.OneWay)
            {
                AnimScale_OneWay(text, data, start, end, scale, time, curve);
            }
            else if (mode == MTextAnimMode.PingPong)
            {
                AnimScale_PingPong(text, data, start, end, scale, time, curve);
            }
        }
        private void AnimTranslation(TMP_Text text, MTextData data, int start, int end, Vector3 delta, float time, MTextAnimMode mode, MCurve curve)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            if (mode == MTextAnimMode.OneWay)
            {
                AnimTranslation_OneWay(text, data, start, end, delta, time, curve);
            }
            else if (mode == MTextAnimMode.PingPong)
            {
                AnimTranslation_PingPong(text, data, start, end, delta, time, curve);
            }
        }
        private void AnimRotation(TMP_Text text, MTextData data, int start, int end, float angle, float time, MTextAnimMode mode, MCurve curve, Vector3 center)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            if (mode == MTextAnimMode.OneWay)
            {
                AnimRotation_OneWay(text, data, start, end, angle, time, curve, center);
            }
            else if (mode == MTextAnimMode.PingPong)
            {
                AnimRotation_PingPong(text, data, start, end, angle, time, curve, center);
            }
        }
        private void AnimWave(TMP_Text text, MTextData data, int start, int end, Vector3 amplitude, float timePerChar)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            int n = end - start + 1;
            //Wave所做的就是对每个字符进行往返平移
            AnimTranslation_PingPong(text, data, start, end, amplitude, timePerChar * 2 * n, MCurve.QuadOut);
        }
        private void AnimWaveLoop(TMP_Text text, MTextData data, int start, int end, Vector3 amplitude, float timePerChar, float interval = 1.0f, float loopIntervalFactor = 1.0f)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            //5个字符，每个间隔1秒进行，一次2秒完成
            //第5个字符在4秒后开始，进行2秒，所以6秒结束
            float totalTime = timePerChar * interval * (end - start) + timePerChar * 2.3f;
            MTextAnimatorCoroutine.Loop(id, text, () =>
            {
                //Tip:此处不能进行[start,end]的一次操作，存在Bug(一开始无法进行去回的回部分，执行一段时间后才恢复正常)
                int index = 0;
                string id2 = MTextAnimatorCoroutine.GetAndAddID(text);
                MTextAnimatorCoroutine.Repeat(id2, text, () =>
                {
                    if (start + index > end) return;
                    AnimTranslation_PingPong(text, data, start + index, start + index, amplitude, timePerChar * 2, MCurve.QuadOut);
                    index++;
                }, true, data.charData.Length, timePerChar * interval);
            }, 0, totalTime * loopIntervalFactor);
        }
        private void AnimShake(TMP_Text text, MTextData data, int start, int end, float amplitude, float frequency, float time, MTextAnimMode mode, MCurve curve)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            AnimShake_OneWay(text, data, start, end, amplitude, frequency, time, curve);
        }
        private void AnimColorFade(TMP_Text text, MTextData data, int start, int end, float frequency, Color32 color, float time, MCurve curve)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            AnimColorFade_OneWay(text, data, start, end, frequency, color, time, curve);
        }
        private void AnimTwinkle(TMP_Text text, MTextData data, int start, int end, float frequency, MCurve curve)
        {
            if (!CheckDataAndStartEnd(data, start, end)) return;

            AnimTwinkle_OneWay(text, data, start, end, frequency, curve);
        }

        private static bool CheckDataAndStartEnd(MTextData data, int start, int end)
        {
            if (data.charData.Length == 0) return false;
            if (start < 0 || start >= data.charData.Length) return false;
            if (end < 0 || end >= data.charData.Length) return false;
            if (start > end) return false;

            return true;
        }
        #endregion

        #region 动画---单程版/往返版
        private void AnimShow_OneWay(TMP_Text text, MTextData data, int start, int end, float time, bool typewriter = false)
        {
            int n = end - start + 1;//执行动画文字个数
            float singleTime = time / n;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
            }

            int index = 0;
            //每隔singleTime后执行一个文字的动画(按顺序逐个触发)
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MCoroutineManager.Instance.RepeatNoRecord(() =>
            {
                AnimShow_Char(text, chars[index++], typewriter, data);
            }, true, n, singleTime);
        }

        private void AnimColor_OneWay(TMP_Text text, MTextData data, int start, int end, Color32 color, float time, MCurve curve, bool typewriter = false)
        {
            int n = end - start + 1;//执行动画文字个数
            float singleTime = time / n;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
            }

            int index = 0;
            //每隔singleTime后执行一个文字的动画(按顺序逐个触发)
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MCoroutineManager.Instance.RepeatNoRecord(() =>
            {
                AnimColor_Char(text, chars[index++], color, singleTime, curve, typewriter, data);
            }, true, n, singleTime);
        }
        private void AnimColor_PingPong(TMP_Text text, MTextData data, int start, int end, Color32 color, float time, MCurve curve)
        {
            //先渐入
            AnimColor_OneWay(text, data, start, end, color, time / 2, curve);
            //等待后渐出(等待0.3份单程时间，如：入1秒->停0.3秒->出1秒)
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                AnimColor_OneWay(text, data, start, end, color, time / 2, curve.Reverse());
            }, time / 2 * 1.3f);
        }

        private void AnimScale_OneWay(TMP_Text text, MTextData data, int start, int end, float scale, float time, MCurve curve)
        {
            int n = end - start + 1;
            float singleTime = time / n;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
            }

            int index = 0;
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MCoroutineManager.Instance.RepeatNoRecord(() =>
            {
                AnimScale_Char(text, chars[index++], scale, singleTime, curve);
            }, true, n, singleTime);
        }
        private void AnimScale_PingPong(TMP_Text text, MTextData data, int start, int end, float scale, float time, MCurve curve)
        {
            AnimScale_OneWay(text, data, start, end, scale, time / 2, curve);
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                AnimScale_OneWay(text, data, start, end, scale, time / 2, curve.Reverse());
            }, time / 5);
        }

        private void AnimTranslation_OneWay(TMP_Text text, MTextData data, int start, int end, Vector3 delta, float time, MCurve curve)
        {
            int n = end - start + 1;
            float singleTime = time / n;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
            }

            int index = 0;
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MCoroutineManager.Instance.RepeatNoRecord(() =>
            {
                AnimTranslation_Char(text, chars[index++], delta, singleTime, curve);
            }, true, n, singleTime);
        }
        private void AnimTranslation_PingPong(TMP_Text text, MTextData data, int start, int end, Vector3 delta, float time, MCurve curve)
        {
            AnimTranslation_OneWay(text, data, start, end, delta, time / 2, curve);
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MTextAnimatorCoroutine.Delay(id, text, () =>
            {
                AnimTranslation_OneWay(text, data, start, end, delta, time / 2, curve.Reverse());
            }, time / 2 * 1.3f);
        }

        private void AnimRotation_OneWay(TMP_Text text, MTextData data, int start, int end, float angle, float time, MCurve curve, Vector3 center)
        {
            int n = end - start + 1;
            float singleTime = time / n;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                if (i >= 0 && i < chars.Length && start + i >= 0 && start + i < data.charData.Length)
                {
                    chars[i] = data.charData[start + i];
                }
            }

            int index = 0;
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MCoroutineManager.Instance.RepeatNoRecord(() =>
            {
                AnimRotation_Char(text, chars[index++], angle, singleTime, curve, center);
            }, true, n, singleTime / 3);
        }
        private void AnimRotation_PingPong(TMP_Text text, MTextData data, int start, int end, float angle, float time, MCurve curve, Vector3 center)
        {
            AnimRotation_OneWay(text, data, start, end, angle, time / 2, curve, center);
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                AnimRotation_OneWay(text, data, start, end, angle, time / 2, curve.Reverse(), center);
            }, time / 8);
        }

        private void AnimShake_OneWay(TMP_Text text, MTextData data, int start, int end, float amplitude, float frequency, float time, MCurve curve)
        {
            int n = end - start + 1;
            float singleTime = time / n;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
            }

            int index = 0;
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MCoroutineManager.Instance.RepeatNoRecord(() =>
            {
                AnimShake_Char(text, chars[index++], amplitude, frequency, time, curve);
            }, true, n, 0);
        }

        private void AnimColorFade_OneWay(TMP_Text text, MTextData data, int start, int end, float frequency, Color32 color, float time, MCurve curve)
        {
            int n = end - start + 1;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
                string id = MTextAnimatorCoroutine.GetAndAddID(text);
                MTextAnimatorCoroutine.BeginIEnumerator(id, text, AnimColorFade_Char(text, chars[i], start, end, frequency, color, time, curve));
            }
        }

        private void AnimTwinkle_OneWay(TMP_Text text, MTextData data, int start, int end, float frequency, MCurve curve)
        {
            int n = end - start + 1;
            MTextCharData[] chars = new MTextCharData[n];
            for (int i = 0; i < n; i++)
            {
                chars[i] = data.charData[start + i];
                AnimTwinkle_Char(text, chars[i], frequency, curve);
            }
        }
        #endregion

        #region 动画---细节(单个字符)
        private void AnimShow_Char(TMP_Text text, MTextCharData data, bool typewriter = false, MTextData textData = null)
        {
            //ColorCharVertices(data, new Color32(255, 255, 255, 255));
            UpdateCharData(data, text);
            text.UpdateVertexData();

            if (typewriter)//在打字机动画情况下
            {
                curTypeWriterStartIndex = data.index;//记录进度(注意：如果除了渐变还有其他操作，这里并不能表示具体的进度，这里的index只能说明该index的alpha已经变为1但有可能还在旋转/波动等操作中)
                curTypeWriterFinishIndex = data.index;

                //提前结算(因为速度过快导致结算顺序不正确)
                int earlierIndex = (int)(2.5f * animator.typeSpeed);//根据速度决定提前量
                if (data.index == textData.charData.Length - 1 - earlierIndex)
                {
                    animator.typewriterPlaying = false;
                }
                //最后一个字符执行完毕
                if (data.index == textData.charData.Length - 1)
                {
                    animator.onTypeWriterFinished?.Invoke();//触发事件
                    curTypeWriterStartIndex = -1;
                    curTypeWriterFinishIndex = -1;

                    if (animator.inlineEffectsAutoDo)
                    {
                        MText mText = text as MText;
                        mText?.GetCurParsedText().ApplyEffects(this);
                    }
                }
            }
        }
        private void AnimColor_Char(TMP_Text text, MTextCharData data, Color32 color, float time, MCurve curve, bool typewriter = false, MTextData textData = null)
        {
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            HashSet<int> indexSet = new HashSet<int>();
            MTextAnimatorCoroutine.Tween01(id, text, (f) =>
            {
                if (fastFinish)
                {
                    //套娃---重新开启携程，携程定义好后就把原来的携程删了
                    string id2 = MTextAnimatorCoroutine.GetAndAddID(text);
                    HashSet<int> indexSet2 = new HashSet<int>();
                    MTextAnimatorCoroutine.Tween01(id2, text, (f) =>
                    {
                        //核心---更改顶点Alpha值
                        Color32 dst = data.oColors32[0];
                        Color32 src = data.colors32[0];
                        ColorCharVertices(data, Color32.Lerp(dst, src, f));
                        UpdateCharData(data, text);
                        text.UpdateVertexData();

                        if (typewriter)
                        {
                            //此处为Tween的持续执行区域，每一个index只应该开始一次，所以需要记录
                            if (!indexSet2.Contains(data.index))
                            {
                                curTypeWriterStartIndex = data.index;
                                indexSet2.Add(data.index);
                            }
                        }
                    }, curve, 0.001f, () =>
                    {
                        if (typewriter)//在打字机动画情况下
                        {
                            curTypeWriterFinishIndex = data.index;//即使没有执行完毕也说明已经进行了

                            //提前结算(因为速度过快导致结算顺序不正确)
                            int earlierIndex = (int)(2.5f * animator.typeSpeed);//根据速度决定提前量
                            if (data.index == textData.charData.Length - 1 - earlierIndex)
                            {
                                animator.typewriterPlaying = false;
                            }
                            //最后一个字符执行完毕
                            if (data.index == textData.charData.Length - 1)
                            {
                                animator.onTypeWriterFinished?.Invoke();//触发事件
                                curTypeWriterFinishIndex = -1;
                                curTypeWriterStartIndex = -1;
                                fastFinish = false;

                                if (animator.inlineEffectsAutoDo)
                                {
                                    MText mText = text as MText;
                                    mText?.GetCurParsedText().ApplyEffects(this);
                                }
                            }
                        }
                    });

                    MTextAnimatorCoroutine.StopCoroutines(text, id);
                }

                //核心---更改顶点Alpha值
                ColorCharVertices(data, Color32.Lerp(data.oColors32[0], color, f));
                UpdateCharData(data, text);
                text.UpdateVertexData();

                if (typewriter)
                {
                    if (!indexSet.Contains(data.index))
                    {
                        curTypeWriterStartIndex = data.index;
                        indexSet.Add(data.index);
                    }
                }
            }, curve, time, () =>
            {
                if (typewriter)//在打字机动画情况下
                {
                    //记录进度(注意：如果除了渐变还有其他操作，这里并不能表示具体的进度，这里的index只能说明该index的alpha已经变为1但有可能还在旋转/波动等操作中)
                    curTypeWriterFinishIndex = data.index;//即使没有执行完毕也说明已经进行了

                    //提前结算(因为速度过快导致结算顺序不正确)
                    int earlierIndex = (int)(2.5f * animator.typeSpeed);//根据速度决定提前量
                    if (data.index == textData.charData.Length - 1)
                    {
                        animator.typewriterPlaying = false;
                    }
                    //最后一个字符执行完毕
                    if (data.index == textData.charData.Length - 1)
                    {
                        animator.onTypeWriterFinished?.Invoke();//触发事件
                        curTypeWriterFinishIndex = -1;
                        curTypeWriterStartIndex = -1;
                        fastFinish = false;

                        if (animator.inlineEffectsAutoDo)
                        {
                            MText mText = text as MText;
                            mText?.GetCurParsedText().ApplyEffects(this);
                        }
                    }
                }
            });
        }
        private void AnimScale_Char(TMP_Text text, MTextCharData data, float scale, float time, MCurve curve)
        {
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MTextAnimatorCoroutine.Tween01(id, text, (f) =>
            {
                //核心---更改顶点缩放
                ScaleCharVertices(data, 1 + (scale - 1) * f);//f为1->0，所以为1.5->1
                UpdateCharData(data, text);
                text.UpdateVertexData();
            }, curve, time);
        }
        private void AnimTranslation_Char(TMP_Text text, MTextCharData data, Vector3 delta, float time, MCurve curve)
        {
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MTextAnimatorCoroutine.Tween01(id, text, (f) =>
            {
                //核心---更改顶点位移
                TranslateCharVertices(data, delta * f);
                UpdateCharData(data, text);
                text.UpdateVertexData();
            }, curve, time);
        }
        private void AnimRotation_Char(TMP_Text text, MTextCharData data, float angle, float time, MCurve curve, Vector3 center)
        {
            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MTextAnimatorCoroutine.Tween01(id, text, (f) =>
            {
                //核心---更改顶点旋转
                RotateCharVertices(data, angle * f, center);
                UpdateCharData(data, text);
                text.UpdateVertexData();
            }, curve, time);
        }
        private void AnimShake_Char(TMP_Text text, MTextCharData data, float amplitude, float frequency, float time, MCurve curve)
        {
            int count = (int)Mathf.Floor(frequency * time);//频率越快，执行次数越多
            float interval = 1 / frequency;//间隔时间

            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            //大致来说就是：
            //每间隔<interval>秒后进行一次随机位移，
            MTextAnimatorCoroutine.Repeat(id, text, () =>
            {
                Vector3 random = MUtility.RandomVector2();
                Vector3 diff = random * amplitude * 10;
                for (int i = 0; i < 4; i++)
                {
                    int index = i;
                    string id2 = MTextAnimatorCoroutine.GetAndAddID(text);
                    //在<interval>期间内会保持位移后的状态
                    MTextAnimatorCoroutine.Tween01(id2, text, (f) =>
                    { 
                        data.shake_translation = Vector3.Lerp(data.vertices[index], diff + data.oVertices[index], 0.2f) - data.oVertices[index];
                        UpdateCharVertexPos(data);

                        UpdateCharData(data, text);
                        text.UpdateVertexData();
                    }, curve, interval);
                }
            }, true, count, interval, () => 
            {
                //执行完毕后，将所有顶点恢复原位置
                for (int i = 0; i < 4; i++)
                {
                    int index = i;
                    string id3 = MTextAnimatorCoroutine.GetAndAddID(text);
                    MTextAnimatorCoroutine.Tween01(id3, text, (f) =>
                    {
                        data.shake_translation = Vector3.Lerp(data.translation, Vector3.zero, 0.2f);
                        UpdateCharVertexPos(data);

                        UpdateCharData(data, text);
                        text.UpdateVertexData();
                    }, curve, time);
                }
            });
        }
        private IEnumerator AnimColorFade_Char(TMP_Text text, MTextCharData data, int start, int end, float frequency, Color32 color, float time, MCurve curve)
        {
            //大致为：
            //xxxxx|XXXXX|xxxxx，在左侧和右侧有一些预留空间，渐变是在整个范围内进行的，
            //简单来说就是每次循环都会有一定的间隔时间
            float currentPos = start - 5;

            while (true)
            {
                currentPos += 0.01f * frequency;//不断向右移动
                if (currentPos >= end + 5)//越界
                {
                    currentPos = start - 5;//重置
                }
                //取得采样点
                float sample = MMath.SmoothStep(currentPos, currentPos + 3f, data.index);
                sample *= MMath.SmoothStep(currentPos + 6, currentPos + 3f, data.index);

                for (int i = 0; i < 4; i++)
                {
                    data.colors32[i] = Color32.Lerp(data.oColors32[i], color, sample);
                    UpdateCharData(data, text);
                    text.UpdateVertexData();
                }
                yield return null;
            }
        }
        private static void AnimTwinkle_Char(TMP_Text text, MTextCharData data, float frequency, MCurve curve)
        {
            float interval = 1 / frequency;

            string id = MTextAnimatorCoroutine.GetAndAddID(text);
            MTextAnimatorCoroutine.Loop(id, text, () =>
            {
                string id2 = MTextAnimatorCoroutine.GetAndAddID(text);
                MTextAnimatorCoroutine.Tween01(id2, text, (f) =>
                {
                    for (int i = 0; i < 4; i++)
                    {
                        data.colors32[i].a = (byte)Mathf.Lerp(data.oColors32[i].a, 0, f);
                        UpdateCharData(data, text);
                        text.UpdateVertexData();
                    }
                }, curve, interval, () => 
                {
                    string id3 = MTextAnimatorCoroutine.GetAndAddID(text);
                    MTextAnimatorCoroutine.Tween01(id3, text, (f) =>
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            data.colors32[i].a = (byte)Mathf.Lerp(data.oColors32[i].a, 0, 1 - f);
                            UpdateCharData(data, text);
                            text.UpdateVertexData();
                        }
                    }, curve, interval);
                });
            }, 0, interval * 2);
        }

        /// <summary>
        /// 更改MTextCharData中的颜色值
        /// </summary>
        private void ColorCharVertices(MTextCharData data, Color32 color)
        {
            for (int i = 0; i < 4; i++)
            {
                data.colors32[i] = color;
            }
        }
        /// <summary>
        /// 更改MTextCharData中的缩放值
        /// </summary>
        private void ScaleCharVertices(MTextCharData data, float scale)
        {
            if (data == null) return;
            for (int i = 0; i < 4; i++)
            {
                data.scale = scale;
            }
            UpdateCharVertexPos(data);
        }
        private void TranslateCharVertices(MTextCharData data, Vector3 delta)
        {
            for (int i = 0; i < 4; i++)
            {
                if (data == null) return;
                data.translation = delta;
            }
            UpdateCharVertexPos(data);
        }
        private void RotateCharVertices(MTextCharData data, float angle, Vector3 center)
        {
            if (data == null)
                return;
            data.angle_rad = angle * Mathf.Deg2Rad;
            data.rot_center = center;
            UpdateCharVertexPos(data);
        }

        /// <summary>
        /// 对顶点位置信息进行TRS矩阵
        /// </summary>
        private static void UpdateCharVertexPos(MTextCharData data)
        {
            Vector3[] curPos = new Vector3[4];

            //S---沿原点到顶点方向向外移动(也就是缩放)
            for (int i = 0; i < 4; i++)
            {
                Vector3 diff = data.oVertices[i] - data.center;
                curPos[i] = data.center + data.scale * diff;
            }
            //R
            Vector3 center = data.rot_center == -Vector3.one ? data.center : GetCenter(data, data.rot_center);
            float dx = center.x;
            float dy = center.y;
            for (int i = 0; i < 4; i++)
            {
                float x = curPos[i].x;
                float y = curPos[i].y;
                curPos[i].x = ((x - dx) * Mathf.Cos(data.angle_rad)) - ((y - dy) * Mathf.Sin(data.angle_rad)) + dx;
                curPos[i].y = dy - ((dy - y) * Mathf.Cos(data.angle_rad)) + ((x - dx) * Mathf.Sin(data.angle_rad));
            }
            //T
            for (int i = 0; i < 4; i++)
            {
                curPos[i] += data.translation;
                curPos[i] += data.shake_translation;
                data.vertices[i] = curPos[i];
            }
        }
        /// <summary>
        /// 通过MTextCharData更改实际数据
        /// </summary>
        private static void UpdateCharData(MTextCharData data, TMP_Text text)
        {
            if (data == null) return;
            TMP_TextInfo info = text.textInfo;
            //逐顶点更改vertices/colors32信息
            for (int i = 0; i < 4; i++)
            {
                //更改顶点
                info.meshInfo[0].vertices[data.index * 4 + i] = data.vertices[i];
                //更改颜色
                info.meshInfo[0].colors32[data.index * 4 + i] = new Color32(data.colors32[i].r, data.colors32[i].g, data.colors32[i].b, data.colors32[i].a);
            }
        }

        private static Vector3 GetCenter(MTextCharData data, Vector3 center)
        {
            if (center.x == 0)
            {
                if (center.y == 0)
                    return data.vertices[0];
                else
                    return data.vertices[3];
            }
            else
            {
                if (center.y == 0)
                    return data.vertices[1];
                else
                    return data.vertices[2];
            }
        }
        #endregion

        #region 内联动画函数
        public void Color(float speed, Color32 color, int startIndex, int endIndex)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Color,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                color = color,
                time = 0.03f / speed * (endIndex - startIndex + 1)
            };
            ApplyTextEffect(text, textData, e);
        }
        public void Scale(float speed, float scaleFactor, int startIndex, int endIndex)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Scale,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                scale = scaleFactor,
                time = 0.03f / speed * (endIndex - startIndex + 1)
            };
            ApplyTextEffect(text, textData, e);
        }
        public void Rotate(float speed, float degree, int startIndex, int endIndex)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Rotation,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                rotation_Degree = -degree,
                time = 0.1f / speed * (endIndex - startIndex + 1)
            };
            ApplyTextEffect(text, textData, e);
        }
        public void FadeIn(float speed, int startIndex, int endIndex)
        {
            SetAlpha(text, 0, startIndex, endIndex);//更改Mesh信息中的colors32值

            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Alpha,
                curve = MCurve.Linear.Reverse(),
                startIndex = startIndex,
                endIndex = endIndex,

                alpha = 0,
                time = 0.03f / speed * (endIndex - startIndex + 1)

            };
            ApplyTextEffect(text, textData, e);
        }
        public void FadeOut(float speed, int startIndex, int endIndex)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Alpha,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                alpha = 0,
                time = 0.03f / speed * (endIndex - startIndex + 1)
            };
            ApplyTextEffect(text, textData, e);
        }
        public void ColorFade(float speed, Color32 color, int startIndex, int endIndex)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.ColorFade,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                colorFade_Frequency = speed * 4,
                color = color
            };
            ApplyTextEffect(text, textData, e);
        }
        public void Shake(int startIndex, int endIndex, float time = -1, float amplitude = 1.0f, float frequency = 1.0f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Shake,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                shake_Amplitude = 0.4f * amplitude,
                shake_Frequency = 40.0f * frequency,
                time = ((time == -1) ? MAXTIME : time)//MAXTIME可以认为是无限时间
            };

            ApplyTextEffect(text, textData, e);
        }
        public void Slant(float speed, int startIndex, int endIndex, float degree = 15.0f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Rotation,
                curve = MCurve.BounceOut,
                startIndex = startIndex,
                endIndex = endIndex,

                mode = MTextAnimMode.OneWay,
                rotation_Degree = degree,
                rotation_Center = new Vector3(1, 1, 0),
                time = (0.5f / speed) * (endIndex - startIndex + 1)
            };
            ApplyTextEffect(text, textData, e);
        }
        public void Twinkle(float speed, int startIndex, int endIndex)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Twinkle,
                curve = MCurve.Linear,
                startIndex = startIndex,
                endIndex = endIndex,

                twinkle_Frequency = 2 * speed
            };
            ApplyTextEffect(text, textData, e);
        }
        public void Exclaim(float speed, int startIndex, int endIndex, float scaleFactor = 1.4f)
        {
            Scale(speed * 4, scaleFactor, startIndex, endIndex); 
            Shake(startIndex, endIndex);
        }
        public void ExclaimColor(float speed, Color32 color, int startIndex, int endIndex, float scaleFactor = 1.4f)
        {
            Scale(speed * 4, scaleFactor, startIndex, endIndex);
            Color(speed * 4, color, startIndex, endIndex);
            Shake(startIndex, endIndex);
        }
        public void ExclaimTimed(float speed, float time, int startIndex, int endIndex, float scaleFactor = 1.4f)
        {
            Scale(speed * 4, scaleFactor, startIndex, endIndex);
            Shake(startIndex, endIndex, time + (0.003f / speed) * (endIndex - startIndex + 1));
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                Scale(speed * 2, 1, startIndex, endIndex);//复原
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    SetOriginalColor(text, textData);
                }, (0.007f / speed) * (endIndex - startIndex + 1));
            }, time);
        }
        public void ExclaimColorTimed(float speed, float time, Color32 color, int startIndex, int endIndex, float scaleFactor = 1.4f)
        {
            Scale(speed * 4, scaleFactor, startIndex, endIndex);
            Color(speed * 4, color, startIndex, endIndex);
            Shake(startIndex, endIndex, time + (0.003f / speed) * (endIndex - startIndex + 1));
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                Scale(speed * 2, 1, startIndex, endIndex);//复原
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    SetOriginalColor(text, textData);
                }, (0.007f / speed) * (endIndex - startIndex + 1));
            }, time);
        }

        /// <summary>
        /// 上下波动(带循环)
        /// </summary>
        public void Wave(float speed, int startIndex, int endIndex, float amplitude = 1.0f, float loopIntervalFactor = 1.0f)
        {
            MTextEffect e = new MTextEffect()
            {
                type = MTextEffectType.Wave,
                mode = MTextAnimMode.PingPong,
                curve = MCurve.QuadOut,
                startIndex = startIndex,
                endIndex = endIndex,

                wave_Amplitude = Vector3.up * WAVE_AMPLITUDE * amplitude,
                time = 0.3f / speed,
                interval = 0.28f / speed,
                loopIntervalFactor = loopIntervalFactor,
                loop = true,
            };
            ApplyTextEffect(text, textData, e);
        }
        #endregion
    }
}