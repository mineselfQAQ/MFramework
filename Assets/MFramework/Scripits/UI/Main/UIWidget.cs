using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

        protected internal void Create(string id, Transform parentTrans, string prefabPath, UIView parent, bool autoEnter)
        {
            parentView = parent;
            base.Create(id, parentTrans, prefabPath);

            if (!autoEnter)
            {
                UIPanelUtility.SetCanvasGroupActive(CanvasGroup, false);

                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
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

            if (!autoEnter)
            {
                UIPanelUtility.SetCanvasGroupActive(CanvasGroup, false);

                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
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
        protected void DestroySelf()
        {
            parentView.DestroyWidget(widgetID);
        }
        protected void SetSiblingSelf(SiblingMode mode)
        {
            parentView.SetWidgetSibiling(widgetID, mode);
        }
        protected void OpenSelf()
        {
            parentView.OpenWidget(widgetID);
        }
        protected void CloseSelf()
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
                if(flag) OnVisibleChanged(true);
                return flag;
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                if (CanvasGroup.alpha == 0) 
                    UIPanelUtility.SetCanvasGroupActive(CanvasGroup, true);//autoEnter导致的第一次进入

                return PlayOpenAnim(() =>
                {
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                });
            }
            else
            {
                bool flag = SetVisible(true);
                if (flag) OnVisibleChanged(true);
                return flag;
            }
        }
        internal bool Close(Action onFinish = null)
        {
            //Simple模式自动调用SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(false);
                if (flag) OnVisibleChanged(false);
                return flag;
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayCloseAnim(() =>
                {
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                });
            }
            else
            {
                bool flag = SetVisible(false);
                if (flag) OnVisibleChanged(false);
                return flag;
            }
        }

        internal bool SetVisible(bool visible, bool enableTransition = false)
        {
            if (ShowState == UIShowState.On && visible) { return false; }
            if (ShowState == UIShowState.Off && !visible) { return false; }

            CanvasGroup.alpha = visible ? 1 : 0;
            CanvasGroup.interactable = visible;
            CanvasGroup.blocksRaycasts = visible;

            ShowState = visible ? UIShowState.On : UIShowState.Off;

            return true;
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off) return false;

            if (widgetBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经打开的也无需再次执行)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Opened)
                    return false;

                AnimState = UIAnimState.Opening;
                widgetBehaviour.PlayOpenAnim(() => { AnimState = UIAnimState.Opened; onFinish?.Invoke(); });
            }
            else
            {
                MLog.Print($"UI：如果想使用SelfControl模式，请重写PlayOpenAnim()---{widgetID}", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off) return false;

            if (widgetBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经关闭的也无需再次执行)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Closed)
                    return false;

                AnimState = UIAnimState.Closing;
                widgetBehaviour.PlayCloseAnim(() => { AnimState = UIAnimState.Closed; onFinish?.Invoke(); });
            }
            else
            {
                MLog.Print($"UI：如果想使用SelfControl模式，请重写PlayCloseAnim()---{widgetID}", MLogType.Warning);
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