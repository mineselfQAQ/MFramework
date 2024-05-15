using System;
using System.Collections.Generic;

namespace MFramework
{
    public class UIPanelUtility
    {
        internal static void ResetOrder(UIRoot root)
        {
            List<UIPanel> panels = FilterSortedPanel(root);
            if (panels.Count > 0) panels[0].SetSortingOrder(root.startOrder);
            for (int i = 1; i < panels.Count; i++)
            {
                int order = panels[i - 1].sortingOrder + panels[i - 1].panelBehaviour.thickness;
                if (order > root.endOrder)
                {
                    MLog.Print($"{root.rootID}已超容，请扩容或减少thickness", MLogType.Error);
                    break;
                }
                panels[i].SetSortingOrder(order);
            }
        }

        /// <summary>
        /// 筛选Panel，如果不筛选就会获取所有的Panel
        /// </summary>
        public static List<UIPanel> FilterPanels(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = new List<UIPanel>();

            foreach (KeyValuePair<string, UIPanel> kvPair in root.panelDic)
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
        public static UIPanel FilterTopestPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels.Count > 0 ? panels[panels.Count - 1] : null;
        }
        /// <summary>
        /// 筛选Panel并排序
        /// </summary>
        /// <param name="filterFunc"></param>
        /// <returns></returns>
        public static List<UIPanel> FilterSortedPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels;
        }
    }
}