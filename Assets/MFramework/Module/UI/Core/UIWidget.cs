using System;
using UnityEngine;

namespace MFramework.UI
{
    public class UIWidget : UIView
    {
        protected string WidgetID => ViewID;
        protected UIView ParentView;
        protected UIPanel Panel;
        public UIWidgetBehaviour WidgetBehaviour => (UIWidgetBehaviour)viewBehaviour;

        protected internal void Create(string id, Transform parentTrans, string prefabPath, UIView parent, bool autoEnter)
        {
            ParentView = parent;
            base.Create(id, parent.UIManager, parentTrans, prefabPath);
            ApplyInitialState(autoEnter);
        }

        protected internal void Create(string id, Transform parentTrans, UIViewBehaviour behaviour, UIView parent, bool autoEnter)
        {
            ParentView = parent;
            base.Create(id, parent.UIManager, parentTrans, behaviour);
            ApplyInitialState(autoEnter);
        }

        public void DestroySelf()
        {
            ParentView.DestroyWidget(WidgetID);
        }

        public void SetSiblingSelf(SiblingMode mode)
        {
            ParentView.SetWidgetSibiling(WidgetID, mode);
        }

        public void OpenSelf()
        {
            ParentView.OpenWidget(WidgetID);
        }

        public void CloseSelf()
        {
            ParentView.CloseWidget(WidgetID);
        }

        internal void SetSibling(SiblingMode mode)
        {
            if (mode == SiblingMode.Top) RectTransform.SetAsLastSibling();
            else RectTransform.SetAsFirstSibling();
        }

        internal bool Open(Action onFinish = null)
        {
            if (WidgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(true);
                if (flag)
                {
                    IsOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                }

                return flag;
            }

            if (WidgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayOpenAnim(() =>
                {
                    IsOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                });
            }

            IsOpen = true;
            bool opened = SetVisible(true);
            if (opened) OnVisibleChanged(true);
            onFinish?.Invoke();
            return opened;
        }

        internal bool Close(Action onFinish = null)
        {
            if (WidgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(false);
                if (flag)
                {
                    IsOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                }

                return flag;
            }

            if (WidgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayCloseAnim(() =>
                {
                    IsOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                });
            }

            IsOpen = false;
            bool closed = SetVisible(false);
            if (closed) OnVisibleChanged(false);
            onFinish?.Invoke();
            return closed;
        }

        internal bool SetVisible(bool visible)
        {
            if (ShowState == UIShowState.On && visible) return false;
            if (ShowState == UIShowState.Off && !visible) return false;

            UIPanelUtility.SetCanvasGroupActive(CanvasGroup, visible);
            ShowState = visible ? UIShowState.On : UIShowState.Off;
            return true;
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (WidgetBehaviour.AnimSwitch == UIAnimSwitch.Off)
            {
                onFinish?.Invoke();
                return false;
            }

            if (AnimState is UIAnimState.Opening or UIAnimState.Closing or UIAnimState.Opened) return false;

            AnimState = UIAnimState.Opening;

            void FinishOpenAnim()
            {
                AnimState = UIAnimState.Opened;
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
                onFinish?.Invoke();
            }

            if (WidgetBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                WidgetBehaviour.PlayOpenAnim(FinishOpenAnim);
            }
            else
            {
                PlaySelfControlAnim(OnOpenAnim(), FinishOpenAnim);
            }

            return true;
        }

        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (WidgetBehaviour.AnimSwitch == UIAnimSwitch.Off)
            {
                onFinish?.Invoke();
                return false;
            }

            if (AnimState is UIAnimState.Opening or UIAnimState.Closing or UIAnimState.Closed) return false;

            AnimState = UIAnimState.Closing;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            void FinishCloseAnim()
            {
                AnimState = UIAnimState.Closed;
                onFinish?.Invoke();
            }

            if (WidgetBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                WidgetBehaviour.PlayCloseAnim(FinishCloseAnim);
            }
            else
            {
                PlaySelfControlAnim(OnCloseAnim(), FinishCloseAnim);
            }

            return true;
        }

        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();
            WidgetBehaviour.View = this;
            WidgetBehaviour.CoroutineManager = UIManager.CoroutineManager;
            Panel = GameObject.GetComponentInParent<UIPanelBehaviour>()?.View as UIPanel;
        }

        protected internal override void DestroyingInternal()
        {
            ParentView = null;
            Panel = null;
            base.DestroyingInternal();
        }

        protected internal override void CreatedInternal()
        {
        }

        private void ApplyInitialState(bool autoEnter)
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                CanvasGroup.alpha = 0f;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
                return;
            }

            CanvasGroup.alpha = 1f;
            if (WidgetBehaviour.AnimSwitch == UIAnimSwitch.On) PlayOpenAnim();
            else SetVisible(true);
        }
    }
}
