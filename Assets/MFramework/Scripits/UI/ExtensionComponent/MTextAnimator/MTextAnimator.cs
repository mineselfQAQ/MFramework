using MFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MFramework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MText))]
    public class MTextAnimator : MonoBehaviour
    {
        [Header("Typewriter")]
        public TypeWriterSwitch typeWriterSwitch = TypeWriterSwitch.Off;
        public MTextTypewriterType typewriterType;
        public bool typewriterStartDo = false;
        public float typeSpeed = 1.0f;
        public UnityEvent onTypeWriterFinished;

        [Header("Inline Effects")]
        public InlineEffectSwitch inlineEffectsSwitch = InlineEffectSwitch.Off;
        public bool inlineEffectsAutoDo = false;

        public bool typewriterPlaying { get; internal set; } = false;

        internal MTextAnimation mAnim;
        private TMP_Text text;

        internal void Init()
        {
            TMP_Text t = GetComponent<TMP_Text>();

            mAnim = new MTextAnimation(t, this);
            text = t;
        }

        internal void PlayInit(ParsedText pText = null)
        {
            //注意：必须延迟进行，否则textData无法正常收集
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                mAnim.UpdateTextInfo();//mAnim收集信息(textData)

                if (typewriterStartDo)
                {
                    PlayTypeWriterInternal();
                }

                //不开启打字机效果才会直接进行内联效果
                if (inlineEffectsAutoDo && typeWriterSwitch == TypeWriterSwitch.Off && pText != null && pText.haveAnimatorInline)
                {
                    pText.ApplyEffects(mAnim);
                }
            }), 0.1f);
        }

        //Tip：Play()不能直接用于开始，需要延迟等待数据加载完毕才能执行
        //提供了StartDo功能，如果不是StartDo，那么必然经过了一段时间
        //简单来说，就是：不要在Awake()/Start()中使用即可
        public void PlayText()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            MTextAnimatorCoroutine.StopAllCoroutines(text);
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                //重置数据
                mAnim.UpdateInfo();
                mAnim.UpdateTextInfo();

                PlayTypeWriterInternal();//Tip:执行完毕后如果存在内联动画会自动执行
            }), 0.05f);
        }
        public void PlayNewText()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            MTextAnimatorCoroutine.StopAllCoroutines(text);
            mAnim = new MTextAnimation(text, this);
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                text.alpha = 1;

                mAnim.UpdateTextInfo();//mAnim收集信息(textData)

                PlayTypeWriterInternal();
            }), 0.06f);

        }

        public void PlayTextFastly()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            MTextAnimatorCoroutine.StopAllCoroutines(text);
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                //重置数据
                mAnim.UpdateInfo();
                mAnim.UpdateTextInfo();

                PlayTypeWriterInternalFastly();//Tip:执行完毕后如果存在内联动画会自动执行
            }), 0.05f);
        }

        public void FinishTextImmediately()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            //if (mAnim.curTypeWriterFinishIndex == -1) return;
            if (mAnim.curTypeWriterStartIndex == -1) return;//当前并没有进行打字机效果，不应该结束
            if (mAnim.fastFinish) return;//正在加速，不能再次加速

            //TODO:暂时只有Show和Fade可以运行
            if (typewriterType != MTextTypewriterType.Show && typewriterType != MTextTypewriterType.Fade) return;

            mAnim.fastFinish = true;
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                FinishTypeWriterInternalFastly();
            }), 0.1f / typeSpeed);//TODO:0.1f是一个很笼统的时间，最准确的时间是：根据最后一个Tween目前执行到的插值决定出现的时机
        }

        private void PlayTypeWriterInternal()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            typewriterPlaying = true;

            //根据Inspector选取的模式进行动画
            switch (typewriterType)
            {
                case MTextTypewriterType.Show:
                    mAnim.TypeWriterShow(typeSpeed);
                    break;
                case MTextTypewriterType.Fade:
                    mAnim.TypeWriterFade(typeSpeed);
                    break;
                case MTextTypewriterType.Scale:
                    mAnim.TypeWriterScale(typeSpeed);
                    break;
                case MTextTypewriterType.Translation:
                    mAnim.TypeWriterTranslation(typeSpeed);
                    break;
                case MTextTypewriterType.Rotation:
                    mAnim.TypeWriterRotation(typeSpeed);
                    break;
                case MTextTypewriterType.Wave:
                    mAnim.TypeWriterWave(typeSpeed);
                    break;
                case MTextTypewriterType.Shake:
                    mAnim.TypeWriterShake(typeSpeed);
                    break;
            }
        }

        private void PlayTypeWriterInternalFastly()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            typewriterPlaying = true;
            float speed = 16777216;

            mAnim.TypeWriterFade(speed);//速度过快无法看出细节，直接渐入效果最好
        }

        private void FinishTypeWriterInternalFastly()
        {
            typewriterPlaying = true;

            float speed = 16777216;
            mAnim.TypeWriterFadePart(mAnim.curTypeWriterStartIndex + 1, mAnim.textData.charData.Length - 1, speed);//速度过快无法看出细节，直接渐入效果最好
        }
    }

    public enum TypeWriterSwitch
    {
        Off,
        On
    }
    public enum InlineEffectSwitch
    {
        Off,
        On
    }

    public enum MTextTypewriterType
    {
        Show,
        Fade,
        Scale,
        Translation,
        Rotation,
        Wave,
        Shake
    }
}