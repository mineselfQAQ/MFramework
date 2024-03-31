using UnityEditor;
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

            BoolEnum logEnum = mCore.m_LogCallbackOn ? BoolEnum.ON : BoolEnum.OFF;
            var newLogEnum = (BoolEnum)EditorGUILayout.EnumPopup("是否开启LOG输出", logEnum);
            if (newLogEnum == BoolEnum.ON) mCore.m_LogCallbackOn = true;
            else mCore.m_LogCallbackOn = false;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
