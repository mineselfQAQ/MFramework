using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// UI的基本组件，处于UICanvas下级
    /// </summary>
    public class UIPanel : UIView
    {
        //UIView字段公开属性
        public string panelID { get { return viewID; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        //UIPanel字段
        public UIRoot parentRoot { private set; get; }//UIPanel所在的UIRoot
        public Canvas canvas { private set; get; }
        public GraphicRaycaster graphicRaycaster { private set; get; }

        //不太合理，prefab存在复用情况(如背包)，应该由用户决定
        //public static HashSet<string> panelPrefabSet = new HashSet<string>();//检测是否已经存放过某个prefab

        public int sortingOrder => canvas.sortingOrder;

        internal void Create(string id, UIRoot root, string prefabPath)
        {
            base.Create(id, UIManager.Instance.UICanvas.transform, prefabPath);
            parentRoot = root;
            //panelPrefabSet.Add(prefabName);

            PlayOpenAnim();
        }

        internal bool Open(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.On) 
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
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.On)
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

        internal void Destroy(Action onFinish = null)
        {
            PlayCloseAnim(() =>
            {
                base.Destroy();
                onFinish?.Invoke();
            });
        }

        //Tip:不添加自身操作，防止Root管理出错
        internal void SetSortingOrder(int order)
        {
            canvas.sortingOrder = order;//更改所属Canvas的sortingOrder
        }

        //TODO:可以加一个简单的过渡隐藏效果
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

        internal void SetFocus(bool focus)
        {
            OnFocusChanged(focus);
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.Off) return false;

            if (panelBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经打开的也无需再次执行)
                if (animState == UIAnimState.Opening || animState == UIAnimState.Closing || animState == UIAnimState.Opened)
                    return false;

                animState = UIAnimState.Opening;
                panelBehaviour.PlayOpenAnim(() => { animState = UIAnimState.Opened; onFinish?.Invoke(); });
            }
            else
            {
                MLog.Print($"UI：如果想使用SelfControl模式，请重写PlayOpenAnim()---{panelID}", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.Off) return false;

            if (panelBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经关闭的也无需再次执行)
                if (animState == UIAnimState.Opening || animState == UIAnimState.Closing || animState == UIAnimState.Closed)
                    return false;

                animState = UIAnimState.Closing;
                panelBehaviour.PlayCloseAnim(() => { animState = UIAnimState.Closed; onFinish?.Invoke(); });
            }
            else
            {
                MLog.Print($"UI：如果想使用SelfControl模式，请重写PlayCloseAnim()---{panelID}", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }

        #region 自身操作
        protected void DestroySelf()
        {
            parentRoot.panelDic.Remove(panelID);
            Destroy();
        }

        //protected void SetVisibleSelf(bool visible)
        //{
        //    SetVisible(visible);
        //}

        protected void OpenSelf(Action onFinish = null)
        {
            Open(onFinish);
        }
        protected void CloseSelf(Action onFinish = null)
        {
            Close(onFinish);
        }
        #endregion

        #region 内部生命周期
        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();

            //没必要强制设置RectTransform，Panel应该跟着原来Prefab的设置走
            //SetRectStretchMode();

            canvas = panelBehaviour.gameObject.GetOrAddComponent<Canvas>();
            graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();

            //使各个Panel可以正常排序(因为是嵌套的)
            canvas.overrideSorting = true;

            panelBehaviour.view = this;//捕获归属物
        }
        protected internal override void DestroyingInternal()
        {
            //UIBlocker.Instance.Unbind();

            canvasGroup = null;
            graphicRaycaster = null;
            canvas = null;
            parentRoot = null;

            base.DestroyingInternal();
        }
        protected internal override void CreatedInternal() { }
        protected internal override void DestroyedInternal() { }

        private void SetRectStretchMode()
        {
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            //trans.sizeDelta = trans.parent.GetComponent<RectTransform>().sizeDelta;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        #endregion

        #region 子类生命周期
        protected virtual void OnFocusChanged(bool focus) { }
        #endregion
    }
}