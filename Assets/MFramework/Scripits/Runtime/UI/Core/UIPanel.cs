
using System;
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

        private bool firstEnter;

        internal void Create(string id, UIRoot root, string prefabPath, bool autoEnter)
        {
            base.Create(id, UIManager.Instance.UICanvas.transform, prefabPath);
            parentRoot = root;
            //panelPrefabSet.Add(prefabName);

            //打开需要的默认设置(需要alpha0-1的话是动画中进行的，如果不改的话alpha就应该为1，interactable/blocksRaycasts会在动画执行完毕后更新)
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                firstEnter = true;

                //UIPanelUtility.SetCanvasGroupActive(CanvasGroup, false);
                CanvasGroup.alpha = 0;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
                CanvasGroup.alpha = 1;
                if (panelBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    PlayOpenAnim();
                }
                else
                {
                    SetVisible(true);
                }
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

        internal bool Open(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.On) 
            {
                //if (CanvasGroup.alpha == 0)
                //    UIPanelUtility.SetCanvasGroupActive(CanvasGroup, true);//autoEnter导致的第一次进入

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
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.On)
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

        //Tip:不添加自身操作，防止Root管理出错
        internal void SetSortingOrder(int order)
        {
            canvas.sortingOrder = order;//更改所属Canvas的sortingOrder
        }
        internal void SetPanelSibling(SiblingMode mode)
        {
            UIRoot root = parentRoot;
            if (mode == SiblingMode.Top)
            {
                int order = root.topOrder + root.topPanel.panelBehaviour.Thickness;
                SetSortingOrder(order);
                if (order > root.endOrder) UIPanelUtility.ResetOrder(root);
            }
            else if (mode == SiblingMode.Bottom)
            {
                UIPanel bottomPanel = UIPanelUtility.FilterBottommostPanel(root);
                int order = bottomPanel.sortingOrder - panelBehaviour.Thickness;
                SetSortingOrder(order);
                if (order < root.startOrder) UIPanelUtility.ResetOrder(root);
            }
        }

        //TODO:可以加一个简单的过渡隐藏效果
        internal bool SetVisible(bool visible, bool enableTransition = false)
        {
            if (ShowState == UIShowState.On && visible) { return false; }
            if (ShowState == UIShowState.Off && !visible) { return false; }

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
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.Off)
            {
                onFinish?.Invoke();
                return false;
            }

            if (panelBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
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
                panelBehaviour.PlayOpenAnim(() => 
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
                MLog.Print($"{typeof(UIPanel)}：ID-<{panelID}>重写PlayOpenAnim()后才能使用SelfControl模式，请检查", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.Off) 
            {
                onFinish?.Invoke();
                return false;
            }

            if (panelBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                //正在操作的内容无法再次执行(已经关闭的也无需再次执行)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Closed)
                    return false;

                AnimState = UIAnimState.Closing;
                //对于关闭情况，需要提前使整体失活，防止再次点击
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
                panelBehaviour.PlayCloseAnim(() =>
                {
                    //CanvasGroup.alpha = 0;
                    AnimState = UIAnimState.Closed;
                    onFinish?.Invoke(); 
                });
            }
            else
            {
                MLog.Print($"{typeof(UIPanel)}：ID-<{panelID}>重写PlayCloseAnim()后才能使用SelfControl模式，请检查", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }

        #region 自身操作
        public void DestroySelf()
        {
            parentRoot.panelDic.Remove(panelID);
            Destroy();
        }

        //protected void SetVisibleSelf(bool visible)
        //{
        //    SetVisible(visible);
        //}
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
            CanvasGroup = null;
            graphicRaycaster = null;
            canvas = null;
            parentRoot = null;

            base.DestroyingInternal();
        }
        protected internal override void CreatedInternal() { base.CreatedInternal(); }
        protected internal override void DestroyedInternal() { base.DestroyedInternal(); }

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