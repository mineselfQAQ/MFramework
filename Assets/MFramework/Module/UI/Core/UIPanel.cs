using System;

using MFramework.Core;

using UnityEngine;
using UnityEngine.UI;

namespace MFramework.UI
{
    public class UIPanel : UIView
    {
        public string PanelID => ViewID;
        public UIPanelBehaviour PanelBehaviour => (UIPanelBehaviour)viewBehaviour;
        public UIRoot ParentRoot { private set; get; }
        public Canvas Canvas { private set; get; }
        public GraphicRaycaster GraphicRaycaster { private set; get; }
        public int SortingOrder => Canvas.sortingOrder;

        internal void Create(string id, UIRoot root, string prefabPath, bool autoEnter)
        {
            base.Create(id, root.UIManager, root.UIManager.UICanvas.transform, prefabPath);
            ParentRoot = root;
            ApplyInitialState(autoEnter);
        }

        internal void Create(string id, UIRoot root, UIPanelBehaviour behaviour, bool autoEnter)
        {
            if (behaviour.transform.parent != root.UIManager.UICanvas.transform)
            {
                MLog.Default.E($"{nameof(UIPanel)}: '{id}' must be a direct child of the UI canvas.");
            }

            base.Create(id, root.UIManager, root.UIManager.UICanvas.transform, behaviour);
            ParentRoot = root;
            ApplyInitialState(autoEnter);
        }

        internal void Destroy(Action onFinish = null)
        {
            PlayCloseAnim(() =>
            {
                base.Destroy();
                onFinish?.Invoke();
            });
        }

        internal bool Open(Action onFinish = null)
        {
            if (PanelBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayOpenAnim(() =>
                {
                    IsOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                });
            }

            IsOpen = true;
            bool flag = SetVisible(true);
            if (flag) OnVisibleChanged(true);
            onFinish?.Invoke();
            return flag;
        }

        internal bool Close(Action onFinish = null)
        {
            if (PanelBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayCloseAnim(() =>
                {
                    IsOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                });
            }

            IsOpen = false;
            bool flag = SetVisible(false);
            if (flag) OnVisibleChanged(false);
            onFinish?.Invoke();
            return flag;
        }

        internal void SetSortingOrder(int order)
        {
            Canvas.sortingOrder = order;
        }

        internal void SetPanelSibling(SiblingMode mode)
        {
            if (mode == SiblingMode.Top)
            {
                int order = ParentRoot.TopOrder + ParentRoot.TopPanel.PanelBehaviour.Thickness;
                SetSortingOrder(order);
                if (order > ParentRoot.EndOrder) UIPanelUtility.ResetOrder(ParentRoot);
            }
            else
            {
                UIPanel bottomPanel = UIPanelUtility.FilterBottommostPanel(ParentRoot);
                int order = bottomPanel.SortingOrder - PanelBehaviour.Thickness;
                SetSortingOrder(order);
                if (order < ParentRoot.StartOrder) UIPanelUtility.ResetOrder(ParentRoot);
            }
        }

        internal bool SetVisible(bool visible)
        {
            if (ShowState == UIShowState.On && visible) return false;
            if (ShowState == UIShowState.Off && !visible) return false;

            UIPanelUtility.SetCanvasGroupActive(CanvasGroup, visible);
            ShowState = visible ? UIShowState.On : UIShowState.Off;
            return true;
        }

        internal void SetFocus(bool focus)
        {
            OnFocusChanged(focus);
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (PanelBehaviour.AnimSwitch == UIAnimSwitch.Off)
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

            if (PanelBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                PanelBehaviour.PlayOpenAnim(FinishOpenAnim);
            }
            else
            {
                PlaySelfControlAnim(OnOpenAnim(), FinishOpenAnim);
            }

            return true;
        }

        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (PanelBehaviour.AnimSwitch == UIAnimSwitch.Off)
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

            if (PanelBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                PanelBehaviour.PlayCloseAnim(FinishCloseAnim);
            }
            else
            {
                PlaySelfControlAnim(OnCloseAnim(), FinishCloseAnim);
            }

            return true;
        }

        public void DestroySelf()
        {
            ParentRoot.PanelDic.Remove(PanelID);
            Destroy();
        }

        public void SetPanelSiblingSelf(SiblingMode mode)
        {
            SetPanelSibling(mode);
        }

        public void OpenSelf(Action onFinish = null)
        {
            Open(onFinish);
        }

        public void CloseSelf(Action onFinish = null)
        {
            Close(onFinish);
        }

        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();
            Canvas = PanelBehaviour.gameObject.GetOrAddComponent<Canvas>();
            GraphicRaycaster = GameObject.GetOrAddComponent<GraphicRaycaster>();
            Canvas.overrideSorting = true;
            PanelBehaviour.View = this;
            PanelBehaviour.CoroutineManager = UIManager.CoroutineManager;
        }

        protected internal override void DestroyingInternal()
        {
            CanvasGroup = null;
            GraphicRaycaster = null;
            Canvas = null;
            ParentRoot = null;
            base.DestroyingInternal();
        }

        protected virtual void OnFocusChanged(bool focus)
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
            if (PanelBehaviour.AnimSwitch == UIAnimSwitch.On) PlayOpenAnim();
            else SetVisible(true);
        }
    }
}
