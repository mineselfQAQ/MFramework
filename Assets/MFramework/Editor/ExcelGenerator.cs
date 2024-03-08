using Excel;
using OfficeOpenXml;
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
            GUIUtility.DrawTitle(5, "Excel编辑器");

            //if (GUILayout.Button("更改Excel存储路径"))
            //{
            //    EditorSettingsController.ChangePath(EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath));
            //    Log.Print($"已更改路径{EditorSettings.excelGenerationPath}.");
            //}

            if (GUILayout.Button("生成Excel文件"))
            {
                CreateExcelFile();
            }

            EditorGUILayout.LabelField("------------------------------------------", GUIStyleUtility.BoldStyle);

            if (GUILayout.Button("生成持久化数据(XLSX--->CSV)"))
            {
                XLSX2CSV();
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
                    Log.Print("已取消生成Excel文件.", MLogType.Warning);
                    return;
                }

                FileInfo file = new FileInfo(path);
                string fileName = Path.GetFileName(path);
                //如果文件已经存在，重新生成
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(path);

                    Log.Print(Log.BoldWord($"已重新生成{fileName}."), MLogType.Warning);
                }

                //创建Excel文件
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//创建表1
                    worksheet.Cells["A1"].LoadFromDataTable(GetDefaultTable(), true);//创建初始表内容
                    worksheet.Cells["A1:C4"].AutoFitColumns();//调整行宽

                    package.Save();
                }

                System.Diagnostics.Process.Start($"Explorer.exe", $@"/select,{path}");
                Log.Print($"已生成{fileName}.");
            }
            else if (state == 1)//取消
            {
                Log.Print("已取消生成Excel文件.", MLogType.Warning);
            }
            else//更改路径
            {
                string pathName = EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath);
                EditorSettingsController.ChangePath(pathName);
                Log.Print($"已更改{pathName}路径.");
            }
        }

        private void XLSX2CSV()
        {
            string CSVFolder = $@"{EditorSettings.excelGenerationPath}\CSVTemp";//CSV临时文件存放位置
            if (!Directory.Exists(CSVFolder))
            {
                Directory.CreateDirectory(CSVFolder);
            }
            //确定是否已经有CSV临时文件，如果存在就全部重新生成
            if (Directory.GetFiles(CSVFolder).Length != 0)
            {
                DeleteFolderFilesSurface(CSVFolder);
                Directory.CreateDirectory(CSVFolder);
            }

            List<string> fileList = GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//获取所有文件名
            if (fileList.Count == 0)
            {
                Log.Print($"{EditorSettings.excelGenerationPath}中发现0个Excel文件，请检查路径是否正确.", MLogType.Warning);
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

                bool createState = CreateCSV(dataset, CSVPath, $"{ fileName }");
                if (!createState) flag = false;
            }
            if (flag)
            {
                EditorUtility.RevealInFinder(12345);
                Log.Print("转换完成");
            }
        }

        public bool CreateCSV(DataSet dataSet, string CSVPath, string fileName = "")
        {
            if (dataSet.Tables.Count < 1)
            {
                Log.Print($"CrateCSV：{fileName}没有表存在，请检查.", MLogType.Error);
                return false;
            }
            DataTable mSheet = dataSet.Tables[0];//取首表
            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
            {
                Log.Print($"CrateCSV：{fileName}中不存在数据，请检查.", MLogType.Error);
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
                using (TextWriter textWriter = new StreamWriter(fileStream))
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
                Debug.Log(file);
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

            table.Columns.Add("编号ID", typeof(object));
            table.Columns.Add("名字NAME", typeof(object));
            table.Columns.Add("描述DESC", typeof(object));
            table.Rows.Add(new object[] {"int","string","string[]"});
            table.Rows.Add(new object[] {1    ,"苹果"  ,"红色#甜"   });
            table.Rows.Add(new object[] {2    ,"香蕉"  ,"黄色#甜"   });
            table.Rows.Add(new object[] {3    ,"橘子"  ,"橙色#酸"   });

            return table;
        }
    }
}
