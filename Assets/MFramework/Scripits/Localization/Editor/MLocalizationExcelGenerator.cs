using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MFramework.UI;

namespace MFramework
{
    public class MLocalizationExcelGenerator : EditorWindow
    {
        private bool FirstOpen = true;
        private List<LocalizationTableInfo> infos;

        [MenuItem("MFramework/MLocalizationExcelGenerator", priority = 102)]
        public static void Init()
        {
            EditorWindow window = GetWindow<MLocalizationExcelGenerator>(true, "MLocalizationExcelGenerator", false);
            window.minSize = new Vector2(500, 200);
            window.Show();
        }

        private void OnGUI()
        {
            DrawCreateBtn();

            MGUIUtility.DrawH2("查询场景中所有的MLocalization");

            DrawResetSortBtn();
            DrawAutoSortBtn();
            DrawMLocalizationChecker();
        }

        private void OnDestroy()
        {
            FirstOpen = true;
        }

        private void DrawAutoSortBtn()
        {
            if (GUILayout.Button("一键排序"))
            {

            }
        }
        private void DrawResetSortBtn()
        {
            if (GUILayout.Button("重置排序"))
            {
                infos = GetMLocalizationTabelInfo(true); 
                GUI.FocusControl(null);
                return;
            }
        }
        private void DrawCreateBtn()
        {
            if (GUILayout.Button("创建Excel文件"))
            {
                string fileName = "LocalizationTable.xlsx";
                string filePath = Path.Combine(EditorSettings.excelGenerationPath, fileName);

                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    int flag = EditorUtility.DisplayDialogComplex("Generating",
                        "本地化Excel文件已存在，是否需要进行何种操作", "覆盖", "取消", "更新");
                    if (flag == 1) return;//取消
                    else if (flag == 0)//覆盖---删除重创
                    {
                        file.Delete();
                        file = new FileInfo(filePath);
                    }
                    else if (flag == 2)//更新
                    {
                        //TODO:未完成
                        MLog.Print("TODO", MLogType.Error);
                    }
                }

                //寻找所有MLocalization脚本
                List<LocalizationTableInfo> infos = GetMLocalizationTabelInfo(true);

                //创建Excel文件
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//创建表1
                    worksheet.Cells["A1"].LoadFromDataTable(GetLocalizationTable(infos), true);//创建初始表内容

                    int row = infos.Count + 3;
                    worksheet.Cells[$"A1:E{row}"].AutoFitColumns();//调整行宽
                    worksheet.Cells[$"A1:E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//居中
                    worksheet.Cells["A1:E3"].Style.Font.Bold = true;//加粗

                    package.Save();
                }

                System.Diagnostics.Process.Start(filePath);

                return;
            }
        }

        private DataTable GetLocalizationTable(List<LocalizationTableInfo> infos)
        {
            DataTable table = new DataTable();

            //必须先创列，才能添加行
            table.Columns.Add("编号");
            table.Columns.Add("物体名");
            table.Columns.Add("描述");
            table.Columns.Add("中文");
            table.Columns.Add("英文");
            table.Rows.Add(new object[] { "ID", "GOName", "Desc", "Chinese", "English" });
            table.Rows.Add(new object[] { "int", "none", "none", "string", "string" });
            foreach (var info in infos)
            {
                table.Rows.Add(new object[] { info.id, info.desc, "", "", "" });
            }

            return table;
        }

        private void DrawMLocalizationChecker()
        {
            List<MLocalization> mLocalList = MLocalizationUtility.FindAllLoclizations();

            if (FirstOpen)
            {
                infos = GetMLocalizationTabelInfo(true);
                FirstOpen = false;
            }
            foreach (var info in infos)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    //添加换行功能
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.wordWrap = true;

                    EditorGUILayout.LabelField($"物体名：{info.desc}\n文字：{info.text}", style);
                    int oldID = info.id;
                    int newID = EditorGUILayout.IntField(oldID);
                    if (newID != oldID)
                    {
                        info.id = newID;
                        info.mLocal.LocalID = newID;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);
            }
        }

        /// <summary>
        /// 获取排序并筛选后的MLocalization信息
        /// </summary>
        /// <returns></returns>
        private List<LocalizationTableInfo> GetMLocalizationTabelInfo(bool isSort)
        {
            List<LocalizationTableInfo> infos = new List<LocalizationTableInfo>();
            List<MLocalization> mLocalList = MLocalizationUtility.FindAllLoclizations();
            foreach (var mLocal in mLocalList)
            {
                if (mLocal.LocalMode == LocalizationMode.Off || mLocal.LocalID == -1) continue;

                LocalizationTableInfo info = new LocalizationTableInfo(mLocal, mLocal.LocalID, mLocal.gameObject.name, mLocal.GetComponent<MText>().text);
                infos.Add(info);
            }

            if(isSort) infos = infos.OrderBy(info => info.id).ToList();//排序

            return infos;
        }

        private class LocalizationTableInfo
        {
            public MLocalization mLocal;
            public int id;
            public string desc;
            public string text;

            public LocalizationTableInfo(MLocalization mLocal, int id, string desc, string text)
            {
                this.mLocal = mLocal;
                this.id = id;
                this.desc = desc;
                this.text = text;
            }
        }
    }
}
