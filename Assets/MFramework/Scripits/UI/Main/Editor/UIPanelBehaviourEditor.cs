using UnityEditor;

namespace MFramework
{
    [CustomEditor(typeof(UIPanelBehaviour))]
    public class UIPanelBehaviourEditor : UIViewBehaviourEditor
    {
        //protected override void OnEnable()
        //{
        //    base.OnEnable();
        //}

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            //DrawUICompCollectionListRL();//OpElementList÷ÿªÊ
            DrawExportButton();

            serializedObject.ApplyModifiedProperties();
        }
    }
}