using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// UIPanel的子物体，可深层嵌套
    /// </summary>
    public class UIWidget : UIView
    {
        protected string widgetID { get { return viewID; } }

        protected UIView parentView;
        protected UIPanel panel;//归属Panel

        public UIWidgetBehaviour widgetBehaviour { get { return (UIWidgetBehaviour)viewBehaviour; } }

        private bool firstEnter;

        protected internal void Create(string id, Transform parentTrans, string prefabPath, UIView parent, bool autoEnter)
        {
            parentView = parent;
            base.Create(id, parentTrans, prefabPath);

            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                firstEnter = true;

                CanvasGroup.alpha = 0;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
                CanvasGroup.alpha = 1;
                if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    PlayOpenAnim();
                }
                else
                {
                    SetVisible(true);
                }
            }
        }
        protected internal void Create(string id, Transform parentTrans, UIViewBehaviour behaviour, UIView parent, bool autoEnter)
        {
            parentView = parent;
            base.Create(id, parentTrans, behaviour);

            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                CanvasGroup.alpha = 0;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
                CanvasGroup.alpha = 1;
                if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    PlayOpenAnim();
                }
                else
                {
                    SetVisible(true);
                }
            }
        }

        #region 自身操作
        public void DestroySelf()
        {
            parentView.DestroyWidget(widgetID);
        }
        public void SetSiblingSelf(SiblingMode mode)
        {
            parentView.SetWidgetSibiling(widgetID, mode);
        }
        public void OpenSelf()
        {
            parentView.OpenWidget(widgetID);
        }
        public void CloseSelf()
        {
            parentView.CloseWidget(widgetID);
        }
        #endregion

        #region 核心操作
        internal void SetSibling(SiblingMode mode)
        {
            if (mode == SiblingMode.Top) SetToTop();
            else if (mode == SiblingMode.Bottom) SetToBottom();
        }
        internal void SetToTop()
        {
            rectTransform.SetAsLastSibling();
        }
        internal void SetToBottom()
        {
            rectTransform.SetAsLastSibling();
        }

        internal bool Open(Action onFinish = null)
        {
            //Simple模式自动调用SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(true);
                if (flag)
                {
                    isOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                }
                return flag;
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                if (CanvasGroup.alpha == 0) 
                    UIPanelUtility.SetCanvasGroupActive(CanvasGroup, true);//autoEnter导致的第一次进入

                return PlayOpenAnim(() =>
                {
                    isOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                });
            }
            else
            {
                isOpen = true;
                bool flag = SetVisible(true);
                if (flag) OnVisibleChanged(true);
                onFinish?.Invoke();
                return flag;
            }
        }
        internal bool Close(Action onFinish = null)
        {
            //Simple模式自动调用SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(false);
                if (flag)
                {
                    isOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                } 

                return flag;
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayCloseAnim(() =>
                {
                    isOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                });
            }
            else
            {
                isOpen = false;
                bool flag = SetVisible(false);
                if (flag) OnVisibleChanged(false);
                onFinish?.Invoke();
                return flag;
            }
        }

        internal bool SetVisible(bool visible, bool enableTransition = false)
        {
            if (ShowState == UIShowState.On && visible) { return false; }
            if (ShowState == UIShowState.Off && !visible) { return false; }

            UIPanelUtility.SetCanvasGroupActive(CanvasGroup, visible);

            ShowState = visible ? UIShowState.On : UIShowState.Off;

            return true;
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off)
            {
                onFinish?.Invoke();
                return false;
            } 

            if (widgetBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经打开的也无需再次执行)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Opened)
                    return false;

                AnimState = UIAnimState.Opening;
                //Tip：只有第一次进入时才会设置alpha值，alpha值应该由Animation设置
                if (firstEnter)
                {
                    firstEnter = false;
                    CanvasGroup.alpha = 1;
                }
                widgetBehaviour.PlayOpenAnim(() => 
                {
                    AnimState = UIAnimState.Opened;
                    //对于开启情况，需要等动画结束后使整体激活，防止再次点击
                    CanvasGroup.interactable = true;
                    CanvasGroup.blocksRaycasts = true;
                    onFinish?.Invoke(); 
                });
            }
            else
            {
                MLog.Print($"{typeof(UIWidget)}：ID-<{widgetID}>重写PlayOpenAnim()后才能使用SelfControl模式，请检查", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off) 
            {
                onFinish?.Invoke();
                return false;
            }

            if (widgetBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经关闭的也无需再次执行)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Closed)
                    return false;

                AnimState = UIAnimState.Closing;
                //对于关闭情况，需要提前使整体失活，防止再次点击
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
                widgetBehaviour.PlayCloseAnim(() =>
                {
                    AnimState = UIAnimState.Closed;
                    onFinish?.Invoke(); 
                });
            }
            else
            {
                MLog.Print($"{typeof(UIWidget)}：ID-<{widgetID}>重写PlayCloseAnim()后才能使用SelfControl模式，请检查", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        #endregion

        #region 内部生命周期
        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();

            widgetBehaviour.view = this;//捕获归属物
            panel = (UIPanel)gameObject.GetComponentInParent<UIPanelBehaviour>().view;
        }
        protected internal override void DestroyingInternal()
        {
            parentView = null;
            panel = null;

            base.DestroyingInternal();
        }
        protected internal override void CreatedInternal() { }
        protected internal override void DestroyedInternal() { }
        #endregion
    }
}