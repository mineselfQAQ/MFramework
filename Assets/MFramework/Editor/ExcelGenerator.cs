using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class ExcelGenerator : EditorWindow
    {
        [MenuItem("MFramework/ExcelGenerator")]
        public static void Init()
        {
            ExcelGenerator window = GetWindow<ExcelGenerator>(true, "ExcelGenerator", false);
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("更改Excel存储路径"))
            {
                EditorSettingsController.ChangePath(EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath));
                Log.Print($"已更改路径{EditorSettings.excelGenerationPath}.");
            }

            if (GUILayout.Button("生成Excel文件"))
            {
                int state = EditorUtility.DisplayDialogComplex("Generating", 
                    $"确定文件将生成在{EditorSettings.excelGenerationPath}处吗？", "确认", "取消", "更改路径");
                if (state == 0)
                {
                    Log.Print("已完成生成.");
                }
                else if (state == 1)
                {
                    Log.Print("已取消生成.", MLogType.Warning);
                }
                else
                {
                    EditorSettingsController.ChangePath(EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath));
                    Log.Print($"已更改路径.");
                } 
            }
        }
    }
}
