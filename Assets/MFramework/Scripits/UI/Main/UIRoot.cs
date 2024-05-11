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

        public Dictionary<string, UIPanel> panelDic { private set; get; }

        public UIRoot(string id, int start, int end)
        {
            rootID = id;
            startOrder = start;
            endOrder = end;

            panelDic = new Dictionary<string, UIPanel>();
        }

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
            //设置组内排序(Panel与Panel之间的排序)
            //int siblingIndex = GetCurrentSiblingIndex(sortingOrder);
            //panel.SetSiblingIndex(siblingIndex);
            panelDic.Add(panel.panelID, panel);//加入Panel字典
            //UIManager.Instance.SetBackgroundAndFocus();//*大致来说为设置背景与聚焦排序*

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
                MLog.Print($"Root-{rootID}下没有<{id}>，无法删除，请检查", MLogType.Warning);
                return false;
            }

            UIPanel panel = panelDic[id];
            panelDic.Remove(id);
            panel.Destroy();

            //UIManager.Instance.SetBackgroundAndFocus();

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
                MLog.Print($"Root-{rootID}下没有<{id}>，获取失败，请检查", MLogType.Warning);
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
    }
}