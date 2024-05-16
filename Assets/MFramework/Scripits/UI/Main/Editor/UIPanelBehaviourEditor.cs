using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(UIPanelBehaviour))]
    public class UIPanelBehaviourEditor : UIViewBehaviourEditor
    {
        //TODO:在AnimSwitch切换至Enabled时，需要开启Open/Close Anim Mode且生成相应内容(在Prefab中添加Animator并生成相应文件)

        private UIPanelBehaviour behaviour;

        private SerializedProperty thicknessSP;
        private SerializedProperty focusModeSP;
        private SerializedProperty animSwitchSP;
        private SerializedProperty openAnimModeSP;
        private SerializedProperty closeAnimModeSP;

        private int lastAnimSwitch;

        protected void OnEnable()
        {
            behaviour = (UIPanelBehaviour)target;

            thicknessSP = serializedObject.FindProperty("thickness");
            focusModeSP = serializedObject.FindProperty("focusMode");
            animSwitchSP = serializedObject.FindProperty("animSwitch");
            openAnimModeSP = serializedObject.FindProperty("openAnimMode");
            closeAnimModeSP = serializedObject.FindProperty("closeAnimMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawUIPanelSettings();

            DrawCompCollections();

            DrawExportBtn();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUIPanelSettings()
        {
            int value = EditorGUILayout.IntField("Thickness", thicknessSP.intValue);
            thicknessSP.intValue = Mathf.Clamp(value, 1, int.MaxValue);

            Enum focusModeEnum = EditorGUILayout.EnumPopup("Focus Mode", (UIPanelFocusMode)focusModeSP.enumValueIndex);
            UIPanelFocusMode focusMode = (UIPanelFocusMode)focusModeEnum;
            focusModeSP.enumValueIndex = (int)focusMode;

            int lastAnimSwitch = animSwitchSP.enumValueIndex;
            Enum animSwitchEnum = EditorGUILayout.EnumPopup("Animation Switch", (UIPanelAnimSwitch)animSwitchSP.enumValueIndex);
            UIPanelAnimSwitch animSwitch = (UIPanelAnimSwitch)animSwitchEnum;
            animSwitchSP.enumValueIndex = (int)animSwitch;
            if (animSwitch == 0) return;//不开启动画
            else//开启动画
            {
                //如果没有Animator组件，就自动生成所有相关内容
                Animator checkAnimator = behaviour.GetComponent<Animator>();
                if (checkAnimator == null)
                {
                    bool state = EditorUtility.DisplayDialog("提示",
                    "启用动画功能需要Animator的支持，是否需要创建", "仅Animator", "全部");
                    if (!state)//取消
                    {
                        animSwitchSP.enumValueIndex = 0;
                        return;
                    }

                    Animator animator = behaviour.gameObject.AddComponent<Animator>();
                    //情况过于复杂，还是右键Prefab创建文件自行拖入比较合理
                    //if (state == 2)//全部生成
                    //{
                    //    string prefabPath = UIPanelUtility.GetPrefabPath(target);
                    //    string objName = Path.GetFileNameWithoutExtension(prefabPath);
                    //    string directoryPath = Path.GetDirectoryName(prefabPath);
                    //    string acPath = Path.Combine(directoryPath, $"{objName}.controller");
                    //    string openClipPath = Path.Combine(directoryPath, $"{objName}_Open.anim");
                    //    string closeClipPath = Path.Combine(directoryPath, $"{objName}_Close.anim");

                    //    //注意：由于会出现第二个弹窗，这会导致面板问题，需要延迟执行
                    //    EditorDelayExecute.Instance.DelayDo(Delay());

                    //    IEnumerator Delay()
                    //    {
                    //        yield return null;

                    //        var controller = CreateUIAnimatorController.CreateAnimatorController(objName, acPath, openClipPath, closeClipPath);
                    //        animator.runtimeAnimatorController = controller;
                    //    }
                    //}
                    MLog.Print("已创建Animator，请及时拖入AnimatorController，可使用MCreate--->UIAnimatorController自动创建", MLogType.Warning);
                    return;
                }
                if (checkAnimator.runtimeAnimatorController == null)
                {
                    //由Off切换至On的那一帧
                    if (lastAnimSwitch == 0 && animSwitchSP.enumValueIndex == 1)
                    {
                        MLog.Print("该Prefab上的Animator中没有Controller，无法使用动画系统，请自行挂载", MLogType.Warning);
                    }
                }

                EditorGUI.indentLevel++;
                {
                    Enum openAnimModeEnum = EditorGUILayout.EnumPopup("Open Animation Mode", (UIPanelOpenAnimMode)openAnimModeSP.enumValueIndex);
                    UIPanelOpenAnimMode openAnimMode = (UIPanelOpenAnimMode)openAnimModeEnum;
                    openAnimModeSP.enumValueIndex = (int)openAnimMode;

                    Enum closeAnimModeEnum = EditorGUILayout.EnumPopup("Close Animation Mode", (UIPanelCloseAnimMode)closeAnimModeSP.enumValueIndex);
                    UIPanelCloseAnimMode closeAnimMode = (UIPanelCloseAnimMode)closeAnimModeEnum;
                    closeAnimModeSP.enumValueIndex = (int)closeAnimMode;
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}