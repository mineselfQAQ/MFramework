using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MFramework
{
    public static class CreateUIAnimatorController
    {
        [MenuItem("Assets/MCreate/UIAnimatorController", false, priority = 9, secondaryPriority = 1.0f)]
        public static void Create()
        {
            if (CheckAvailability())
            {
                var objName = Selection.objects[0].name;

                //创建.controller文件
                var guids = Selection.assetGUIDs;
                string directoryPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[0]));
                string acPath = Path.Combine(directoryPath, $"{objName}.controller");
                string openClipPath = Path.Combine(directoryPath, $"{objName}_Open.anim");
                string closeClipPath = Path.Combine(directoryPath, $"{objName}_Close.anim");
                if (File.Exists(acPath) || File.Exists(openClipPath) || File.Exists(closeClipPath))
                {
                    MLog.Print($"{objName}中已存在.controller或.anim文件，如需重新创建，请全部删除后再次点击", MLogType.Warning);
                    return;
                } 

                var controller = AnimatorController.CreateAnimatorControllerAtPath(acPath);

                //添加参数
                controller.AddParameter("Open", AnimatorControllerParameterType.Trigger);
                controller.AddParameter("Close", AnimatorControllerParameterType.Trigger);

                //添加状态
                var rootStateMachine = controller.layers[0].stateMachine;
                var defaultState = rootStateMachine.AddState("DefaultState");
                rootStateMachine.defaultState = defaultState;
                var openState = rootStateMachine.AddState("OpenState");
                var closeState = rootStateMachine.AddState("CloseState");

                //添加Clip
                AnimationClip openClip = new AnimationClip();
                AnimationUtility.GetAnimationClipSettings(openClip).loopTime = false;
                AssetDatabase.CreateAsset(openClip, openClipPath);
                openState.motion = openClip;

                AnimationClip closeClip = new AnimationClip();
                AnimationUtility.GetAnimationClipSettings(closeClip).loopTime = false;
                AssetDatabase.CreateAsset(closeClip, closeClipPath);
                closeState.motion = closeClip;

                //添加过渡
                var defaultToOpen = defaultState.AddTransition(openState);
                defaultToOpen.AddCondition(AnimatorConditionMode.If, 0, "Open");
                defaultToOpen.duration = 0;

                var openToClose = openState.AddTransition(closeState);
                openToClose.AddCondition(AnimatorConditionMode.If, 0, "Close");
                openToClose.duration = 0;

                var closeToOpen = closeState.AddTransition(openState);
                closeToOpen.AddCondition(AnimatorConditionMode.If, 0, "Open");
                closeToOpen.duration = 0;

                //保存文件
                AssetDatabase.SaveAssets();
                MLog.Print($"已生成{objName}.controller文件");
            }
        }

        private static bool CheckAvailability()
        {
            var objs = Selection.objects;

            if (objs.Length != 1)
            {
                MLog.Print("请勿多选资源，请重试", MLogType.Warning);
                return false;
            }

            if (!PrefabUtility.IsPartOfAnyPrefab(objs[0]))
            {
                MLog.Print($"{objs[0].name}并非prefab，无法进行创建操作，请重试", MLogType.Warning);
                return false;
            }

            GameObject prefab = objs[0] as GameObject;
            bool flag = false;
            var comps = prefab.GetComponents<Component>();
            foreach (var comp in comps)
            {
                MonoScript monoScript = MonoScript.FromMonoBehaviour(comp as MonoBehaviour);
                if (monoScript != null && monoScript.name == "UIPanelBehaviour")
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                MLog.Print($"{objs[0].name}中没有UIPanelBehaviour脚本，无法进行创建操作，请重试", MLogType.Warning);
                return false;
            }

            return true;
        }
    }
}