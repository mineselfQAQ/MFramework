using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace MFramework.UI
{
    //TODO:添加后点击MText就会报错
    //[CustomEditor(typeof(MText), true)]
    public class MTextEditor : TMP_EditorPanel
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("添加MTextAnimator"))
            {
                //TODO
            }

            base.OnInspectorGUI();
        }
    }
}