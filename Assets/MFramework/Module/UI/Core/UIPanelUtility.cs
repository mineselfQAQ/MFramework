using System;
using System.Collections.Generic;

using MFramework.Core;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MFramework.UI
{
    public static class UIPanelUtility
    {
#if UNITY_EDITOR
        public static string GetPrefabPath(UnityEngine.Object target)
        {
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(target);
            string targetAssetPath = AssetDatabase.GetAssetPath(target);
            string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(targetAssetPath)) return targetAssetPath;
            if (prefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(prefabAssetPath)) return prefabAssetPath;
            return prefabStage?.assetPath;
        }
#endif

        internal static void ResetOrder(UIRoot root)
        {
            List<UIPanel> panels = FilterSortedPanel(root);
            if (panels.Count <= 0) return;

            panels[0].SetSortingOrder(root.StartOrder);
            for (int i = 1; i < panels.Count; i++)
            {
                int order = panels[i - 1].SortingOrder + panels[i - 1].PanelBehaviour.Thickness;
                if (order > root.EndOrder)
                {
                    MLog.Default.E($"{nameof(UIPanelUtility)}.{nameof(ResetOrder)}: Root '{root.RootID}' order range is full.");
                    break;
                }

                panels[i].SetSortingOrder(order);
            }
        }

        public static List<UIPanel> FilterPanels(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = new();
            foreach (KeyValuePair<string, UIPanel> pair in root.PanelDic)
            {
                if (filterFunc == null || filterFunc(pair.Value))
                {
                    panels.Add(pair.Value);
                }
            }

            return panels;
        }

        public static List<UIPanel> FilterSortedPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);
            panels.Sort((a, b) => a.Canvas.sortingOrder - b.Canvas.sortingOrder);
            return panels;
        }

        public static UIPanel FilterTopestPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterSortedPanel(root, filterFunc);
            return panels.Count > 0 ? panels[^1] : null;
        }

        public static UIPanel FilterBottommostPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterSortedPanel(root, filterFunc);
            return panels.Count > 0 ? panels[0] : null;
        }

        public static bool SetCanvasGroupActive(CanvasGroup canvasGroup, bool active)
        {
            if (canvasGroup == null) return false;

            canvasGroup.alpha = active ? 1f : 0f;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
            return true;
        }
    }
}
