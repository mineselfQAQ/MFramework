using System;
using System.Collections.Generic;

using MFramework.Core;

namespace MFramework.UI
{
    public class UIRoot
    {
        public string RootID { get; }
        public int StartOrder { get; }
        public int EndOrder { get; }
        public MUIManager UIManager { get; }
        public UIPanel TopPanel { internal set; get; }
        public int TopOrder => TopPanel == null ? -1 : TopPanel.SortingOrder;
        public Dictionary<string, UIPanel> PanelDic { get; } = new();

        public UIRoot(string id, int start, int end, MUIManager uiManager)
        {
            RootID = id;
            StartOrder = start;
            EndOrder = end;
            UIManager = uiManager;
        }

        public T CreatePanel<T>(string id, string prefabPath, int order, bool autoEnter = false) where T : UIPanel, new()
        {
            if (!CanCreatePanel(id, order)) return null;

            T panel = new T();
            panel.Create(id, this, prefabPath, autoEnter);
            panel.SetSortingOrder(order);
            PanelDic.Add(panel.PanelID, panel);
            MaintainTopPanelCreate(panel, order);
            return panel;
        }

        public T CreatePanel<T>(string id, string prefabPath, bool autoEnter = false) where T : UIPanel, new()
        {
            int order = GetNextOrder();
            if (order > EndOrder) UIPanelUtility.ResetOrder(this);
            return CreatePanel<T>(id, prefabPath, order, autoEnter);
        }

