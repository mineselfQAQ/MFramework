using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(UIWidgetBehaviour))]
    public class UIWidgetBehaviourEditor : UIViewBehaviourEditor
    {
        private UIWidgetBehaviour behaviour;

        private SerializedProperty widgetModeSP;
        private SerializedProperty animSwitchSP;
        private SerializedProperty openAnimModeSP;
        private SerializedProperty closeAnimModeSP;

        private static GUIContent widgetModeLabel = new GUIContent("Widget Mode", "组件模式");
        private static GUIContent animationSwitchLabel = new GUIContent("Animation Switch", "动画开关");
        private static GUIContent openAnimationModeLabel = new GUIContent("Open Animation Mode", "开启动画模式");
        private static GUIContent closeAnimationModeLabel = new GUIContent("Close Animation Mode", "关闭动画模式");
        
        protected void OnEnable()
        {
            behaviour = (UIWidgetBehaviour)target;

            widgetModeSP = serializedObject.FindProperty("widgetMode");
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
            var widgetMode = MEditorControlUtility.DrawPopup<UIWidgetMode>(widgetModeSP, widgetModeLabel);
            if (widgetMode == UIWidgetMode.Simple) return;//不开启高级模式
            else//高级模式
            {
                EditorGUI.indentLevel++;
                {
                    int lastAnimSwitch = animSwitchSP.enumValueIndex;

                    var animSwitch = MEditorControlUtility.DrawPopup<UIAnimSwitch>(animSwitchSP, animationSwitchLabel);
                    if (animSwitch ==  UIAnimSwitch.Off) return;//不开启动画
                    else//开启动画
                    {
                        //如果没有Animator组件，就自动生成所有相关内容
                        Animator checkAnimator = behaviour.GetComponent<Animator>();
                        if (checkAnimator == null)
                        {
                            bool state = EditorUtility.DisplayDialog("提示",
                            "启用动画功能需要Animator的支持，是否需要创建", "仅Animator", "不启用动画");
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
                            //    EditorDelayExecute.Instance.DelayDo(DelayNoRecord());

                            //    IEnumerator DelayNoRecord()
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
                            MEditorControlUtility.DrawPopup<UIOpenAnimMode>(openAnimModeSP, openAnimationModeLabel);
                            MEditorControlUtility.DrawPopup<UICloseAnimMode>(closeAnimModeSP, closeAnimationModeLabel);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}