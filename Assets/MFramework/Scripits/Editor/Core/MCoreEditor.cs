using UnityEditor;
using UnityEngine;
using static MFramework.MGUIOptionUtility;

namespace MFramework
{
    [CustomEditor(typeof(MCore))]
    public class MCoreEditor : Editor
    {
        public static Texture2D LOGOTex;

        //***ע��***��SerializedProperty��Ҫ[SerializeField]���ܻ�ȡ
        private SerializedProperty logStateSP;
        private SerializedProperty UICustomLoadStateSP;
        private SerializedProperty localStateSP;

        private MCore mCore;

        /// <summary>
        /// ����MCoreʱ�ᴥ��һ��(���Hierarchy�µ�MCore��Inspector�Ͽ���MCore)
        /// </summary>
        private void OnEnable()
        {
            LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorResourcesPath.LOGOPath);

            mCore = (MCore)target;

            logStateSP = serializedObject.FindProperty("m_LogState");
            UICustomLoadStateSP = serializedObject.FindProperty("m_UICustomLoadState");
            localStateSP = serializedObject.FindProperty("m_LocalState");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MGUIUtility.DrawTexture(LOGOTex, MGUIStyleUtility.CenterStyle);

            MGUIUtility.DrawH2("�༭��");
            DrawEnumPopup(UICustomLoadStateSP, "�Ƿ�����UI�Զ������");
            MGUIUtility.DrawH2("���");
            DrawEnumPopup(logStateSP, "�Ƿ����LOG��Ϣ");
            MGUIUtility.DrawH2("���ػ�");
            DrawEnumPopup(localStateSP, "�Ƿ������ػ�");

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEnumPopup(SerializedProperty property, string label)
        {
            BoolEnum currentEnum = property.boolValue ? BoolEnum.ON : BoolEnum.OFF;
            var newEnum = (BoolEnum)EditorGUILayout.EnumPopup(label, currentEnum);
            if (newEnum != currentEnum)
            {
                property.boolValue = newEnum == BoolEnum.ON;
            }
        }
    }
}
