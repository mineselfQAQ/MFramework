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

        //ע�⣺SerializedProperty��Ҫ[SerializeField]���ܻ�ȡ
        //private SerializedProperty m_LogCallbackOnSP;

        private MCore mCore;

        /// <summary>
        /// ����MCoreʱ�ᴥ��һ��(���Hierarchy�µ�MCore��Inspector�Ͽ���MCore)
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
            Undo.RecordObject(target, "ChangeLogState");//��¼Undo��Ϣ
            BoolEnum logEnum = mCore.GetExportLog() ? BoolEnum.ON : BoolEnum.OFF;
            var newLogEnum = (BoolEnum)EditorGUILayout.EnumPopup("�Ƿ����LOG��Ϣ", logEnum);
            if (newLogEnum == BoolEnum.ON) mCore.SetExportLog(true);
            else mCore.SetExportLog(false);

            if (GUI.changed)
            {
                //������Ҫ
                //EditorUtility.SetDirty(target);
                //AssetDatabase.SaveAssets();
                //EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
