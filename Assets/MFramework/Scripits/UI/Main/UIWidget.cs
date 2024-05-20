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

        protected internal void Create(string id, Transform parentTrans, string prefabPath, UIView parent)
        {
            parentView = parent;
            base.Create(id, parentTrans, prefabPath);
            panel = (UIPanel)gameObject.GetComponentInParent<UIPanelBehaviour>().view;
        }
        protected internal void Create(string id, Transform parentTrans, UIViewBehaviour behaviour, UIView parent)
        {
            parentView = parent;
            base.Create(id, parentTrans, behaviour);
            panel = (UIPanel)gameObject.GetComponentInParent<UIPanelBehaviour>().view;
        }

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

        #region 高级模式操作
        internal bool Open(Action onFinish = null)
        {
            //Simple模式自动调用SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                return SetVisible(true);
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayOpenAnim(() =>
                {
                    onFinish?.Invoke();
                });
            }
            else
            {
                return SetVisible(true);
            }
        }
        internal bool Close(Action onFinish = null)
        {
            //Simple模式自动调用SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                return SetVisible(false);
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayCloseAnim(() =>
                {
                    onFinish?.Invoke();
                });
            }
            else
            {
                return SetVisible(false);
            }
        }

        internal bool SetVisible(bool visible, bool enableTransition = false)
        {
            if (showState == UIShowState.On && visible) { return false; }
            if (showState == UIShowState.Off && !visible) { return false; }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;

            showState = visible ? UIShowState.On : UIShowState.Off;

            OnVisibleChanged(visible);

            return true;
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off) return false;

            if (widgetBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经打开的也无需再次执行)
                if (animState == UIAnimState.Opening || animState == UIAnimState.Closing || animState == UIAnimState.Opened)
                    return false;

                animState = UIAnimState.Opening;
                widgetBehaviour.PlayOpenAnim(() => { animState = UIAnimState.Opened; onFinish?.Invoke(); });
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
                if (animState == UIAnimState.Opening || animState == UIAnimState.Closing || animState == UIAnimState.Closed)
                    return false;

                animState = UIAnimState.Closing;
                widgetBehaviour.PlayCloseAnim(() => { animState = UIAnimState.Closed; onFinish?.Invoke(); });
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
        }
        protected internal override void DestroyingInternal()
        {
            parentView = null;

            base.DestroyingInternal();
        }
        protected internal override void CreatedInternal() { }
        protected internal override void DestroyedInternal() { }
        #endregion
    }
}