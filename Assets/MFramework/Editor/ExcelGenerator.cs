//using CsvHelper;
using Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
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

            if (GUILayout.Button("生成持久化数据"))
            {
                XLSX2PersistentData();
            }
        }

        private void CreateExcelFile()
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{EditorSettings.excelGenerationPath}处吗？", "确认", "取消", "更改路径");
            if (state == 0)//确认
            {
                string path = EditorUtility.SaveFilePanel("保存", EditorSettings.excelGenerationPath, "Sheet", "xlsx");
                path = path.Replace("/", "\\");//Process.Start()只接受\而非/

                if (path == "")
                {
                    MLog.Print("已取消生成Excel文件.", MLogType.Warning);
                    return;
                }

                FileInfo file = new FileInfo(path);
                string fileName = Path.GetFileName(path);
                //如果文件已经存在，重新生成
                bool isExist = false;
                if (file.Exists)
                {
                    isExist = true;

                    file.Delete();
                    file = new FileInfo(path);
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
                if (isExist) MLog.Print(MLog.BoldWord($"已重新生成{fileName}."), MLogType.Warning);
                else MLog.Print($"已生成{fileName}.");
            }
            else if (state == 1)//取消
            {
                MLog.Print("已取消生成Excel文件.", MLogType.Warning);
            }
            else//更改路径
            {
                string pathName = EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath);
                bool flag = EditorSettingsController.ChangePath(pathName);
                if(flag) MLog.Print($"已更改{pathName}路径.");
            }
        }

        private void XLSX2PersistentData()
        {
            string resourceFolder = $@"{Application.dataPath}\Resources";
            string BINFolder = $@"{resourceFolder}\ExcelBIN";//默认.byte文件存放位置---Resources文件夹内部
            string CSFolder = EditorSettings.tableCSGenerationPath;

            //检查文件夹是否存在，如果不存在就创建
            CreateFolderIfDirectoryNotExist(resourceFolder);
            CreateFolderIfDirectoryNotExist(BINFolder);
            CreateFolderIfDirectoryNotExist(CSFolder);
            //确定是否已经有BIN文件夹，如果存在就全部重新生成
            bool isDelete1 = RecreateDirectoryIfFolderNotEmpty(BINFolder);
            bool isDelete2 = RecreateDirectoryIfFolderNotEmpty(CSFolder);

            List<string> fileList = GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//获取所有文件名
            if (fileList.Count == 0)
            {
                MLog.Print($"{EditorSettings.excelGenerationPath}中发现0个Excel文件，请检查路径是否正确.", MLogType.Warning);
                return;
            }

            bool flag = true;
            //遍历所有文件，创建脚本与二进制文件
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string CSPath = $@"{CSFolder}\{fileName}.cs";
                string BINPath = $@"{BINFolder}\{fileName}.byte";

                //获取dataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                if (!CheckTable(dataset))
                {
                    MLog.Print($"{fileName}表存在问题，请检查.", MLogType.Error);
                    continue;
                }
                GetDataFromTable(dataset.Tables[0], 
                    out string[] names, out string[] types, out object[][] data);

                CreateCS(CSPath, names, types);
                CreateBIN(BINPath, fileName, data);
            }
            if (flag)
            {
                //string dstFolder = BINFolder.Replace("/", "\\");
                //System.Diagnostics.Process.Start(dstFolder);
                //AssetDatabase.Refresh();

                if (isDelete1 || isDelete2) MLog.Print("已重新生成所有文件");
                else MLog.Print("已生成所有文件");
            }
        }

        /// <summary>
        /// //保证文件夹的创建
        /// </summary>
        private void CreateFolderIfDirectoryNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// //保证文件夹的最新状态(重新创建)
        /// </summary>
        /// <param name="path"></param>
        /// <returns>重新创建时为true，否则为false</returns>
        private bool RecreateDirectoryIfFolderNotEmpty(string path)
        {
            if (Directory.GetFiles(path).Length != 0)
            {
                DeleteFolderFilesSurface(path);
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }

        private bool CheckTable(DataSet dataSet)
        {
            if (dataSet.Tables.Count < 1)//是否存在表
            {
                return false;
            }
            DataTable sheet = dataSet.Tables[0];//取首表
            //判断数据表内是否存在数据
            if (sheet.Rows.Count < 1)
            {
                return false;
            }
            return true;
        }

        private void GetDataFromTable(DataTable sheet, out string[] names, out string[] types, out object[][] data)
        {
            int rowCount = sheet.Rows.Count;
            int colCount = sheet.Columns.Count;

            //初始化数组
            names = new string[colCount];
            types = new string[colCount];
            data = new string[rowCount - 3][];
            for (int i = 0; i < data.Length; i++) data[i] = new string[colCount];
            //初始化数据
            //TODO:对于非string类型，必须将其使用类似Convert.ToInt32()方法指示
            for (int i = 0; i < colCount; i++)
            {
                names[i] = sheet.Rows[1][i].ToString();
                types[i] = sheet.Rows[2][i].ToString();
            }
            for (int i = 0; i < rowCount - 3; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    data[i][j] = sheet.Rows[3 + i][j];
                }
            }
        }

        private void CreateCS(string CSPath, string[] names, string[] types)
        {
            string code = CSBASECODE;

            string className = Path.GetFileNameWithoutExtension(CSPath);
            string collectionClassName = $"{className}s";

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{CollectionClassName}", collectionClassName);

            string fieldsDefine = GenerateFieldsDefine(names, types);
            string propertiesDefine = GeneratePropertiesDefine(names, types);
            string constructorDefine = GenerateConstructorDefine(className, names, types);

            code = code.Replace("{FieldsDefine}", fieldsDefine);
            code = code.Replace("{PropertiesDefine}", propertiesDefine);
            code = code.Replace("{ConstructorDefine}", constructorDefine);

            //写入文件
            using (FileStream fileStream = new FileStream(CSPath, FileMode.Create, FileAccess.Write))
            {
                //注意：指定了UTF8格式(用Excel打开.csv文件时可以正常显示，不指定会变成乱码)
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(code);
                }
            }
        }

        private string GenerateFieldsDefine(string[] names, string[] types)
        {
            StringBuilder res = new StringBuilder();

            int n = names.Length;
            for (int i = 0; i < n; i++)
            {
                string name = names[i].ToLower();

                string tempLine = FIELDBASECODE;
                tempLine = tempLine.Replace("{Type}", types[i]);
                tempLine = tempLine.Replace("{Name}", name);

                if(i != n - 1) res.Append(tempLine + "\n\t\t");
                else res.Append(tempLine);
            }

            return res.ToString();
        }

        private string GeneratePropertiesDefine(string[] names, string[] types)
        {
            StringBuilder res = new StringBuilder();

            int n = names.Length;
            for (int i = 0; i < n; i++)
            {
                string name = names[i].ToUpper();

                string tempLine = PROPERTIESBASECODE;
                tempLine = tempLine.Replace("{Type}", types[i]);
                tempLine = tempLine.Replace("{Name}", name);

                if (i != n - 1) res.Append(tempLine + "\n\t\t");
                else res.Append(tempLine);
            }

            return res.ToString();
        }

        private string GenerateConstructorDefine(string className, string[] names, string[] types)
        {
            string constructorBaseCode = CONSTRUCTORBASECODE;
            constructorBaseCode = constructorBaseCode.Replace("{ClassName}", className);
            constructorBaseCode = constructorBaseCode.Replace("{Parameter}", GetParameter(names, types));
            constructorBaseCode = constructorBaseCode.Replace("{AssignmentOperator}", GetAssignmentOperator(names));
            return constructorBaseCode;

            string GetParameter(string[] names, string[] types)
            {
                StringBuilder sb = new StringBuilder();
                int n = names.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    sb.Append($"{types[i]} {names[i].ToLower()}, ");
                }
                sb.Append($"{types[n - 1]} {names[n - 1].ToLower()}");

                return sb.ToString();
            }
            string GetAssignmentOperator(string[] names)
            {
                StringBuilder sb = new StringBuilder();
                int n = names.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    sb.Append($"{names[i].ToUpper()} = {names[i].ToLower()};\n\t\t\t");
                }
                sb.Append($"{names[n - 1].ToUpper()} = {names[n - 1].ToLower()};");

                return sb.ToString();
            }
        }

        private void CreateBIN(string BINPath, string className, object[][] data)
        {
            //GenerateInstance();

            string csAssemblyPath = $"{Application.dataPath}/../Library/ScriptAssemblies/Assembly-CSharp.dll";
            Assembly assembly = Assembly.LoadFile(csAssemblyPath);
            if (assembly != null)
            {
                Type[] types = assembly.GetTypes();
                foreach(var type in types)
                {
                    if (type.Namespace == "Table" && type.Name == className)
                    {
                        var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                        var ctor = ctors[0];//只有私有构造函数

                        //foreach(var param in ctors[0].GetParameters()) 
                        //{
                        //    Debug.Log(param.ParameterType);
                        //}

                        int rowLength = data.Length;
                        int colLength = data[0].Length;

                        object[] instances = new object[rowLength];
                        for (int i = 0; i < rowLength; i++)
                        {
                            object[] parameters = new object[colLength];
                            for (int j = 0; j < colLength; j++)
                            {
                                parameters[j] = data[i][j];
                            }
                            object instance = ctor.Invoke(parameters);
                            instances[i] = instance;
                        }
                        Debug.Log(instances[0]);
                        Debug.Log(instances[1]);
                        Debug.Log(instances[2]);
                    }
                }
            }
        }

        //private void CreateBIN(string BINPath, Type[] types, object[][] obj2)
        //{
        //    object[][] ans = new object[obj2.Length][];
        //    for (int i = 0; i < ans.Length; i++)
        //    {
        //        ans[i] = new object[obj2[0].Length];
        //        for (int j = 0; j < ans[i].Length; j++)
        //        {
        //            ans[i][j] = obj2[i][j] as GetType(types[j]);
        //        }
        //    }

        //}

        /// <summary>
        /// <para>.xlsx转.csv</para>
        /// 设计失误，应该直接使用.xlsx转二进制文件即可
        /// </summary>
        private void XLSX2CSV()
        {
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
        private bool CreateCSV(DataSet dataSet, string CSVPath, string fileName = "")
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
            table.Columns.Add("编号");
            table.Columns.Add("名字");
            table.Columns.Add("描述");
            //table.Rows.Add(new object[] { "编号", "名字", "描述" });
            table.Rows.Add(new object[] { "ID", "NAME", "DESC" });
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

        private const string CSBASECODE = 
@"using System;
using System.Collections.Generic;

namespace Table
{
    [Serializable]
    public class {ClassName}
    {
        {FieldsDefine}
        
        {PropertiesDefine}

        {ConstructorDefine}
    }

    [Serializable]
    public class {CollectionClassName}
    {
        public List<{ClassName}> items;

        private {CollectionClassName}(List<{ClassName}> items)
        {
            this.items = items;
        }
    }
}";
        private const string FIELDBASECODE = "private {Type} {Name};";
        private const string PROPERTIESBASECODE = "public {Type} {Name} { get; private set; }";
        private const string CONSTRUCTORBASECODE = 
@"private {ClassName}({Parameter})
        {
            {AssignmentOperator}
        }";
    }
}
