using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    public class UIPanel : UIView
    {
        //UIView字段公开属性
        public string panelID { get { return viewID; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        //UIPanel字段
        public UIRoot parentRoot { private set; get; }//UIPanel所在的UIRoot
        public Canvas canvas { private set; get; }
        public GraphicRaycaster graphicRaycaster { private set; get; }
        public CanvasGroup canvasGroup { private set; get; }

        public UIPanelShowState showState { protected set; get; } = UIPanelShowState.On;
        public UIPanelAnimState animState { protected set; get; } = UIPanelAnimState.Idle;

        public static HashSet<string> panelPrefabSet = new HashSet<string>();//检测是否已经存放过某个prefab

        public int sortingOrder => canvas.sortingOrder;

        internal void Create(string id, UIRoot root, string prefabPath)
        {
            base.Create(id, UIManager.Instance.UICanvas.transform, prefabPath);
            parentRoot = root;
            panelPrefabSet.Add(prefabName);

            PlayOpenAnim();
        }

        internal void Open(Action onFinish = null)
        {
            PlayOpenAnim(() =>
            {
                onFinish?.Invoke();
            });
        }

        internal void Close(Action onFinish = null)
        {
            PlayCloseAnim(() =>
            {
                onFinish?.Invoke();
            });
        }

        internal void Destroy(Action onFinish = null)
        {
            PlayCloseAnim(() =>
            {
                base.Destroy();
                onFinish?.Invoke();
            });
        }

        internal void SetSortingOrder(int order)
        {
            canvas.sortingOrder = order;//更改所属Canvas的sortingOrder
        }

        internal void SetVisible(bool visible)
        {
            if (showState == UIPanelShowState.On && visible) { return; }
            if (showState == UIPanelShowState.Off && !visible) { return; }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;

            showState = visible ? UIPanelShowState.On : UIPanelShowState.Off;

            OnVisibleChanged(visible);
        }

        protected virtual void PlayOpenAnim(Action onFinish = null)
        {
            if (panelBehaviour.openAnimMode == UIPanelOpenAnimMode.Disabled) return;

            if (panelBehaviour.openAnimMode == UIPanelOpenAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经打开的也无需再次执行)
                if (animState == UIPanelAnimState.Opening || animState == UIPanelAnimState.Closing || animState == UIPanelAnimState.Opened)
                    return;

                animState = UIPanelAnimState.Opening;
                panelBehaviour.PlayOpenAnim(() => { animState = UIPanelAnimState.Opened; onFinish?.Invoke(); });
            }
            else
            {
                MLog.Print($"UI：如果想使用SelfControl模式，请重写PlayOpenAnim()---{panelID}", MLogType.Warning);
                onFinish?.Invoke();
            }
        }
        protected virtual void PlayCloseAnim(Action onFinish = null)
        {
            if (panelBehaviour.closeAnimMode == UIPanelCloseAnimMode.Disabled) return;

            if (panelBehaviour.closeAnimMode == UIPanelCloseAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经关闭的也无需再次执行)
                if (animState == UIPanelAnimState.Opening || animState == UIPanelAnimState.Closing || animState == UIPanelAnimState.Closed)
                    return;

                animState = UIPanelAnimState.Closing;
                panelBehaviour.PlayCloseAnim(() => { animState = UIPanelAnimState.Closed; onFinish?.Invoke(); });
            }
            else
            {
                MLog.Print($"UI：如果想使用SelfControl模式，请重写PlayCloseAnim()---{panelID}", MLogType.Warning);
                onFinish?.Invoke();
            }
        }

        #region 自身操作
        public void DestroySelf()
        {
            parentRoot.panelDic.Remove(panelID);
            Destroy();
        }

        public void SetVisibleSelf(bool visible)
        {
            SetVisible(visible);
        }
        #endregion

        #region 内部生命周期
        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();

            SetRectStretchMode();//TODO:存疑，是否需要强制设置Transform模式

            canvas = panelBehaviour.gameObject.GetOrAddComponent<Canvas>();
            graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();

            //使各个Panel可以正常排序(因为是嵌套的)
            canvas.overrideSorting = true;

            panelBehaviour.panel = this;//捕获归属物
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
            trans.anchorMin = new Vector2(0f, 0f);
            trans.anchorMax = new Vector2(1f, 1f);
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;
            trans.pivot = new Vector2(0.5f, 0.5f);
            
            //trans.sizeDelta = trans.parent.GetComponent<RectTransform>().sizeDelta;
            trans.anchoredPosition = Vector2.zero;
        }
        #endregion

        #region 子类生命周期
        protected virtual void OnVisibleChanged(bool visible) { }
        #endregion
    }
}