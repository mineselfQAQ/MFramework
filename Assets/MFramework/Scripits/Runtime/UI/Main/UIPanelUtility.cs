using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MFramework
{
    public static class UIPanelUtility
    {
#if UNITY_EDITOR
        public static string GetPrefabPath(UnityEngine.Object target)
        {
            //大致就是：
            //在不同状态下点击Inspector的Export按钮，使用同一方法并不都能获取到路径
            //1---最普通的，就是点击Project面板下的Prefab，此时就是最常规的AssetDatabase.GetAssetPath()
            //2---点击Hierarchy下的Prefab，此时只有通过PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot()才能获取
            //3---在预制体面板中点击，prefabStage.assetPath可以获取

            PrefabAssetType singlePrefabType = PrefabUtility.GetPrefabAssetType(target);
            PrefabInstanceStatus singleInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(target);
            string targetAssetPath = AssetDatabase.GetAssetPath(target);
            string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            //需要覆盖并正确判断这三种情况
            string finalPrefabPath = null;
            if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(targetAssetPath))
            {
                finalPrefabPath = targetAssetPath;//点击预设时
            }
            else if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(prefabAssetPath))
            {
                finalPrefabPath = prefabAssetPath;//预设拖入Hierarchy并选择时
            }
            else if (prefabStage != null)
            {
                finalPrefabPath = prefabStage.assetPath;//双击预设并在Hierarchy上选择时
            }

            return finalPrefabPath;
        }
#endif

        internal static void ResetOrder(UIRoot root)
        {
            List<UIPanel> panels = FilterSortedPanel(root);
            if (panels.Count > 0) panels[0].SetSortingOrder(root.startOrder);
            for (int i = 1; i < panels.Count; i++)
            {
                int order = panels[i - 1].sortingOrder + panels[i - 1].panelBehaviour.Thickness;
                if (order > root.endOrder)
                {
                    MLog.Print($"{typeof(UIPanelUtility)}.{nameof(ResetOrder)}：RootID-<{root.rootID}>已超容，请扩容或减少Thickness", MLogType.Error);
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
        /// 筛选Panel并排序
        /// </summary>
        /// <param playerName="filterFunc"></param>
        /// <returns></returns>
        public static List<UIPanel> FilterSortedPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

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
        /// 筛选Panel并获取最下层Panel
        /// </summary>
        public static UIPanel FilterBottommostPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels.Count > 0 ? panels[0] : null;
        }

        public static bool SetCanvasGroupActive(CanvasGroup canvasGroup, bool active)
        {
            if (canvasGroup == null) return false;

            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
            return true;
        }
    }
}