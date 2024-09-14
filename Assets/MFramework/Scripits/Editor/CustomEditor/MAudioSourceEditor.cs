using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(MAudioSource))]
    public class MAudioSourceEditor : Editor
    {
        private SerializedProperty OnStartSP;
        private SerializedProperty modeSP;
        private SerializedProperty audioClipSP;
        private SerializedProperty audioMixerGroupSP;
        private SerializedProperty muteSP;
        private SerializedProperty playOnAwakeSP;
        private SerializedProperty loopSP;
        private SerializedProperty fadeInOutSP;
        private SerializedProperty fadeInTimeSP;
        private SerializedProperty fadeOutTimeSP;

        private SerializedProperty prioritySP;
        private SerializedProperty volumeSP;
        private SerializedProperty pitchSP;

        private static GUIContent modeLabel = new GUIContent("Mode", "AudioSource类型");

        private static GUIContent muteLabel = new GUIContent("Mute", "是否静音");
        private static GUIContent playOnAwakeLabel = new GUIContent("Play On Awake", "开启时自动播放");
        private static GUIContent loopLabel = new GUIContent("Loop", "是否循环播放");
        private static GUIContent fadeInoutLabel = new GUIContent("Enable Fade", "开启渐入渐出");
        private static GUIContent fadeInTimeLabel = new GUIContent("FadeIn Time", "渐入时长");
        private static GUIContent fadeOutTimeLabel = new GUIContent("FadeOut Time", "渐出时长");

        private static GUIContent priorityLabel = new GUIContent("Priority", "初始顺位");
        private static GUIContent volumeLabel = new GUIContent("Volume", "初始音量");
        private static GUIContent pitchLabel = new GUIContent("Pitch", "初始音调");

        protected void OnEnable()
        {
            OnStartSP = serializedObject.FindProperty("OnStart");
            modeSP = serializedObject.FindProperty("mode");
            audioClipSP = serializedObject.FindProperty("audioClip");
            audioMixerGroupSP = serializedObject.FindProperty("audioMixerGroup");
            muteSP = serializedObject.FindProperty("mute");
            playOnAwakeSP = serializedObject.FindProperty("playOnAwake");
            loopSP = serializedObject.FindProperty("loop");
            fadeInOutSP = serializedObject.FindProperty("fadeInOut");
            fadeInTimeSP = serializedObject.FindProperty("fadeInTime");
            fadeOutTimeSP = serializedObject.FindProperty("fadeOutTime");
            prioritySP = serializedObject.FindProperty("priority");
            volumeSP = serializedObject.FindProperty("volume");
            pitchSP = serializedObject.FindProperty("pitch");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Draw();

            serializedObject.ApplyModifiedProperties();
        }

        private void Draw()
        {
            MGUIUtility.DrawLeftH2("基础设置");

            MEditorControlUtility.DrawProperty(OnStartSP);
            MEditorControlUtility.DrawProperty(audioClipSP);
            MEditorControlUtility.DrawProperty(audioMixerGroupSP);

            EditorGUILayout.Space(5);

            MEditorControlUtility.DrawIntSlider(prioritySP, 0, 256, priorityLabel);
            MEditorControlUtility.DrawSlider(volumeSP, 0, 1, volumeLabel);
            MEditorControlUtility.DrawSlider(pitchSP, 0, 3, pitchLabel);

            EditorGUILayout.Space(5);
            MGUIUtility.DrawLeftH2("专项设置");

            var mode = MEditorControlUtility.DrawPopup<AudioSourceMode>(modeSP, modeLabel);

            EditorGUI.indentLevel++;
            {
                if (mode == AudioSourceMode.Music)
                {
                    MEditorControlUtility.SetToggle(false, muteSP, muteLabel);
                    MEditorControlUtility.SetToggle(true, playOnAwakeSP, playOnAwakeLabel);
                    MEditorControlUtility.SetToggle(true, loopSP, loopLabel);

                    EditorGUILayout.Space(5);

                    MEditorControlUtility.SetToggle(true, fadeInOutSP, fadeInoutLabel);
                    EditorGUI.indentLevel++;
                    {
                        MEditorControlUtility.DrawFloat(fadeInTimeSP, fadeInTimeLabel);
                        MEditorControlUtility.DrawFloat(fadeOutTimeSP, fadeOutTimeLabel);
                    }
                    EditorGUI.indentLevel--;
                }
                else if (mode == AudioSourceMode.SFX)
                {
                    MEditorControlUtility.SetToggle(false, muteSP, muteLabel);
                    MEditorControlUtility.SetToggle(false, playOnAwakeSP, playOnAwakeLabel);
                    MEditorControlUtility.SetToggle(false, loopSP, loopLabel);

                    EditorGUILayout.Space(5);

                    MEditorControlUtility.SetToggle(false, fadeInOutSP, fadeInoutLabel);
                }
                else if (mode == AudioSourceMode.Custom)
                {
                    MEditorControlUtility.DrawToggle(muteSP, muteLabel);
                    MEditorControlUtility.DrawToggle(playOnAwakeSP, playOnAwakeLabel);
                    MEditorControlUtility.DrawToggle(loopSP, loopLabel);

                    EditorGUILayout.Space(5);

                    bool fadeState = MEditorControlUtility.DrawToggle(fadeInOutSP, fadeInoutLabel);

                    if (fadeState)
                    {
                        EditorGUI.indentLevel++;
                        {
                            MEditorControlUtility.DrawFloat(fadeInTimeSP, fadeInTimeLabel);
                            MEditorControlUtility.DrawFloat(fadeOutTimeSP, fadeOutTimeLabel);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}
