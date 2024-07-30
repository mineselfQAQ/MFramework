using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// UIView的MonoBehaviour脚本，用于Prefab的基本设置以及收集组件
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UIViewBehaviour : MonoBehaviour
    {
        [SerializeField]
        private List<UICompCollection> compCollections;
        public List<UICompCollection> CompCollections { get { return compCollections; } }

        internal UIView view;//归属View

        //动画模式---是否开启动画，如开启动画会使用Animator进行播放，否则为瞬切(CanvasGroup设0/1)
        [SerializeField] private UIAnimSwitch animSwitch = UIAnimSwitch.Off;
        [SerializeField] private UIOpenAnimMode openAnimMode = UIOpenAnimMode.AutoPlay;
        [SerializeField] private UICloseAnimMode closeAnimMode = UICloseAnimMode.AutoPlay;

        public UIAnimSwitch AnimSwitch { get { return animSwitch; } }
        public UIOpenAnimMode OpenAnimMode { get { return openAnimMode; } }
        public UICloseAnimMode CloseAnimMode { get { return closeAnimMode; } }

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
        }
#endif

        /// <summary>
        /// 获取某个Collection的某个Component
        /// </summary>
        public Component GetComp(int collectionIndex, int compIndex)
        {
            return compCollections[collectionIndex].GetComp(compIndex);
        }

        #region 动画操作
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
                MLog.Print($"{typeof(UIViewBehaviour)}：<{view}>中不存在Animator组件，请检查", MLogType.Warning);
                onFinish();
                return;
            }
            if (animator.runtimeAnimatorController == null)
            {
                MLog.Print($"{typeof(UIViewBehaviour)}：ID-<{view.viewID}>中不存在Controller，请检查", MLogType.Warning);
                onFinish();
                return;
            }

            animator.updateMode = AnimatorUpdateMode.UnscaledTime;//暂停(TimeScale=0)时可用
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
                MLog.Print($"{typeof(UIViewBehaviour)}：ID-<{view.viewID}>中的Animator未找到合适的Clip，请检查", MLogType.Warning);
                onFinish();
                return;
            }

            MCoroutineManager.Instance.DelayNoRecord(onFinish, length);
        }
        #endregion
    }
}