using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    internal static class MEditorUtility
    {
        //显示窗口并等待返回选择内容
        internal static int state;
        internal static async Task<int> DisplayDialogAsync(string title, string message, params string[] names)
        {
            state = -100;
            await ShowDialogAsync(title, message, names);
            return state;
        }
        private static async Task ShowDialogAsync(string title, string message, params string[] names)
        {
            MDialog.ShowDialog(title, message, (i) => { state = i; }, names);

            await Task.Run(() =>
            {
                while (state == -100)
                {
                    continue;
                }
            });
        }

        internal static Texture GetIcon(Type type)
        {
            Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;
            Texture customIcon = null;
            Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            if (type == typeof(TMPro.TMP_InputField))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Input Field Icon.psd");
            }
            else if (type == typeof(TMPro.TMP_Dropdown))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd");
            }
            else if (type == typeof(TMPro.TextMeshProUGUI))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Text Component Icon.psd");
            }

            return systemIcon ?? customIcon ?? csScriptIcon;
        }

        internal static string ChangePath()
        {
            string guideFolder = Path.GetDirectoryName(Application.dataPath);
            string newPath = EditorUtility.OpenFolderPanel("请选择路径", guideFolder, "");

            if (newPath == "")
            {
                return null;
            }
            return newPath;
        }

        internal static string DisplayGenerateFileDialog(string oldPath, string fileName = "")
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{oldPath}处吗？", "确认", "取消", "更改路径");

            if (state == 0)//确认
            {
                return oldPath;
            }
            else if (state == 1)//取消
            {
                MLog.Print($"已取消生成{fileName}文件.", MLogType.Warning);
                return null;
            }
            else//更改路径
            {
                string newPath = ChangePath();
                newPath = Path.Combine(newPath, fileName);
                return newPath;
            }
        }
    }
}