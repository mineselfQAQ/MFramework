using System;
using System.Collections.Generic;
using System.IO;

namespace MFramework
{
    /// <summary>
    /// UIPanel的管理类
    /// </summary>
    public class UIRoot
    {
        public string rootID;
        public int startOrder;
        public int endOrder;
        public UIPanel topPanel { internal set; get; }
        public int topOrder => topPanel.sortingOrder;

        public Dictionary<string, UIPanel> panelDic { private set; get; }

        public UIRoot(string id, int start, int end)
        {
            rootID = id;
            startOrder = start;
            endOrder = end;

            panelDic = new Dictionary<string, UIPanel>();
        }

        #region Panel基础操作
        //Tip：
        //无id函数为便捷用法，此时id为T类型的名称
        //关键：这意味着此类函数一种T只能控制一个Panel，如有多个同类型Panel，请提供各自id
        public T CreatePanel<T>(string id, string prefabPath, int order) where T : UIPanel
        {
            if (panelDic.ContainsKey(id))
            {
                MLog.Print($"UI：{rootID}中已创建id-{id}，请勿重复创建", MLogType.Warning);
                return null;
            }
            if (order < startOrder || order > endOrder)
            {
                MLog.Print($"UI：order-{order}不在<{id}>的[{startOrder},{endOrder}]区间，请重试", MLogType.Warning);
                return null;
            }
            string name = Path.GetFileNameWithoutExtension(prefabPath);
            if (UIPanel.panelPrefabSet.Contains(name))
            {
                MLog.Print($"UI：{name}.prefab已创建，请勿重复创建", MLogType.Warning);
                return null;
            }

            T panel = Activator.CreateInstance(typeof(T)) as T;//创建实例
            panel.Create(id, this, prefabPath);//创建Panel
            panel.SetSortingOrder(order);//设置排序(Canvas之间的排序)
            panelDic.Add(panel.panelID, panel);//加入Panel字典

            MaintainTopPanel_Create(panel, order);

            return panel;
        }
        public T CreatePanel<T>(string id, string prefabPath) where T : UIPanel
        {
            int order = GetNextOrder();
            if (order > endOrder)
            {
                UIPanelUtility.ResetOrder(this);
            }

            return CreatePanel<T>(id, prefabPath, order);
        }
        public T CreatePanel<T>(string prefabPath, int order) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath, order);
        }
        public T CreatePanel<T>(string prefabPath) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath);
        }

        public bool DestroyPanel(string id)
        {
            if (!panelDic.ContainsKey(id))
            {
                MLog.Print($"UI：Root-{rootID}下没有<{id}>，无法删除，请检查", MLogType.Warning);
                return false;
            }

            UIPanel panel = panelDic[id];
            panelDic.Remove(id);
            MaintainTopPanel_Destroy(panel);
            panel.Destroy();

            return true;
        }
        public bool DestroyPanel<T>()
        {
            return DestroyPanel(typeof(T).Name);
        }

        public T GetPanel<T>(string id) where T : UIPanel
        {
            if (!panelDic.ContainsKey(id))
            {
                MLog.Print($"UI：Root-{rootID}下没有<{id}>，获取失败，请检查", MLogType.Warning);
                return default(T);
            };

            return (T)panelDic[id];
        }
        public T GetPanel<T>() where T : UIPanel
        {
            return GetPanel<T>(typeof(T).Name);
        }

        public bool ExistPanel(string id)
        {
            return panelDic.ContainsKey(id);
        }
        public bool ExistPanel<T>()
        {
            return panelDic.ContainsKey(typeof(T).Name);
        }

        public void SetPanelVisible(string id, bool visible, bool pinToTop = false)
        {
            if (!panelDic.ContainsKey(id))
            {
                MLog.Print($"UI：Root-{rootID}下没有<{id}>，设置失败，请检查", MLogType.Warning);
                return;
            }

            UIPanel panel = panelDic[id];
            bool flag = panel.SetVisible(visible);
            if(flag) MaintainTopPanel_Visible(panel, visible, pinToTop);
        }
        public void SetPanelVisible<T>(bool visible, bool pinToTop = false)
        {
            SetPanelVisible(typeof(T).Name, visible, pinToTop);
        }

        public void OpenPanel(string id, Action onFinish = null, bool pinToTop = false)
        {
            if (!panelDic.ContainsKey(id))
            {
                MLog.Print($"UI：Root-{rootID}下没有<{id}>，打开失败，请检查", MLogType.Warning);
                return;
            }

            UIPanel panel = panelDic[id];
            bool flag = panel.Open(onFinish);
            if (flag) MaintainTopPanel_Open(panel, pinToTop);
        }
        public void OpenPanel(string id, bool pinToTop = false)
        {
            OpenPanel(id, null, pinToTop);
        }
        public void OpenPanel<T>(Action onFinish = null, bool pinToTop = false)
        {
            OpenPanel(typeof(T).Name, onFinish);
        }
        public void OpenPanel<T>(bool pinToTop = false)
        {
            OpenPanel(typeof(T).Name, pinToTop);
        }

        public void ClosePanel(string id, Action onFinish = null)
        {
            if (!panelDic.ContainsKey(id))
            {
                MLog.Print($"UI：Root-{rootID}下没有<{id}>，关闭失败，请检查", MLogType.Warning);
                return;
            }

            UIPanel panel = panelDic[id];
            if (panel.showState == UIPanelShowState.Off) return;//已经关闭
            bool flag = panel.Close(onFinish);
            if(flag) MaintainTopPanel_Close(panel);
        }
        public void ClosePanel<T>(Action onFinish = null)
        {
            ClosePanel(typeof(T).Name, onFinish);
        }

        private void MaintainTopPanel_Create<T>(T panel, int order) where T : UIPanel
        {
            if (panel.panelBehaviour.focusMode == UIPanelFocusMode.Disabled) return;

            if (topPanel == null || order >= topOrder)
            {
                topPanel = panel;
            }
            //创建时有Create回调，不需要Focus回调
            //topPanel.SetFocus(true);
        }
        private void MaintainTopPanel_Destroy<T>(T panel) where T : UIPanel
        {
            if (panel.panelBehaviour.focusMode == UIPanelFocusMode.Disabled) return;

            panel.SetFocus(false);
            if (panel.sortingOrder == topOrder)
            {
                topPanel = UIPanelUtility.FilterTopestPanel(this);
            }
            topPanel.SetFocus(true);
        }
        private void MaintainTopPanel_Visible<T>(T panel, bool visible, bool pinToTop) where T : UIPanel
        {
            if (panel.panelBehaviour.focusMode == UIPanelFocusMode.Disabled) return;

            if (visible && pinToTop)//启用Panel且强制置顶
            {
                MaintainTopPanel_Open(panel, pinToTop);
            }
            else if (!visible)//禁用Panel
            {
                MaintainTopPanel_Close(panel);
            }
        }
        private void MaintainTopPanel_Open<T>(T panel, bool pinToTop) where T : UIPanel
        {
            if (panel.panelBehaviour.focusMode == UIPanelFocusMode.Disabled) return;

            int order = topOrder + topPanel.panelBehaviour.thickness;
            if (pinToTop)
            {
                panel.SetSortingOrder(order);
                if (order > endOrder) UIPanelUtility.ResetOrder(this);

                topPanel.SetFocus(false);
                panel.SetFocus(true);

                topPanel = panel;
            }
            else
            {
                if (panel.sortingOrder >= topOrder)
                {
                    topPanel = panel;
                }
            }
        }
        private void MaintainTopPanel_Close<T>(T panel) where T : UIPanel
        {
            if (panel.panelBehaviour.focusMode == UIPanelFocusMode.Disabled) return;

            panel.SetFocus(false);
            topPanel = UIPanelUtility.FilterTopestPanel(this, (panel) =>
            { return panel != topPanel && panel.sortingOrder <= topOrder; });
            topPanel.SetFocus(true);
        }

        private int GetNextOrder()
        {
            //1.panelDic为空，得0(startOrder)
            //2.panelDic不为空，得topPanel+厚度
            return topPanel == null ? startOrder :
                topPanel.canvas.sortingOrder + topPanel.panelBehaviour.thickness;
            #endregion
        }
    }
}