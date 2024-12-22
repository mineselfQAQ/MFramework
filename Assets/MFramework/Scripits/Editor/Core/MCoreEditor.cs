using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(MCore))]
    public class MCoreEditor : Editor
    {
        public static Texture2D LOGOTex;

        //***注意***：SerializedProperty需要[SerializeField]才能获取
        private SerializedProperty logStateSP;
        private SerializedProperty UICustomLoadStateSP;
        private SerializedProperty localStateSP;
        private SerializedProperty performanceStateSP;
        private SerializedProperty fpsDisplayModeSP;
        private SerializedProperty fpsSampleDurationSP;
        private SerializedProperty fpsKeycodeSP;

        private MCore mCore;

        /// <summary>
        /// 看到MCore时会触发一次(点击Hierarchy下的MCore在Inspector上看到MCore)
        /// </summary>
        private void OnEnable()
        {
            LOGOTex = MTextureLibrary.LOGOTex;
            
            mCore = (MCore)target;
            
            logStateSP = serializedObject.FindProperty("m_LogState");
            UICustomLoadStateSP = serializedObject.FindProperty("m_UICustomLoadState");
            localStateSP = serializedObject.FindProperty("m_LocalState");
            performanceStateSP = serializedObject.FindProperty("m_PerformanceState");
            fpsDisplayModeSP = serializedObject.FindProperty("m_FPSDisplayMode");
            fpsSampleDurationSP = serializedObject.FindProperty("m_FPSSampleDuration");
            fpsKeycodeSP = serializedObject.FindProperty("m_keycode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MEditorGUIUtility.DrawTexture(LOGOTex, MEditorGUIStyleUtility.CenterStyle);

            MEditorGUIUtility.DrawH2("编辑器");
            MEditorControlUtility.DrawBoolPopup(UICustomLoadStateSP, "是否启用UI自定义加载");
            MEditorGUIUtility.DrawH2("打包");
            MEditorControlUtility.DrawBoolPopup(logStateSP, "是否输出LOG信息");
            MEditorGUIUtility.DrawH2("本地化");
            MEditorControlUtility.DrawBoolPopup(localStateSP, "是否开启本地化");
            MEditorGUIUtility.DrawH2("性能检测");
            bool flag = MEditorControlUtility.DrawBoolPopup(performanceStateSP, "是否开启性能检测");
            if (flag)
            {
                EditorGUI.indentLevel++;
                MEditorControlUtility.DrawPopup<FPSMonitor.DisplayMode>(fpsDisplayModeSP, "FPS显示模式");
                MEditorControlUtility.DrawFloat(fpsSampleDurationSP, "采样间隔(秒)");
                MEditorControlUtility.DrawPopup<PerformanceMonitor.PKeycode>(fpsKeycodeSP, "显示/隐藏按键");
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
