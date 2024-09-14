using System;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(MTextAnimator))]
    public class MTextAnimatorEditor : Editor
    {
        private bool showFunState;

        private SerializedProperty typeWriterSwitchSP;
        private SerializedProperty typewriterTypeSP;
        private SerializedProperty typewriterStartDoSP;
        private SerializedProperty typeSpeedSP;
        private SerializedProperty onTypeWriterFinishedSP;

        private SerializedProperty inlineEffectsSwitchSP;
        private SerializedProperty inlineEffectsAutoDoSP;

        private static GUIContent typeWriterSwitchLabel = new GUIContent("TypeWriter Switch", "是否开启打字机效果");
        private static GUIContent typewriterTypeLabel = new GUIContent("Typewriter Type", "打字机类型");
        private static GUIContent typewriterAutoDoLabel = new GUIContent("Is StartDo", "是否启动执行");
        private static GUIContent typeSpeedLabel = new GUIContent("Type Speed", "打字机速度");
        
        private static GUIContent inlineEffectsSwitchLabel = new GUIContent("InlineEffects Switch", "是否开启内联动画效果");
        private static GUIContent inlineEffectsAutoDoLabel = new GUIContent("Is AutoDo", "是否打字机效果结束后自动执行");

        private const string FUNSTR =
@"基础格式:<内联函数>...</>     如：I'm <exclaim>so good</>
注意：无参形式支持带括号与不带括号，如：<fadein>|<fadein()>

颜色Color：
    color(speed,r,g,b)
    color(r,g,b)
缩放Scale:
    scale(scaleFactor,speed)
    scale(scaleFactor)
旋转Rotate:   Tip：degree>0为顺时针旋转
    rotate(degree,speed)
    rotate(degree)
渐入FadeIn:
    fadein(speed)
    fadein()
渐出FadeOut:
    fadeout(speed)
    fadeout()
彩色渐变ColorFade:  固定周期逐字变色
    colorfade(speed,r,g,b)
    colorfade(r,g,b)
震动Shake:    在一定的范围内不断随机移动
    定时：
    shake(duration,amplitude,frequency)
    shake(duration)
    无限：
    shake(amplitude,frequency)
    shake()
倾斜Slant:    像画的上侧钉子掉了1个导致的倾斜
    slant(speed,degree)
    slant(speed)
    slant()
闪烁Twinkle:  以一定的速度不断渐入渐出
    twinkle(speed)
    twinkle()
尖叫Exclaim:  具有更多效果的震动Shake(变色+加粗)
    定时：
    exclaim(speed,duration,scaleFactor,r,g,b)
    exclaim(speed,duration)
    无限：
    exclaim(speed,scaleFactor,r,g,b)
    exclaim(speed)
    exclaim()
无限波动WaveLoop:   无限上下起伏
    wave(speed, amplitude, loopIntervalFactor)
    wave(speed)
    wave()";

        protected void OnEnable()
        {
            showFunState = false;

            typeWriterSwitchSP = serializedObject.FindProperty("typeWriterSwitch");
            typewriterTypeSP = serializedObject.FindProperty("typewriterType");
            typewriterStartDoSP = serializedObject.FindProperty("typewriterStartDo");
            typeSpeedSP = serializedObject.FindProperty("typeSpeed");
            onTypeWriterFinishedSP = serializedObject.FindProperty("onTypeWriterFinished");

            inlineEffectsSwitchSP = serializedObject.FindProperty("inlineEffectsSwitch");
            inlineEffectsAutoDoSP = serializedObject.FindProperty("inlineEffectsAutoDo");

            if (typeSpeedSP.floatValue == default(float)) typeSpeedSP.floatValue = 1.0f;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTypeWriter();

            EditorGUILayout.Space(10);

            DrawTextAnimator();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTypeWriter()
        {
            var typeWriterSwitch = MEditorControlUtility.DrawPopup<TypeWriterSwitch>(typeWriterSwitchSP, typeWriterSwitchLabel);

            if (typeWriterSwitch == TypeWriterSwitch.Off) return;
            else//开启打字机效果
            {
                EditorGUI.indentLevel++;
                {
                    MEditorControlUtility.DrawPopup<MTextTypewriterType>(typewriterTypeSP, typewriterTypeLabel);
                    MEditorControlUtility.DrawToggle(typewriterStartDoSP, typewriterAutoDoLabel);

                    var typeSpeedFloat = MEditorControlUtility.DrawFloat(typeSpeedSP, typeSpeedLabel);
                    typeSpeedSP.floatValue = Mathf.Clamp(typeSpeedFloat, 0, float.MaxValue);

                    MEditorControlUtility.DrawProperty(onTypeWriterFinishedSP);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawTextAnimator()
        {
            var inlineEffectSwitch = MEditorControlUtility.DrawPopup<InlineEffectSwitch>(inlineEffectsSwitchSP, inlineEffectsSwitchLabel);

            if (inlineEffectSwitch == InlineEffectSwitch.Off) return;
            else//开启内联效果
            {
                EditorGUI.indentLevel++;
                {
                    MEditorControlUtility.DrawToggle(inlineEffectsAutoDoSP, inlineEffectsAutoDoLabel);
                }
                EditorGUI.indentLevel--;
            }

            string btnStr = showFunState ? "隐藏内联函数声明" : "显示内联函数声明";
            if (GUILayout.Button(btnStr))
            {
                showFunState = !showFunState;
                return;
            }
            if (showFunState)
            {
                EditorGUILayout.HelpBox(FUNSTR, MessageType.None);
            }
        }
    }
}