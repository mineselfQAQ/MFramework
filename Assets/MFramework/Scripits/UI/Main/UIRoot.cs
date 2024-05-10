using System;
using System.Collections.Generic;

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
                MLog.Print($"UI：已创建id-{id}，请勿重复创建", MLogType.Warning);
                return null;
            }
            if (order < startOrder || order > endOrder)
            {
                MLog.Print($"UI：order-{order}不在<{id}>的[{startOrder},{endOrder}]区间，请重试", MLogType.Warning);
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
    }
}