        public T CreatePanel<T>(string prefabPath, int order, bool autoEnter = false) where T : UIPanel, new()
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath, order, autoEnter);
        }

        public T CreatePanel<T>(string prefabPath, bool autoEnter = false) where T : UIPanel, new()
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath, autoEnter);
        }

        public T CreatePanel<T>(string id, UIPanelBehaviour behaviour, int order, bool autoEnter = false) where T : UIPanel, new()
        {
            if (!CanCreatePanel(id, order)) return null;

            T panel = new T();
            panel.Create(id, this, behaviour, autoEnter);
            panel.SetSortingOrder(order);
            PanelDic.Add(panel.PanelID, panel);
            MaintainTopPanelCreate(panel, order);
            return panel;
        }

        public T CreatePanel<T>(string id, UIPanelBehaviour behaviour, bool autoEnter = false) where T : UIPanel, new()
        {
            int order = GetNextOrder();
            if (order > EndOrder) UIPanelUtility.ResetOrder(this);
            return CreatePanel<T>(id, behaviour, order, autoEnter);
        }

        public T CreatePanel<T>(UIPanelBehaviour behaviour, int order, bool autoEnter = false) where T : UIPanel, new()
        {
            return CreatePanel<T>(typeof(T).Name, behaviour, order, autoEnter);
        }

        public T CreatePanel<T>(UIPanelBehaviour behaviour, bool autoEnter = false) where T : UIPanel, new()
        {
            return CreatePanel<T>(typeof(T).Name, behaviour, autoEnter);
        }

        public bool DestroyPanel(string id)
        {
            if (!PanelDic.TryGetValue(id, out UIPanel panel)) return false;

            PanelDic.Remove(id);
            MaintainTopPanelDestroy(panel);
            panel.Destroy();
            return true;
        }

        public bool DestroyPanel<T>()
        {
            return DestroyPanel(typeof(T).Name);
        }

        public T GetPanel<T>(string id) where T : UIPanel
        {
            return PanelDic.TryGetValue(id, out UIPanel panel) ? (T)panel : null;
        }

        public T GetPanel<T>() where T : UIPanel
        {
            return GetPanel<T>(typeof(T).Name);
        }

        public bool ExistPanel(string id)
        {
            return PanelDic.ContainsKey(id);
        }

        public bool ExistPanel<T>()
        {
            return ExistPanel(typeof(T).Name);
        }

        public void OpenPanel(string id, Action onFinish = null, bool pinToTop = false)
        {
            if (!PanelDic.TryGetValue(id, out UIPanel panel)) return;
            if (panel.AnimState == UIAnimState.Opened) return;

            bool flag = panel.Open(onFinish);
            if (flag) MaintainTopPanelOpen(panel, pinToTop);
        }

        public void OpenPanel(string id, bool pinToTop)
        {
            OpenPanel(id, null, pinToTop);
        }

        public void OpenPanel<T>(Action onFinish = null, bool pinToTop = false)
        {
            OpenPanel(typeof(T).Name, onFinish, pinToTop);
        }

        public void OpenPanel<T>(bool pinToTop)
        {
            OpenPanel(typeof(T).Name, pinToTop);
        }

        public void ClosePanel(string id, Action onFinish = null)
        {
            if (!PanelDic.TryGetValue(id, out UIPanel panel)) return;
            if (panel.AnimState == UIAnimState.Closed) return;

            bool flag = panel.Close(onFinish);
            if (flag) MaintainTopPanelClose(panel);
        }

        public void ClosePanel<T>(Action onFinish = null)
        {
            ClosePanel(typeof(T).Name, onFinish);
        }

        public void SetPanelSibling(string id, SiblingMode mode)
        {
            if (PanelDic.TryGetValue(id, out UIPanel panel)) panel.SetPanelSibling(mode);
        }

        public void SetPanelSibling<T>(SiblingMode mode)
        {
            SetPanelSibling(typeof(T).Name, mode);
        }

        public void SetSortingOrder(string id, int order)
        {
            if (PanelDic.TryGetValue(id, out UIPanel panel)) panel.SetSortingOrder(order);
        }

        public void SetSortingOrder<T>(int order)
        {
            SetSortingOrder(typeof(T).Name, order);
        }

        private bool CanCreatePanel(string id, int order)
        {
            if (PanelDic.ContainsKey(id))
            {
                MLog.Default.W($"{nameof(UIRoot)}: panel '{id}' already exists in root '{RootID}'.");
                return false;
            }

            if (order < StartOrder || order > EndOrder)
            {
                MLog.Default.W($"{nameof(UIRoot)}: order {order} is outside [{StartOrder}, {EndOrder}].");
                return false;
            }

            return true;
        }

        private void MaintainTopPanelCreate(UIPanel panel, int order)
        {
            if (panel.PanelBehaviour.FocusMode == UIPanelFocusMode.Disabled) return;
            if (TopPanel == null || order >= TopOrder) TopPanel = panel;
        }

        private void MaintainTopPanelDestroy(UIPanel panel)
        {
            if (panel.PanelBehaviour.FocusMode == UIPanelFocusMode.Disabled) return;

            panel.SetFocus(false);
            if (TopPanel != panel) return;

            TopPanel = UIPanelUtility.FilterTopestPanel(this);
            TopPanel?.SetFocus(true);
        }

        private void MaintainTopPanelOpen(UIPanel panel, bool pinToTop)
        {
            if (panel.PanelBehaviour.FocusMode == UIPanelFocusMode.Disabled) return;

            int order = TopPanel == null
                ? (UIPanelUtility.FilterTopestPanel(this)?.SortingOrder ?? StartOrder) + panel.PanelBehaviour.Thickness
                : TopOrder + TopPanel.PanelBehaviour.Thickness;

            if (pinToTop)
            {
                panel.SetSortingOrder(order);
                if (order > EndOrder) UIPanelUtility.ResetOrder(this);
                TopPanel?.SetFocus(false);
                panel.SetFocus(true);
                TopPanel = panel;
            }
            else if (TopPanel == null || panel.SortingOrder >= TopOrder)
            {
                TopPanel = panel;
            }
        }

        private void MaintainTopPanelClose(UIPanel panel)
        {
            if (panel.PanelBehaviour.FocusMode == UIPanelFocusMode.Disabled) return;

            panel.SetFocus(false);
            if (TopPanel != panel) return;

            TopPanel = UIPanelUtility.FilterTopestPanel(this, candidate => candidate != panel && candidate.SortingOrder <= TopOrder);
            TopPanel?.SetFocus(true);
        }

        private int GetNextOrder()
        {
            return TopPanel == null ? StartOrder : TopPanel.Canvas.sortingOrder + TopPanel.PanelBehaviour.Thickness;
        }
    }
}
