using System;
using UnityEngine;

namespace MFramework
{
    public enum UIPanelOpenAnimMode
    {
        Disabled,
        AutoPlay,
        SelfControl
    }
    public enum UIPanelCloseAnimMode
    {
        Disabled,
        AutoPlay,
        SelfControl
    }

    public class UIPanelBehaviour : UIViewBehaviour
    {
        [HideInInspector]
        internal UIPanel panel;//归属Panel

        //必须>=1
        public int thinkness = 10;//Panel的厚度(该Panel与下一Panel之间的sortingOrder距离)

        //除了AutoPlay/SelfControl，还隐藏了一种---不启用动画(通过Editor控制)
        public UIPanelOpenAnimMode openAnimMode;
        public UIPanelCloseAnimMode closeAnimMode;

        internal void PlayOpenAnim(Action onFinish)
        {
            PlayAnim(onFinish, "Open");
        }
        internal void PlayCloseAnim(Action onFinish)
        {
            PlayAnim(onFinish, "Close");
        }

        private void PlayAnim(Action onFinish, string operationName)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                MLog.Print($"{panel.panelID}中不存在Animator组件，请检查", MLogType.Warning);
                onFinish();
                return;
            }
            if (animator.runtimeAnimatorController == null)
            {
                MLog.Print($"{panel.panelID}中不存在Controller，请检查", MLogType.Warning);
                onFinish();
                return;
            }

            animator.SetTrigger(operationName);

            float length = 0;
            bool flag = false;
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                //通过后缀确定是否为Open Animation
                string[] strs = clip.name.Split('_');
                string suffix = strs[strs.Length - 1];

                if (suffix == operationName) { length = clip.length; flag = true; break; }
            }
            if (!flag)
            {
                MLog.Print($"{panel.panelID}中的Animator未找到合适的Clip，请检查", MLogType.Warning);
                onFinish();
                return;
            }

            StartCoroutine(CoroutineUtility.Delay(onFinish, length));
        }
    }
}