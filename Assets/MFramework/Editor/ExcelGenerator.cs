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
            if (GUILayout.Button("生成Excel文件"))
            {
                string fullExcelGenerationPath = GetFullExcelGenerationPath(EditorSettings.Instance.excelGenerationPath);

                bool isContinue = EditorUtility.DisplayDialog
                    ("Generating", $"确定文件将生成在{fullExcelGenerationPath}处吗？", "YES", "Cancel");
                if (isContinue) Log.Print("已完成生成.");
                else Log.Print("已取消生成.");
            }
        }

        private string GetFullExcelGenerationPath(string secondPath)
        {
            string fullPath = Path.GetFullPath(secondPath);
            return fullPath;
        }

    }
}
