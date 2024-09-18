using UnityEditor.Callbacks;
using MFramework;
using UnityEngine;
using UnityEditor;

public class BuildABWhenBuild
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        EditorDelayExecute.Instance.DelayDo(() =>
        {
            Builder.Build(pathToBuiltProject);
        });
    }
}
