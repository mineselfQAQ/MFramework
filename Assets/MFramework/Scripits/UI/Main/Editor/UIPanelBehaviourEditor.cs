using UnityEditor;

namespace MFramework
{
    [CustomEditor(typeof(UIPanelBehaviour))]
    public class UIPanelBehaviourEditor : UIViewBehaviourEditor
    {
        //TODO:在AnimSwitch切换至Enabled时，需要开启Open/Close Anim Mode且生成相应内容(在Prefab中添加Animator并生成相应文件)

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawExportBtn();

            serializedObject.ApplyModifiedProperties();
        }
    }
}