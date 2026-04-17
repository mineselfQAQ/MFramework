#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework.Core
{
    /// <summary>
    /// 在 Unity 菜单中直接打开根目录下的 VS Code 工作区文件。
    /// </summary>
    public static class OpenWorkspaceMenu
    {
        private const string MenuPath = "MFramework/打开 MFramework.code-workspace";
        private const string WorkspaceFileName = "MFramework.code-workspace";

        [MenuItem(MenuPath, false, 1)]
        private static void OpenWorkspace()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string workspacePath = Path.Combine(projectRoot, WorkspaceFileName);

            if (!File.Exists(workspacePath))
            {
                EditorUtility.DisplayDialog(
                    "打开工作区失败",
                    $"未找到工作区文件：\n{workspacePath}",
                    "确定");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = workspacePath,
                    WorkingDirectory = projectRoot,
                    UseShellExecute = true,
                });
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError($"打开工作区失败：{workspacePath}\n{exception}");
                EditorUtility.DisplayDialog(
                    "打开工作区失败",
                    "系统没有正确关联 .code-workspace 文件，或 VS Code 未安装完成。",
                    "确定");
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateOpenWorkspace()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string workspacePath = Path.Combine(projectRoot, WorkspaceFileName);
            return File.Exists(workspacePath);
        }
    }
}
#endif
