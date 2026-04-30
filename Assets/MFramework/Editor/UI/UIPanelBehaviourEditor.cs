using MFramework.Core;
using MFramework.UI;
using UnityEditor;
using UnityEngine;

namespace MFramework.Editor.UI
{
    [CustomEditor(typeof(UIPanelBehaviour))]
    public class UIPanelBehaviourEditor : UIViewBehaviourEditor
    {
        private UIPanelBehaviour _behaviour;
        private SerializedProperty _thicknessProperty;
        private SerializedProperty _focusModeProperty;
        private SerializedProperty _animSwitchProperty;
        private SerializedProperty _openAnimModeProperty;
        private SerializedProperty _closeAnimModeProperty;

        private static readonly GUIContent ThicknessLabel = new("Thickness", "Panel sorting distance to the next panel.");
        private static readonly GUIContent FocusModeLabel = new("Focus Mode", "Whether this panel can receive focus when clicked.");
        private static readonly GUIContent AnimSwitchLabel = new("Animation Switch", "Enable Animator based open/close animation.");
        private static readonly GUIContent OpenAnimModeLabel = new("Open Animation Mode", "Open animation control mode.");
        private static readonly GUIContent CloseAnimModeLabel = new("Close Animation Mode", "Close animation control mode.");

        private void OnEnable()
        {
            _behaviour = (UIPanelBehaviour)target;
            _thicknessProperty = serializedObject.FindProperty("thickness");
            _focusModeProperty = serializedObject.FindProperty("focusMode");
            _animSwitchProperty = serializedObject.FindProperty("animSwitch");
            _openAnimModeProperty = serializedObject.FindProperty("openAnimMode");
            _closeAnimModeProperty = serializedObject.FindProperty("closeAnimMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPanelSettings();
            EditorGUILayout.Space(4f);
            DrawCompCollections();
            EditorGUILayout.Space(4f);
            DrawExportButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPanelSettings()
        {
            int thickness = UIEditorUtility.DrawInt(_thicknessProperty, ThicknessLabel);
            _thicknessProperty.intValue = Mathf.Clamp(thickness, 1, int.MaxValue);

            UIEditorUtility.DrawPopup<UIPanelFocusMode>(_focusModeProperty, FocusModeLabel);

            int lastAnimSwitch = _animSwitchProperty.enumValueIndex;
            UIAnimSwitch animSwitch = UIEditorUtility.DrawPopup<UIAnimSwitch>(_animSwitchProperty, AnimSwitchLabel);
            if (animSwitch == UIAnimSwitch.Off) return;

            if (!EnsureAnimator(lastAnimSwitch)) return;

            EditorGUI.indentLevel++;
            UIEditorUtility.DrawPopup<UIOpenAnimMode>(_openAnimModeProperty, OpenAnimModeLabel);
            UIEditorUtility.DrawPopup<UICloseAnimMode>(_closeAnimModeProperty, CloseAnimModeLabel);
            DrawAnimatorControllerTools();
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
