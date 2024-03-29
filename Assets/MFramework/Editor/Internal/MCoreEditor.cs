using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(MCore))]
    public class MCoreEditor : Editor
    {
        public const string LOGOPath = @"Assets\MFramework\Resources\LOGO.png";
        public static Texture2D LOGOTex;

        private SerializedProperty m_LogCallbackOn;

        private void Awake()
        {
            LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(LOGOPath);
        }

        public override void OnInspectorGUI()
        {
            DrawLOGO();

            //base.OnInspectorGUI();
            MCore mCore = (MCore)target;

        }

        private void DrawLOGO()
        {
            if (LOGOTex != null)
            {
                //TODO:需要一张大约为256*128的贴图
                GUILayout.Label(LOGOTex, MGUIStyleUtility.CenterStyle);
            }
        }
    }
}
