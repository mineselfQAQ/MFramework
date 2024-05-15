using System;
using UnityEngine;

namespace MFramework
{
    public enum UIPanelGetFocusMode
    {
        Disabled,
        Get,
        //与其它配合的焦点设置
    }

    public enum UIPanelAnimSwitch
    {
        Enabled,
        Disabled
    }

    public enum UIPanelOpenAnimMode
    {
        AutoPlay,
        SelfControl
    }
    public enum UIPanelCloseAnimMode
    {
        AutoPlay,
        SelfControl
    }

    public class UIPanelBehaviour : UIViewBehaviour
    {
        internal UIPanel panel;//归属Panel

        //必须>=1
        public int thickness = 10;//Panel的厚度(该Panel与下一Panel之间的sortingOrder距离)

        public UIPanelGetFocusMode getFocusMode = UIPanelGetFocusMode.Get;

        public UIPanelAnimSwitch animSwitch = UIPanelAnimSwitch.Disabled;
        public UIPanelOpenAnimMode openAnimMode = UIPanelOpenAnimMode.AutoPlay;
        public UIPanelCloseAnimMode closeAnimMode = UIPanelCloseAnimMode.AutoPlay;

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