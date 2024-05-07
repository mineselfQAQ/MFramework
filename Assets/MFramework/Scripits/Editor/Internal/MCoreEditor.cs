using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static MFramework.MGUIOptionUtility;

namespace MFramework
{
    [CustomEditor(typeof(MCore))]
    public class MCoreEditor : Editor
    {
        public static Texture2D LOGOTex;

        //注意：SerializedProperty需要[SerializeField]才能获取
        //private SerializedProperty m_LogCallbackOnSP;

        private MCore mCore;

        /// <summary>
        /// 看到MCore时会触发一次(点击Hierarchy下的MCore在Inspector上看到MCore)
        /// </summary>
        private void OnEnable()
        {
            LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorResourcesPath.LOGOPath);

            mCore = (MCore)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MGUIUtility.DrawTexture(LOGOTex, MGUIStyleUtility.CenterStyle);

            //m_ExportLog
            Undo.RecordObject(target, "ChangeLogState");//记录Undo信息
            BoolEnum logEnum = mCore.GetExportLog() ? BoolEnum.ON : BoolEnum.OFF;
            var newLogEnum = (BoolEnum)EditorGUILayout.EnumPopup("是否输出LOG信息", logEnum);
            if (newLogEnum == BoolEnum.ON) mCore.SetExportLog(true);
            else mCore.SetExportLog(false);

            if (GUI.changed)
            {
                //并不需要
                //EditorUtility.SetDirty(target);
                //AssetDatabase.SaveAssets();
                //EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
