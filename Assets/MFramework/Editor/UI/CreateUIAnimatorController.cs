using System.IO;
using MFramework.Core;
using MFramework.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MFramework.Editor.UI
{
    internal static class CreateUIAnimatorController
    {
        private const string OpenTrigger = "Open";
        private const string CloseTrigger = "Close";

        [MenuItem("Assets/MCreate/UI/UIAnimatorController", false, priority = 1)]
        private static void Create()
        {
            if (!TryGetSelectedUIPrefab(out GameObject prefab, out string prefabPath)) return;

            string objectName = prefab.name;
            string directoryPath = Path.GetDirectoryName(prefabPath)?.Replace('\\', '/');
            string controllerPath = $"{directoryPath}/{objectName}.controller";
            string openClipPath = $"{directoryPath}/{objectName}_Open.anim";
            string closeClipPath = $"{directoryPath}/{objectName}_Close.anim";

            AnimatorController controller = CreateAnimatorController(objectName, controllerPath, openClipPath, closeClipPath);
            AssignControllerToPrefab(prefabPath, controller);
        }

        [MenuItem("Assets/MCreate/UI/UIAnimatorController", true)]
        private static bool ValidateCreate()
        {
            return Selection.objects is { Length: 1 } &&
                   Selection.objects[0] is GameObject gameObject &&
                   PrefabUtility.IsPartOfPrefabAsset(gameObject) &&
                   HasUIBehaviour(gameObject);
        }

        internal static AnimatorController CreateAnimatorController(string objectName, string controllerPath, string openClipPath, string closeClipPath)
        {
            if (File.Exists(controllerPath) || File.Exists(openClipPath) || File.Exists(closeClipPath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Overwrite UI Animator",
                    $"{objectName} already has a controller or animation clip in this folder. Recreate all three files?",
                    "Overwrite",
                    "Cancel");

                if (!overwrite)
                {
                    MLog.Default?.W("Canceled UI AnimatorController generation.");
                    return AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
                }

                DeleteAssetIfExists(controllerPath);
                DeleteAssetIfExists(openClipPath);
                DeleteAssetIfExists(closeClipPath);
            }

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            controller.AddParameter(OpenTrigger, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(CloseTrigger, AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState idleState = stateMachine.AddState("Idle");
            stateMachine.defaultState = idleState;

            AnimatorState openState = stateMachine.AddState("OpenState");
            AnimatorState closeState = stateMachine.AddState("CloseState");

            AnimationClip openClip = CreateClip(openClipPath);
            AnimationClip closeClip = CreateClip(closeClipPath);
            openState.motion = openClip;
            closeState.motion = closeClip;

            AnimatorStateTransition anyStateToOpen = stateMachine.AddAnyStateTransition(openState);
            anyStateToOpen.AddCondition(AnimatorConditionMode.If, 0, OpenTrigger);
            anyStateToOpen.duration = 0f;
            anyStateToOpen.hasExitTime = false;

            AnimatorStateTransition anyStateToClose = stateMachine.AddAnyStateTransition(closeState);
            anyStateToClose.AddCondition(AnimatorConditionMode.If, 0, CloseTrigger);
            anyStateToClose.duration = 0f;
            anyStateToClose.hasExitTime = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            MLog.Default?.D($"Generated {Path.GetFileName(controllerPath)}, {Path.GetFileName(openClipPath)}, {Path.GetFileName(closeClipPath)}.");
            return controller;
        }

        internal static AnimatorController CreateForBehaviour(UIViewBehaviour behaviour)
        {
            if (behaviour == null) return null;

            string prefabPath = UIPanelUtility.GetPrefabPath(behaviour);
            if (string.IsNullOrEmpty(prefabPath))
            {
                MLog.Default?.W("Create UI AnimatorController requires a prefab asset or prefab instance.");
                return null;
            }

            string objectName = Path.GetFileNameWithoutExtension(prefabPath);
            string directoryPath = Path.GetDirectoryName(prefabPath)?.Replace('\\', '/');
            string controllerPath = $"{directoryPath}/{objectName}.controller";
            string openClipPath = $"{directoryPath}/{objectName}_Open.anim";
            string closeClipPath = $"{directoryPath}/{objectName}_Close.anim";

            AnimatorController controller = CreateAnimatorController(objectName, controllerPath, openClipPath, closeClipPath);
            Animator animator = behaviour.GetComponent<Animator>();
            if (animator == null)
            {
                animator = behaviour.gameObject.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = controller;
            EditorUtility.SetDirty(behaviour.gameObject);
            return controller;
        }

        private static AnimationClip CreateClip(string path)
        {
            AnimationClip clip = new()
            {
                frameRate = 60f,
            };

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        private static bool TryGetSelectedUIPrefab(out GameObject prefab, out string prefabPath)
        {
            prefab = null;
            prefabPath = null;

            Object[] selectedObjects = Selection.objects;
            if (selectedObjects.Length != 1)
            {
                MLog.Default?.W("Select exactly one prefab asset.");
                return false;
            }

            prefab = selectedObjects[0] as GameObject;
            if (prefab == null || !PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                MLog.Default?.W("Selected asset is not a prefab.");
                return false;
            }

            if (!HasUIBehaviour(prefab))
            {
                MLog.Default?.W($"{prefab.name} has no UIPanelBehaviour or UIWidgetBehaviour.");
                return false;
            }

            prefabPath = AssetDatabase.GetAssetPath(prefab);
            return !string.IsNullOrEmpty(prefabPath);
        }

        private static bool HasUIBehaviour(GameObject gameObject)
        {
            return gameObject.GetComponent<UIPanelBehaviour>() != null ||
                   gameObject.GetComponent<UIWidgetBehaviour>() != null;
        }

        private static void AssignControllerToPrefab(string prefabPath, AnimatorController controller)
        {
            if (controller == null) return;

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                Animator animator = prefabRoot.GetComponent<Animator>();
                if (animator == null)
                {
                    animator = prefabRoot.AddComponent<Animator>();
                }

                animator.runtimeAnimatorController = controller;
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static void DeleteAssetIfExists(string path)
        {
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}
