using System;
using System.Collections.Generic;
using MFramework.Coroutines;
using UnityEngine;

namespace MFramework.UI
{
    [DisallowMultipleComponent]
    public abstract class UIViewBehaviour : MonoBehaviour
    {
        [SerializeField] private List<UICompCollection> compCollections = new();
        [SerializeField] private UIAnimSwitch animSwitch = UIAnimSwitch.Off;
        [SerializeField] private UIOpenAnimMode openAnimMode = UIOpenAnimMode.AutoPlay;
        [SerializeField] private UICloseAnimMode closeAnimMode = UICloseAnimMode.AutoPlay;

        internal UIView View;
        internal MCoroutineManager CoroutineManager;

        public List<UICompCollection> CompCollections => compCollections;
        public UIAnimSwitch AnimSwitch => animSwitch;
        public UIOpenAnimMode OpenAnimMode => openAnimMode;
        public UICloseAnimMode CloseAnimMode => closeAnimMode;

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
        }
#endif

        public Component GetComp(int collectionIndex, int compIndex)
        {
            return compCollections[collectionIndex].GetComp(compIndex);
        }

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
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                onFinish?.Invoke();
                return;
            }

            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.SetTrigger(operationName);

            float length = 0f;
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                string[] parts = clip.name.Split('_');
                if (parts.Length > 0 && parts[^1] == operationName)
                {
                    length = clip.length;
                    break;
                }
            }

            if (length <= 0f || CoroutineManager == null)
            {
                onFinish?.Invoke();
                return;
            }

            CoroutineManager.DelayNoRecord(onFinish, length);
        }
    }
}
