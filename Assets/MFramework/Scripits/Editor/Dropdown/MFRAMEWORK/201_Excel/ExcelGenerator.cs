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
        [MenuItem("MFramework/GenerateAllExcel _F8", priority = 202)]
        public static void GenerateAllExcel()
        {
            EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, true);
            ExcelGenerator window = CreateInstance<ExcelGenerator>();
            window.XLSX2PersistentData();
        }

        [MenuItem("MFramework/ExcelGenerator", priority = 201)]
        public static void Init()
        {
            ExcelGenerator window = GetWindow<ExcelGenerator>(true, "ExcelGenerator", false);
            window.minSize = new Vector2(300, 220);
            window.maxSize = new Vector2(300, 220);
            window.Show();
        }

        private void OnGUI()
        {
            //标题Excel编辑器
            MEditorGUIUtility.DrawH1("Excel编辑器");

            DrawGenerateExcelPart();//生成Excel部分

            EditorGUILayout.LabelField("------------------------------------------", MEditorGUIStyleUtility.BoldStyle);

            DrawGeneratePersistentDataPart();//生成持久化数据部分

            EditorGUILayout.Space(20);

            DrawCheckPathBtn();
        }

        #region 公开函数
        //public static void CreateCSBIN(string excelName, string CSName, string BINName, string BINLoadPath)
        //{
        //    CreateSingleBIN(excelName, BINName);

        //    AssetDatabase.Refresh();
        //    EditorUtility.RequestScriptReload();
        //}
        public static bool CreateSingleCS(string excelName, string CSName, string BINLoadPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(excelName);
            string CSPath = CSName;
            string BINPath = BINLoadPath;

            FileStream stream = File.Open(excelName, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataset = excelReader.AsDataSet();

            if (!CheckTable(dataset))
            {
                MLog.Print($"{fileName}表存在问题，请检查.", MLogType.Error);
                return false;
            }
            GetDataFromTable(dataset.Tables[0],
                out string[] names, out string[] types, out object[][] data);

            bool isSucceed = CreateCS(CSPath, BINPath, names, types, "MFramework");
            return isSucceed;
        }
        public static bool CreateSingleBIN(string excelName, string BINName)
        {
            string fileName = Path.GetFileNameWithoutExtension(excelName);

            //获取dataset
            FileStream stream = File.Open(excelName, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataset = excelReader.AsDataSet();

            if (!CheckTable(dataset))
            {
                MLog.Print($"{fileName}表存在问题，请检查.", MLogType.Error);
                return false;
            }
            GetDataFromTable(dataset.Tables[0],
                out string[] names, out string[] types, out object[][] data);

            bool isSucceed = CreateBIN(BINName, fileName, data, "MFramework.Runtime.dll");
            return isSucceed;
        }
        #endregion

        #region Excel部分
        private void DrawGenerateExcelPart()
        {
            MEditorGUIUtility.DrawH2("生成Excel文件");

            if (GUILayout.Button("生成"))
            {
                CreateExcelFile();
            }
        }

        private void CreateExcelFile()
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{MConfigurableSettings.ExcelPath}处吗？", "确认", "取消", "更改路径");
            if (state == 0)//确认
            {
                string path = EditorUtility.SaveFilePanel("保存", MConfigurableSettings.ExcelPath, "Sheet", "xlsx");
                path = path.ReplaceSlash(false);//Process.Start()只接受\而非/

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

                    package.Save();
                }

                System.Diagnostics.Process.Start($"Explorer.exe", $@"/select,{path}");
                if (isExist) MLog.Print($"已重新生成{fileName}.", MLogType.Warning);
                else MLog.Print($"已生成{fileName}.");
            }
            else if (state == 1)//取消
            {
                MLog.Print("已取消生成Excel文件.", MLogType.Warning);
            }
            else//更改路径
            {
                string pathName = MConfigurableSettingsBase.GetPathName(MConfigurableName.ExcelGenerationPath);
                bool flag = EditorSettingsConfigurator.ChangePath(pathName);
                if (flag) MLog.Print($"已更改{pathName}路径.");

                AssetDatabase.Refresh();
            }
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
            table.Rows.Add(new object[] { 1, "苹果", "红色#甜" });
            table.Rows.Add(new object[] { 2, "香蕉", "黄色#甜" });
            table.Rows.Add(new object[] { 3, "橘子", "橙色#酸" });

            return table;
        }
        #endregion

        #region CS/BIN部分
        private void DrawGeneratePersistentDataPart()
        {
            MEditorGUIUtility.DrawH2("生成持久化数据");

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("1.创建.cs文件"))
                {
                    string BINFolder = MSettings.ExcelBINPath;
                    string CSFolder = MConfigurableSettings.ExcelCSPath;
                    List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//获取所有文件名

                    bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.CS);
                    if (!haveFile) return;

                    CreateAllCS(BINFolder, CSFolder, fileList);
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("2.创建.byte文件"))
                {
                    string BINFolder = MSettings.ExcelBINPath;
                    string CSFolder = MConfigurableSettings.ExcelCSPath;
                    List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//获取所有文件名

                    bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.BIN);
                    if (!haveFile) return;

                    CreateAllBIN(BINFolder, fileList);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("一键生成(F8)"))
            {
                EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, true);
                XLSX2PersistentData();
            }
        }

        /// <summary>
        /// 完成.cs生成后立刻进行.byte生成
        /// </summary>
        private void XLSX2PersistentData()
        {
            string BINFolder = MSettings.ExcelBINPath;//默认.byte文件存放位置---StreamingAssets文件夹内部
            string CSFolder = MConfigurableSettings.ExcelCSPath;
            List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//获取所有文件名

            bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.CSAndBIN);
            if (!haveFile) return;

            CreateAllCS(BINFolder, CSFolder, fileList);

            //此处需要进行域重新加载，完成后才能生成BIN文件  Tip:必须先Refresh()再Reload()才可以
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            //"延迟执行"，在ExcelDelayDo中---InitializeAfterAssemblyReload()
            //CreateAllBIN(BINFolder, fileList);
        }

        private bool CreateAllCS(string BINFolder, string CSFolder, List<string> fileList)
        {
            bool flag = true;
            //遍历所有文件，创建脚本
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string CSPath = $"{CSFolder}/{fileName}.cs".ReplaceSlash();
                string BINPath = $"{BINFolder}/{fileName}.byte".ReplaceSlash();

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

                bool isSucceed = CreateCS(CSPath, BINPath, names, types);
                if (!isSucceed) flag = false;
            }
            if (flag)
            {
                //TODO:不知道为什么在家中创建.cs文件中的Log都不输出
                MLog.Print("已生成所有CS文件");
                return true;
            }
            else
            {
                MLog.Print("有CS文件未成功生成，请检查", MLogType.Warning);
                return false;
            }
        }
        private static bool CreateCS(string CSPath, string BINPath, string[] names, string[] types, string nameSpace = "")
        {
            string code;
            bool haveNamespace;
            if (nameSpace == "")
            {
                code = CSBASECODE;
                haveNamespace = false;
            }
            else
            {
                code = CSBASECODEWITHNAMESPACE;
                haveNamespace = true;
            }

            code = code.Replace("{NameSpace}", nameSpace);

            string className = Path.GetFileNameWithoutExtension(CSPath);
            string collectionClassName = $"{className}s";

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{CollectionClassName}", collectionClassName);

            string propertiesDefine = GeneratePropertiesDefine(names, types, haveNamespace);
            string constructorDefine = GenerateConstructorDefine(className, names, types, haveNamespace);

            code = code.Replace("{PropertiesDefine}", propertiesDefine);
            code = code.Replace("{ConstructorDefine}", constructorDefine);

            code = code.Replace("{BINPath}", BINPath);

            //写入文件
            MSerializationUtility.SaveToFile(CSPath, code);

            return true;
        }

        public static bool CreateAllBIN(string BINFolder, List<string> fileList)
        {
            bool flag = true;
            //遍历所有文件，创建二进制文件
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string BINPath = $"{BINFolder}/{fileName}.byte".ReplaceSlash();

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

                bool isSucceed = CreateBIN(BINPath, fileName, data);
                if (!isSucceed) flag = false;
            }
            if (flag)
            {
                MLog.Print("已生成所有BIN文件");
                return true;
            }
            else
            {
                MLog.Print("有BIN文件未成功生成，请检查", MLogType.Warning);
                return false;
            }
        }
        private static bool CreateBIN(string BINPath, string className, object[][] data, string dllName = "Assembly-CSharp.dll")
        {
            //注意！！！
            //打成dll后不再是原来的Assembly-CSharp的程序集，而是自己创建的dll的程序集
            string CSAssemblyPath = $"{Application.dataPath}/../Library/ScriptAssemblies/{dllName}";
            Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

            int rowLength = data.Length;
            int colLength = data[0].Length;
            Array instances = null;
            object resInstance = null;

            Type[] types = assembly.GetTypes();
            foreach (var type in types)//1.创建所有小实例
            {
                if (type.Name == className)
                {
                    instances = Array.CreateInstance(type, rowLength);

                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = ctors[0];//只有私有构造函数

                    for (int i = 0; i < rowLength; i++)
                    {
                        object[] parameters = new object[colLength];
                        for (int j = 0; j < colLength; j++)
                        {
                            parameters[j] = data[i][j];
                        }
                        object instance = ctor.Invoke(parameters);
                        instance = Convert.ChangeType(instance, type);
                        instances.SetValue(instance, i);
                    }
                }
            }
            foreach (var type in types)//2.通过小实例组成大实例
            {
                if (type.Name == $"{className}s")
                {
                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = ctors[0];//只有私有构造函数

                    object[] parameter = new object[1] { instances };
                    resInstance = ctor.Invoke(parameter);
                }
            }

            bool flag = MSerializationUtility.SaveToByte(resInstance, BINPath);
            if (!flag) MLog.Print($"{typeof(ExcelGenerator)}：序列化失败", MLogType.Error);

            return true;
        }

        private static string GeneratePropertiesDefine(string[] names, string[] types, bool haveNamespace)
        {
            StringBuilder res = new StringBuilder();

            int n = names.Length;
            for (int i = 0; i < n; i++)
            {
                string name = names[i].ToUpper();

                string tempLine = PROPERTIESBASECODE;
                tempLine = tempLine.Replace("{Type}", types[i]);
                tempLine = tempLine.Replace("{Name}", name);

                if (i != n - 1)
                {
                    if(haveNamespace) res.Append(tempLine + "\n\t\t");
                    else res.Append(tempLine + "\n\t");
                }
                else res.Append(tempLine);
            }

            return res.ToString();
        }
        private static string GenerateConstructorDefine(string className, string[] names, string[] types, bool haveNamespace)
        {
            string constructorBaseCode = haveNamespace ? CONSTRUCTORBASECODEWITHNAMESPACE : CONSTRUCTORBASECODE;
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
                    if(haveNamespace) sb.Append($"{names[i].ToUpper()} = {names[i].ToLower()};\n\t\t\t");
                    else sb.Append($"{names[i].ToUpper()} = {names[i].ToLower()};\n\t\t");
                }
                sb.Append($"{names[n - 1].ToUpper()} = {names[n - 1].ToLower()};");

                return sb.ToString();
            }
        }

        public static void GetDataFromTable(DataTable sheet, out string[] names, out string[] types, out object[][] data)
        {
            //避免多余行
            //int actualRowCount = 0;
            //foreach (DataRow row in sheet.Rows)
            //{
            //    if (!row.IsNull(0))
            //    {
            //        actualRowCount++;
            //    }
            //}

            int rowCount = sheet.Rows.Count;
            int colCount = sheet.Columns.Count;

            //计算实际列数(去除类型为none的列)
            int typeCount = 0;
            for (int i = 0; i < colCount; i++)
            {
                if (sheet.Rows[2][i].ToString() == "none") continue;
                typeCount++;
            }

            //初始化数组
            names = new string[typeCount];
            types = new string[typeCount];
            data = new object[rowCount - 3][];
            for (int i = 0; i < data.Length; i++) data[i] = new object[typeCount];
            //初始化数据
            //names/types(第二行英文名/第三行类型名)
            for (int i = 0, col = 0; i < typeCount; i++, col++)
            {
                while (sheet.Rows[2][col].ToString() == "none") col++;

                names[i] = sheet.Rows[1][col].ToString();
                types[i] = sheet.Rows[2][col].ToString();
            }
            //data
            for (int i = 0, col = 0; i < typeCount; i++, col++)
            {
                while (sheet.Rows[2][col].ToString() == "none") col++;
                string colType = sheet.Rows[2][col].ToString();

                for (int row = 0; row < rowCount - 3; row++)
                {
                    if (colType == "byte")
                    {
                        data[row][i] = Convert.ToByte(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "byte[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        byte[] resBytes = new byte[n];
                        for (int j = 0; j < n; j++)
                        {
                            resBytes[j] = Convert.ToByte(splitStrs[j]);
                        }

                        data[row][i] = resBytes;
                    }
                    else if (colType == "short")
                    {
                        data[row][i] = Convert.ToInt16(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "short[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        short[] resShorts = new short[n];
                        for (int j = 0; j < n; j++)
                        {
                            resShorts[j] = Convert.ToInt16(splitStrs[j]);
                        }

                        data[row][i] = resShorts;
                    }
                    else if (colType == "int")
                    {
                        data[row][i] = Convert.ToInt32(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "int[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        int[] resInts = new int[n];
                        for (int j = 0; j < n; j++)
                        {
                            resInts[j] = Convert.ToInt32(splitStrs[j]);
                        }

                        data[row][i] = resInts;
                    }
                    else if (colType == "long")
                    {
                        data[row][i] = Convert.ToUInt64(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "long[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        long[] resLongs = new long[n];
                        for (int j = 0; j < n; j++)
                        {
                            resLongs[j] = Convert.ToInt64(splitStrs[j]);
                        }

                        data[row][i] = resLongs;
                    }
                    else if (colType == "float")
                    {
                        data[row][i] = Convert.ToSingle(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "float[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        float[] resFloats = new float[n];
                        for (int j = 0; j < n; j++)
                        {
                            resFloats[j] = Convert.ToSingle(splitStrs[j]);
                        }

                        data[row][i] = resFloats;
                    }
                    else if (colType == "double")
                    {
                        data[row][i] = Convert.ToDouble(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "double[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        double[] resDoubles = new double[n];
                        for (int j = 0; j < n; j++)
                        {
                            resDoubles[j] = Convert.ToDouble(splitStrs[j]);
                        }

                        data[row][i] = resDoubles;
                    }
                    else if (colType == "bool")
                    {
                        data[row][i] = Convert.ToBoolean(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "char")
                    {
                        data[row][i] = Convert.ToChar(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "char[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        char[] resChars = new char[n];
                        for (int j = 0; j < n; j++)
                        {
                            resChars[j] = Convert.ToChar(splitStrs[j]);
                        }

                        data[row][i] = resChars;
                    }
                    else if (colType == "string")
                    {
                        data[row][i] = sheet.Rows[3 + row][col].ToString();
                    }
                    else if (colType == "string[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] resStr = originStr.Split("#");
                        data[row][i] = resStr;
                    }
                    else if (colType == "none")
                    {
                        break;
                    }
                    else
                    {
                        MLog.Print("数据表中存在未知类型，请检查.", MLogType.Warning);
                    }
                }
            }
        }

        private bool CheckAllFolder(string BINFolder, string CSFolder, List<string> fileList, CreateMode mode)
        {
            //检查文件夹是否存在，如果不存在就创建
            MPathUtility.CreateFolderIfNotExist(BINFolder);
            MPathUtility.CreateFolderIfNotExist(CSFolder);
            //确定是否已经有文件夹，如果存在就全部重新生成
            if (mode == CreateMode.CS)
            {
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(CSFolder);
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(BINFolder);
            }
            else if (mode == CreateMode.BIN)
            {
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(BINFolder);
            }
            else if (mode == CreateMode.CSAndBIN)
            {
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(CSFolder);
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(BINFolder);
            }

            if (fileList.Count == 0)
            {
                MLog.Print($"{MConfigurableSettings.ExcelPath}中发现0个Excel文件，请检查路径是否正确.", MLogType.Warning);
                return false;
            }
            return true;
        }
        public static bool CheckTable(DataSet dataSet)
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
        #endregion

        #region 打开路径配置按钮
        private void DrawCheckPathBtn()
        {
            if (GUILayout.Button("检查路径配置器"))
            {
                EditorSettingsConfigurator.Init();
            }
        }
        #endregion


        #region 预设初始代码
        private const string CSBASECODEWITHNAMESPACE =
      @"using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace {NameSpace}
{
    [Serializable]
    public class {ClassName}
    {
        {PropertiesDefine}

        {ConstructorDefine}

        public static {ClassName}[] LoadBytes()
        {
            string path = $""{BINPath}"";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                {CollectionClassName} table = binaryFormatter.Deserialize(stream) as {CollectionClassName};
                {ClassName}[] res = table.items;
                return res;
            }
        }
    }

    [Serializable]
    internal class {CollectionClassName}
    {
        public {ClassName}[] items;

        private {CollectionClassName}({ClassName}[] items)
        {
            this.items = items;
        }
    }
}";
        private const string CSBASECODE =
      @"using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class {ClassName}
{
    {PropertiesDefine}

    {ConstructorDefine}

    public static {ClassName}[] LoadBytes()
    {
        string path = $""{BINPath}"";
        if (!File.Exists(path)) return null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            {CollectionClassName} table = binaryFormatter.Deserialize(stream) as {CollectionClassName};
            {ClassName}[] res = table.items;
            return res;
        }
    }
}

[Serializable]
internal class {CollectionClassName}
{
    public {ClassName}[] items;

    private {CollectionClassName}({ClassName}[] items)
    {
        this.items = items;
    }
}";
        private const string FIELDBASECODE = "private {Type} {Name};";
        private const string PROPERTIESBASECODE = "public {Type} {Name} { get; private set; }";
        private const string CONSTRUCTORBASECODEWITHNAMESPACE = 
@"private {ClassName}({Parameter})
        {
            {AssignmentOperator}
        }"; 
        private const string CONSTRUCTORBASECODE =
@"private {ClassName}({Parameter})
    {
        {AssignmentOperator}
    }";
        #endregion
    }

    public enum CreateMode
    {
        CS,
        BIN,
        CSAndBIN
    }
}
