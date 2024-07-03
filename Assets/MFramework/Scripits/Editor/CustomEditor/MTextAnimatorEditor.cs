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

        private static GUIContent typeWriterSwitchLabel = new GUIContent("TypeWriter Switch", "�Ƿ������ֻ�Ч��");
        private static GUIContent typewriterTypeLabel = new GUIContent("Typewriter Type", "���ֻ�����");
        private static GUIContent typewriterAutoDoLabel = new GUIContent("Is StartDo", "�Ƿ�����ִ��");
        private static GUIContent typeSpeedLabel = new GUIContent("Type Speed", "���ֻ��ٶ�");
        
        private static GUIContent inlineEffectsSwitchLabel = new GUIContent("InlineEffects Switch", "�Ƿ�����������Ч��");
        private static GUIContent inlineEffectsAutoDoLabel = new GUIContent("Is AutoDo", "�Ƿ���ֻ�Ч���������Զ�ִ��");

        private const string FUNSTR =
@"������ʽ:<��������>...</>     �磺I'm <exclaim>so good</>
ע�⣺�޲���ʽ֧�ִ������벻�����ţ��磺<fadein>|<fadein()>

��ɫColor��
    color(speed,r,g,b)
    color(r,g,b)
����Scale:
    scale(scaleFactor,speed)
    scale(scaleFactor)
��תRotate:   Tip��degree>0Ϊ˳ʱ����ת
    rotate(degree,speed)
    rotate(degree)
����FadeIn:
    fadein(speed)
    fadein()
����FadeOut:
    fadeout(speed)
    fadeout()
��ɫ����ColorFade:  �̶��������ֱ�ɫ
    colorfade(speed,r,g,b)
    colorfade(r,g,b)
��Shake:    ��һ���ķ�Χ�ڲ�������ƶ�
    ��ʱ��
    shake(duration,amplitude,frequency)
    shake(duration)
    ���ޣ�
    shake(amplitude,frequency)
    shake()
��бSlant:    �񻭵��ϲඤ�ӵ���1�����µ���б
    slant(speed,degree)
    slant(speed)
    slant()
��˸Twinkle:  ��һ�����ٶȲ��Ͻ��뽥��
    twinkle(speed)
    twinkle()
���Exclaim:  ���и���Ч������Shake(��ɫ+�Ӵ�)
    ��ʱ��
    exclaim(speed,duration,scaleFactor,r,g,b)
    exclaim(speed,duration)
    ���ޣ�
    exclaim(speed,scaleFactor,r,g,b)
    exclaim(speed)
    exclaim()
���޲���WaveLoop:   �����������
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
            Enum typeWriterSwitchEnum = EditorGUILayout.EnumPopup(typeWriterSwitchLabel, (TypeWriterSwitch)typeWriterSwitchSP.enumValueIndex);
            TypeWriterSwitch typeWriterSwitch = (TypeWriterSwitch)typeWriterSwitchEnum;
            typeWriterSwitchSP.enumValueIndex = (int)typeWriterSwitch;

            if (typeWriterSwitch == 0) return;
            else//�������ֻ�Ч��
            {
                EditorGUI.indentLevel++;

                Enum typewriterTypeEnum = EditorGUILayout.EnumPopup(typewriterTypeLabel, (MTextTypewriterType)typewriterTypeSP.enumValueIndex);
                MTextTypewriterType typewriterType = (MTextTypewriterType)typewriterTypeEnum;
                typewriterTypeSP.enumValueIndex = (int)typewriterType;

                bool typewriterStartDoBool = EditorGUILayout.Toggle(typewriterAutoDoLabel, typewriterStartDoSP.boolValue);
                typewriterStartDoSP.boolValue = typewriterStartDoBool;

                float typeSpeedFloat = EditorGUILayout.FloatField(typeSpeedLabel, typeSpeedSP.floatValue);
                typeSpeedSP.floatValue = Mathf.Clamp(typeSpeedFloat, 0, float.MaxValue);

                EditorGUILayout.PropertyField(onTypeWriterFinishedSP);

                EditorGUI.indentLevel--;
            }
        }

        private void DrawTextAnimator()
        {
            Enum inlineEffectsSwitchEnum = EditorGUILayout.EnumPopup(inlineEffectsSwitchLabel, (InlineEffectSwitch)inlineEffectsSwitchSP.enumValueIndex);
            InlineEffectSwitch InlineEffectSwitch = (InlineEffectSwitch)inlineEffectsSwitchEnum;
            inlineEffectsSwitchSP.enumValueIndex = (int)InlineEffectSwitch;

            if (InlineEffectSwitch == 0) return;
            else//��������Ч��
            {
                EditorGUI.indentLevel++;

                bool inlineEffectsAutoDoBool = EditorGUILayout.Toggle(inlineEffectsAutoDoLabel, inlineEffectsAutoDoSP.boolValue);
                inlineEffectsAutoDoSP.boolValue = inlineEffectsAutoDoBool;

                EditorGUI.indentLevel--;
            }

            string btnStr = showFunState ? "����������������" : "��ʾ������������";
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