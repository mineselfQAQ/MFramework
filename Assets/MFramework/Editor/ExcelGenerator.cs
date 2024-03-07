using OfficeOpenXml;
using System;
using System.Data;
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
            //标题
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Excel编辑器", GUIStyleUtility.TitleStyle);
            EditorGUILayout.Space(5);

            //if (GUILayout.Button("更改Excel存储路径"))
            //{
            //    EditorSettingsController.ChangePath(EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath));
            //    Log.Print($"已更改路径{EditorSettings.excelGenerationPath}.");
            //}

            if (GUILayout.Button("生成Excel文件"))
            {
                int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{EditorSettings.excelGenerationPath}处吗？", "确认", "取消", "更改路径");
                if (state == 0)
                {
                    string path = EditorUtility.SaveFilePanel("保存", EditorSettings.excelGenerationPath, "Excel", "xlsx");
                    path = path.Replace("/", "\\");

                    if (path != null)
                    {
                        FileInfo file = new FileInfo(path);

                        if (file.Exists)
                        {
                            file.Delete();
                            file = new FileInfo(path);

                            string fileName = Path.GetFileName(path);
                            Log.Print(Log.BoldWord($"已重新生成{fileName}."), MLogType.Warning);
                        }

                        using (ExcelPackage package = new ExcelPackage(file))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                            worksheet.Cells["A1"].LoadFromDataTable(GetDefaultTable(), true);
                            worksheet.Cells["A1:C4"].AutoFitColumns();

                            package.Save();
                        }
                    }
                    System.Diagnostics.Process.Start($"Explorer.exe", $@"/select,{path}");
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

        private DataTable GetDefaultTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("编号ID|int", typeof(Int32));
            table.Columns.Add("名字Name|string", typeof(String));
            table.Columns.Add("描述DESC|string[]", typeof(String));
            table.Rows.Add(new object[] { 1      , "苹果"     , "红色#甜" });
            table.Rows.Add(new object[] { 2      , "香蕉"     , "黄色#甜" });
            table.Rows.Add(new object[] { 3      , "橘子"     , "橙色#酸" });

            return table;
        }
    }
}
