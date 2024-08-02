using System;

namespace MFramework
{
    public static class MUIUtitlity
    {
        private static UIRoot topestRoot;

        private static UIPanel simpleFadePanel;

        private static readonly string panelPrepath = "Assets/MFramework/Scripits/Runtime/UI/Core/Preset";
        private static readonly string rootName = "MUIUtilityRoot";
        private static readonly string simpleFadePanelName = "SIMPLEFADE";

        /// <summary>
        /// ºÚÆÁ½øÈë
        /// </summary>
        public static void BlackIn(Action onFinish)
        {
            if (topestRoot == null) topestRoot = UIManager.Instance.CreateRoot(rootName, 9999, 9999);

            if (simpleFadePanel == null)
            {
                simpleFadePanel = topestRoot.CreatePanel<SimpleFadePanel>(simpleFadePanelName, $"{panelPrepath}/SimpleFadePanel/SimpleFadePanel.prefab", false);
                simpleFadePanel.Open(onFinish);
            }
            else
            {
                simpleFadePanel.Open(onFinish);
            }
        }
        /// <summary>
        /// ºÚÆÁÍË³ö
        /// </summary>
        public static void BlackOut(Action onFinish)
        {
            if (simpleFadePanel == null)
            {
                return;
            }
            else
            {
                simpleFadePanel.Close(onFinish);
            }
        }
    }
}
