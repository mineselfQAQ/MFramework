using UnityEditor.Callbacks;
using MFramework;
using UnityEngine;
using UnityEditor;

public class BuildABWhenBuild
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        //???
#elif UNITY_ANDROID
        //???
#else
        EditorDelayExecute.Instance.DelayDo(() =>
        {
            Builder.Build_Windows(pathToBuiltProject);
        });
#endif
    }
}