using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(Transform))]
    public class HideTransformEditor : Editor
    {
        private Editor transformInspector;

        private void OnEnable()
        {
            Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));
            Type transformInspectorType = editorAssembly.GetType("UnityEditor.TransformInspector");

            transformInspector = CreateEditor(targets, transformInspectorType);
        }

        public override void OnInspectorGUI()
        {
            Transform targetTransform = (Transform)target;
            if (targetTransform.gameObject.name == "MCore") return;

            transformInspector.OnInspectorGUI();
        }

        private void OnDisable()
        {
            DestroyImmediate(transformInspector);
        }
    }
}
