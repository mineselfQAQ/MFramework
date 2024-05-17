using UnityEditor;

namespace MFramework
{
    [CustomEditor(typeof(UIWidgetBehaviour))]
    public class UIWidgetBehaviourEditor : UIViewBehaviourEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCompCollections();
            DrawExportBtn();

            serializedObject.ApplyModifiedProperties();
        }
    }
}