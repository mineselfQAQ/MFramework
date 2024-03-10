//using CsvHelper;
using Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
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
            window.minSize = new Vector2(300, 400);
            window.maxSize = new Vector2(300, 400);
            window.Show();
        }

        private void OnGUI()
        {
            //标题Excel编辑器
            MGUIUtility.DrawTitle(5, "Excel编辑器");

            //if (GUILayout.Button("更改Excel存储路径"))
            //{
            //    EditorSettingsController.ChangePath(EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath));
            //    Log.Print($"已更改路径{EditorSettings.excelGenerationPath}.");
            //}

            if (GUILayout.Button("生成Excel文件"))
            {
                CreateExcelFile();
            }

            EditorGUILayout.LabelField("------------------------------------------", MGUIStyleUtility.BoldStyle);

            if (GUILayout.Button("生成CSV临时文件(XLSX--->CSV)"))
            {
                XLSX2CSV();
            }

            EditorGUILayout.LabelField("------------------------------------------", MGUIStyleUtility.BoldStyle);

            if (GUILayout.Button("生成二进制文件(XLSX--->BIN)"))
            {
                XLSX2BIN();
            }
        }

        private void CreateExcelFile()
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{EditorSettings.excelGenerationPath}处吗？", "确认", "取消", "更改路径");
            if (state == 0)//确认
            {
                string path = EditorUtility.SaveFilePanel("保存", EditorSettings.excelGenerationPath, "Table", "xlsx");
                path = path.Replace("/", "\\");//Process.Start()只接受\而非/

                if (path == "")
                {
                    MLog.Print("已取消生成Excel文件.", MLogType.Warning);
                    return;
                }

                FileInfo file = new FileInfo(path);
                string fileName = Path.GetFileName(path);
                //如果文件已经存在，重新生成
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(path);

                    MLog.Print(MLog.BoldWord($"已重新生成{fileName}."), MLogType.Warning);
                }

                //创建Excel文件
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//创建表1
                    worksheet.Cells["A1"].LoadFromDataTable(GetDefaultTable(), true);//创建初始表内容

                    worksheet.Cells["A1:C6"].AutoFitColumns();//调整行宽
                    worksheet.Cells["A1:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//居中
                    worksheet.Cells["A1:C3"].Style.Font.Bold = true;//加粗
                    //worksheet.Cells["A1:A3"].Style.Numberformat = ???;

                    package.Save();
                }

                System.Diagnostics.Process.Start($"Explorer.exe", $@"/select,{path}");
                MLog.Print($"已生成{fileName}.");
            }
            else if (state == 1)//取消
            {
                MLog.Print("已取消生成Excel文件.", MLogType.Warning);
            }
            else//更改路径
            {
                string pathName = EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath);
                EditorSettingsController.ChangePath(pathName);
                MLog.Print($"已更改{pathName}路径.");
            }
        }

        private void XLSX2BIN()
        {
            string resourceFolder = $@"{Application.dataPath}\Resources";
            if (!Directory.Exists(resourceFolder))//保证Resources文件夹的创建
            {
                Directory.CreateDirectory(resourceFolder);
            }
            string BINFolder = $@"{resourceFolder}\ExcelBIN";//默认存放位置---Resources文件夹内部

            if (!Directory.Exists(BINFolder))
            {
                Directory.CreateDirectory(BINFolder);
            }
            //确定是否已经有BIN文件夹，如果存在就全部重新生成
            bool isDelete = false;
            if (Directory.GetFiles(BINFolder).Length != 0)
            {
                isDelete = true;
                DeleteFolderFilesSurface(BINFolder);
                Directory.CreateDirectory(BINFolder);
            }

            List<string> fileList = GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//获取所有文件名
            if (fileList.Count == 0)
            {
                MLog.Print($"{EditorSettings.excelGenerationPath}中发现0个Excel文件，请检查路径是否正确.", MLogType.Warning);
                return;
            }

            bool flag = true;
            //遍历所有文件，在BINFolder中创建相应的二进制文件
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string BINPath = $@"{BINFolder}\{fileName}.byte";

                //获取dataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                bool createState = CreateBIN(dataset, BINPath, $" {fileName} ");
                if (!createState) flag = false;
            }
            if (flag)
            {
                string dstFolder = BINFolder.Replace("/", "\\");
                System.Diagnostics.Process.Start(dstFolder);
                AssetDatabase.Refresh();

                if (isDelete) MLog.Print("已重新生成所有BIN文件");
                else MLog.Print("已生成所有BIN文件");
            }
        }

        private bool CreateBIN(DataSet dataSet, string BINPath, string fileName = "")
        {
            if (dataSet.Tables.Count < 1)
            {
                MLog.Print($"CrateBIN：{fileName}没有表存在，请检查.", MLogType.Error);
                return false;
            }
            DataTable sheet = dataSet.Tables[0];//取首表
            //判断数据表内是否存在数据
            if (sheet.Rows.Count < 1)
            {
                MLog.Print($"CrateBIN：{fileName}中不存在数据，请检查.", MLogType.Error);
                return false;
            }

            int rowCount = sheet.Rows.Count;
            int colCount = sheet.Columns.Count;
            Debug.Log(sheet.Rows[0][0]);
        }

        /// <summary>
        /// <para>.xlsx转.csv</para>
        /// 设计失误，不再使用，应该直接使用.xlsx转.byte文件即可
        /// </summary>
        private void XLSX2CSV()
        {
            //string CSVFolder = $@"{Application.dataPath}/Resources";//CSV临时文件存放位置---Resources文件夹
            string CSVFolder = $@"{EditorSettings.excelGenerationPath}/CSVTemp";//CSV临时文件存放位置
            if (!Directory.Exists(CSVFolder))
            {
                Directory.CreateDirectory(CSVFolder);
            }
            //确定是否已经有CSV临时文件，如果存在就全部重新生成
            bool isDelete = false;
            if (Directory.GetFiles(CSVFolder).Length != 0)
            {
                isDelete = true;
                DeleteFolderFilesSurface(CSVFolder);
                Directory.CreateDirectory(CSVFolder);
            }

            List<string> fileList = GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//获取所有文件名
            if (fileList.Count == 0)
            {
                MLog.Print($"{EditorSettings.excelGenerationPath}中发现0个Excel文件，请检查路径是否正确.", MLogType.Warning);
                return;
            }

            bool flag = true;
            //遍历所有文件，在CSVFolder中创建相应的临时文件
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string CSVPath = $@"{CSVFolder}\{fileName}_Temp.csv";

                //获取dataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                bool createState = CreateCSV(dataset, CSVPath, $" {fileName} ");
                if (!createState) flag = false;
            }
            if (flag)
            {
                string dstFolder = CSVFolder.Replace("/", "\\");
                System.Diagnostics.Process.Start(dstFolder);
                AssetDatabase.Refresh();

                if(isDelete) MLog.Print("已重新生成所有CSV文件");
                else MLog.Print("已生成所有CSV文件");
            }
        }

        public bool CreateCSV(DataSet dataSet, string CSVPath, string fileName = "")
        {
            if (dataSet.Tables.Count < 1)
            {
                MLog.Print($"CrateCSV：{fileName}没有表存在，请检查.", MLogType.Error);
                return false;
            }
            DataTable mSheet = dataSet.Tables[0];//取首表
            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
            {
                MLog.Print($"CrateCSV：{fileName}中不存在数据，请检查.", MLogType.Error);
                return false;
            }

            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;
            StringBuilder sb = new StringBuilder();
            //读取数据
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    //使用","分割每一个数值
                    sb.Append(mSheet.Rows[i][j] + ",");
                }
                sb.Append("\r\n");//换行
            }

            //写入文件
            using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write))
            {
                //注意：指定了UTF8格式(用Excel打开.csv文件时可以正常显示，不指定会变成乱码)
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(sb.ToString());
                }
            }

            return true;
        }

        /// <summary>
        /// 删除文件夹中所有文件(只处理一层，包括文件夹)
        /// </summary>
        private void DeleteFolderFilesSurface(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(folder);
        }

        private List<string> GetFolderFiles(string folder, string extension)
        {
            List<string> res = new List<string>();
            if (Directory.Exists(folder))
            {
                DirectoryInfo info = new DirectoryInfo(folder);
                FileInfo[] files = info.GetFiles("*");
                foreach (var file in files)
                {
                    if (file.Name.EndsWith(extension))
                    {
                        res.Add(file.FullName);
                    }
                }
            }
            return res;
        }

        private DataTable GetDefaultTable()
        {
            DataTable table = new DataTable();

            //必须先创列，才能添加行
            table.Columns.Add("ID");
            table.Columns.Add("NAME");
            table.Columns.Add("DESC");
            //table.Rows.Add(new object[] { "ID", "NAME", "DESC" });
            table.Rows.Add(new object[] { "编号", "名字", "描述" });
            table.Rows.Add(new object[] { "int", "string", "string[]" });
            table.Rows.Add(new object[] { 1    , "苹果"  , "红色#甜" });
            table.Rows.Add(new object[] { 2    , "香蕉"  , "黄色#甜" });
            table.Rows.Add(new object[] { 3    , "橘子"  , "橙色#酸" });

            //table.Columns.Add("编号ID", typeof(object));
            //table.Columns.Add("名字NAME", typeof(object));
            //table.Columns.Add("描述DESC", typeof(object));
            //table.Rows.Add(new object[] {"int","string","string[]"});
            //table.Rows.Add(new object[] {1    ,"苹果"  ,"红色#甜"   });
            //table.Rows.Add(new object[] {2    ,"香蕉"  ,"黄色#甜"   });
            //table.Rows.Add(new object[] {3    ,"橘子"  ,"橙色#酸"   });

            return table;
        }
    }
}
