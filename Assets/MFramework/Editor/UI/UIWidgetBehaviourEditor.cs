using MFramework.Core;
using MFramework.UI;
using UnityEditor;
using UnityEngine;

namespace MFramework.Editor.UI
{
    [CustomEditor(typeof(UIWidgetBehaviour))]
    public class UIWidgetBehaviourEditor : UIViewBehaviourEditor
    {
        private UIWidgetBehaviour _behaviour;
        private SerializedProperty _widgetModeProperty;
        private SerializedProperty _animSwitchProperty;
        private SerializedProperty _openAnimModeProperty;
        private SerializedProperty _closeAnimModeProperty;

        private static readonly GUIContent WidgetModeLabel = new("Widget Mode", "Widget behavior mode.");
        private static readonly GUIContent AnimSwitchLabel = new("Animation Switch", "Enable Animator based open/close animation.");
        private static readonly GUIContent OpenAnimModeLabel = new("Open Animation Mode", "Open animation control mode.");
        private static readonly GUIContent CloseAnimModeLabel = new("Close Animation Mode", "Close animation control mode.");

        private void OnEnable()
        {
            _behaviour = (UIWidgetBehaviour)target;
            _widgetModeProperty = serializedObject.FindProperty("widgetMode");
            _animSwitchProperty = serializedObject.FindProperty("animSwitch");
            _openAnimModeProperty = serializedObject.FindProperty("openAnimMode");
            _closeAnimModeProperty = serializedObject.FindProperty("closeAnimMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawWidgetSettings();
            EditorGUILayout.Space(4f);
            DrawCompCollections();
            EditorGUILayout.Space(4f);
            DrawExportButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWidgetSettings()
        {
            UIWidgetMode widgetMode = UIEditorUtility.DrawPopup<UIWidgetMode>(_widgetModeProperty, WidgetModeLabel);
            if (widgetMode == UIWidgetMode.Simple) return;

            EditorGUI.indentLevel++;
            int lastAnimSwitch = _animSwitchProperty.enumValueIndex;
            UIAnimSwitch animSwitch = UIEditorUtility.DrawPopup<UIAnimSwitch>(_animSwitchProperty, AnimSwitchLabel);
            if (animSwitch == UIAnimSwitch.Off)
            {
                EditorGUI.indentLevel--;
                return;
            }

            if (EnsureAnimator(lastAnimSwitch))
            {
                EditorGUI.indentLevel++;
                UIEditorUtility.DrawPopup<UIOpenAnimMode>(_openAnimModeProperty, OpenAnimModeLabel);
                UIEditorUtility.DrawPopup<UICloseAnimMode>(_closeAnimModeProperty, CloseAnimModeLabel);
                DrawAnimatorControllerTools();
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        private bool EnsureAnimator(int lastAnimSwitch)
        {
            Animator animator = _behaviour.GetComponent<Animator>();
            if (animator == null)
            {
                bool create = EditorUtility.DisplayDialog("Animator Required", "Animation needs an Animator component. Create one?", "Create Animator", "Disable Animation");
                if (!create)
                {
                    _animSwitchProperty.enumValueIndex = 0;
                    return false;
                }

                animator = _behaviour.gameObject.AddComponent<Animator>();
                MLog.Default?.W("Animator created. Assign an AnimatorController before using UI animation.");
                return false;
            }

            if (animator.runtimeAnimatorController == null && lastAnimSwitch == 0 && _animSwitchProperty.enumValueIndex == 1)
            {
                MLog.Default?.W("Animator has no controller. Assign one before using UI animation.");
            }

            return true;
        }

        private void DrawAnimatorControllerTools()
        {
            Animator animator = _behaviour.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null) return;

            if (GUILayout.Button("Create UI AnimatorController"))
            {
                CreateUIAnimatorController.CreateForBehaviour(_behaviour);
            }
        }
    }
}
