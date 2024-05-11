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

        public static HashSet<string> panelPrefabSet = new HashSet<string>();//检测是否已经存放过某个prefab

        public int sortingOrder => canvas.sortingOrder;

        internal void Create(string id, UIRoot root, string prefabPath)
        {
            panelPrefabSet.Add(prefabName);
            parentRoot = root;
            base.Create(id, UIManager.Instance.UICanvas.transform, prefabPath);

            //PlayOpenAnim(null);
        }

        internal new void Destroy()
        {
            base.Destroy();
        }

        public void DestroySelf()
        {
            parentRoot.panelDic.Remove(panelID);
            Destroy();
        }

        internal void SetSortingOrder(int order)
        {
            canvas.sortingOrder = order;//更改所属Canvas的sortingOrder
        }

        #region 内部生命周期
        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();

            SetRectStretchMode();//TODO:存疑，是否需要强制设置Transform模式

            canvas = panelBehaviour.gameObject.GetOrAddComponent<Canvas>();
            graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();

            //使各个Panel可以正常排序(因为是嵌套的)
            canvas.overrideSorting = true;
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
    }
}