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
        //TODO:ID用路径中的文件名可能更好
        //TODO:目前Focus逻辑混乱，而且需要调用panel.SetFocus()，需要封装函数
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

            //维护topPanel
            if (topPanel == null || order >= topOrder)
            {
                topPanel = panel;
            }

            return panel;
        }
        public T CreatePanel<T>(string id, string prefabPath) where T : UIPanel
        {
            int order = GetNextOrder();
            return CreatePanel<T>(id, prefabPath, order);
        }
        public T CreatePanel<T>(string prefabPath, int order) where T : UIPanel
        {
            return CreatePanel<T>(typeof(T).Name, prefabPath, order);
        }
        public T CreatePanel<T>(string prefabPath) where T : UIPanel
        {
            int order = GetNextOrder();
            return CreatePanel<T>(typeof(T).Name, prefabPath, order);
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
            //维护topPanel
            if (panel.sortingOrder == topOrder)
            {
                topPanel = FilterTopestPanel();
            }
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
            //维护topPanel
            if (visible && pinToTop && panel.sortingOrder <= topOrder)//启用Panel且强制置顶
            {
                panel.SetSortingOrder(topOrder + topPanel.panelBehaviour.thinkness);
                topPanel = panel;
            }
            else if (!visible)
            {
                topPanel = FilterTopestPanel((panel) =>
                { return panel != topPanel && panel.sortingOrder <= topOrder; });
            }
            panel.SetVisible(visible);
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
            if (panel.showState == UIPanelShowState.On) return;//已经打开
            //维护topPanel
            if (pinToTop && panel.sortingOrder <= topOrder) panel.SetSortingOrder(topOrder + topPanel.panelBehaviour.thinkness);
            topPanel = panel;
            panel.Open(onFinish);
        }
        public void OpenPanel<T>(Action onFinish = null, bool pinToTop = false)
        {
            OpenPanel(typeof(T).Name, onFinish);
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
            //维护topPanel
            topPanel = FilterTopestPanel((panel) =>
                { return panel != topPanel && panel.sortingOrder <= topOrder; });
            panel.Close(onFinish);
        }
        public void ClosePanel<T>(Action onFinish = null)
        {
            ClosePanel(typeof(T).Name, onFinish);
        }


        private int GetNextOrder()
        {
            UIPanel topPanel = null;
            foreach (var panel in panelDic.Values)
            {
                if (topPanel == null || panel.canvas.sortingOrder > topPanel.canvas.sortingOrder)
                {
                    topPanel = panel;
                }
            }

            //1.panelDic为空，得0(startOrder)
            //2.panelDic不为空，得topPanel+厚度
            return topPanel == null ? startOrder : topPanel.canvas.sortingOrder + topPanel.panelBehaviour.thinkness;
        }

        /// <summary>
        /// 筛选Panel，如果不筛选就会获取所有的Panel
        /// </summary>
        private List<UIPanel> FilterPanels(Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = new List<UIPanel>();

            foreach (KeyValuePair<string, UIPanel> kvPair in panelDic)
            {
                if (filterFunc == null || filterFunc(kvPair.Value))
                {
                    panels.Add(kvPair.Value);
                }
            }
            return panels;
        }
        /// <summary>
        /// 筛选Panel并获取最上层Panel
        /// </summary>
        private UIPanel FilterTopestPanel(Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels.Count > 0 ? panels[panels.Count - 1] : null;
        }
        #endregion
    }
}