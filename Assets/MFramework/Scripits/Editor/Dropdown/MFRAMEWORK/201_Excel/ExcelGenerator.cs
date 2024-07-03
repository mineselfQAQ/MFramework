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
        [MenuItem("MFramework/GenerateAllExcel _F8", priority = 109)]
        public static void GenerateAllExcel()
        {
            EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, true);
            ExcelGenerator window = CreateInstance<ExcelGenerator>();
            window.XLSX2PersistentData();
        }

        [MenuItem("MFramework/ExcelGenerator", priority = 101)]
        public static void Init()
        {
            ExcelGenerator window = GetWindow<ExcelGenerator>(true, "ExcelGenerator", false);
            window.minSize = new Vector2(300, 220);
            window.maxSize = new Vector2(300, 220);
            window.Show();
        }

        private void OnGUI()
        {
            //����Excel�༭��
            MGUIUtility.DrawH1("Excel�༭��");

            DrawGenerateExcelPart();//����Excel����

            EditorGUILayout.LabelField("------------------------------------------", MGUIStyleUtility.BoldStyle);

            DrawGeneratePersistentDataPart();//���ɳ־û����ݲ���

            EditorGUILayout.Space(20);

            DrawCheckPathBtn();
        }

        private void DrawGenerateExcelPart()
        {
            MGUIUtility.DrawH2("����Excel�ļ�");

            if (GUILayout.Button("����"))
            {
                CreateExcelFile();
            }
        }

        private void DrawGeneratePersistentDataPart()
        {
            MGUIUtility.DrawH2("���ɳ־û�����");

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("1.����.cs�ļ�"))
                {
                    string BINFolder = EditorSettings.excelBINGenerationPath;
                    string CSFolder = EditorSettings.excelCSGenerationPath;
                    List<string> fileList = MPathUtility.GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//��ȡ�����ļ���

                    bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.CS);
                    if (!haveFile) return;

                    CreateAllCS(BINFolder, CSFolder, fileList);
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("2.����.byte�ļ�"))
                {
                    string BINFolder = EditorSettings.excelBINGenerationPath;
                    string CSFolder = EditorSettings.excelCSGenerationPath;
                    List<string> fileList = MPathUtility.GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//��ȡ�����ļ���

                    bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.BIN);
                    if (!haveFile) return;

                    CreateAllBIN(BINFolder, fileList);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("һ������(F8)"))
            {
                EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, true);
                XLSX2PersistentData();
            }
        }

        private void DrawCheckPathBtn()
        {
            if (GUILayout.Button("���·��������"))
            {
                EditorSettingsConfigurator.Init();
            }
        }

        private void CreateExcelFile()
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"ȷ���ļ���������{EditorSettings.excelGenerationPath}����", "ȷ��", "ȡ��", "����·��");
            if (state == 0)//ȷ��
            {
                string path = EditorUtility.SaveFilePanel("����", EditorSettings.excelGenerationPath, "Sheet", "xlsx");
                path = path.Replace("/", "\\");//Process.Start()ֻ����\����/

                if (path == "")
                {
                    MLog.Print("��ȡ������Excel�ļ�.", MLogType.Warning);
                    return;
                }

                FileInfo file = new FileInfo(path);
                string fileName = Path.GetFileName(path);
                //����ļ��Ѿ����ڣ���������
                bool isExist = false;
                if (file.Exists)
                {
                    isExist = true;

                    file.Delete();
                    file = new FileInfo(path);
                }

                //����Excel�ļ�
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//������1
                    worksheet.Cells["A1"].LoadFromDataTable(GetDefaultTable(), true);//������ʼ������

                    worksheet.Cells["A1:C6"].AutoFitColumns();//�����п�
                    worksheet.Cells["A1:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//����
                    worksheet.Cells["A1:C3"].Style.Font.Bold = true;//�Ӵ�

                    package.Save();
                }

                System.Diagnostics.Process.Start($"Explorer.exe", $@"/select,{path}");
                if (isExist) MLog.Print($"����������{fileName}.", MLogType.Warning);
                else MLog.Print($"������{fileName}.");
            }
            else if (state == 1)//ȡ��
            {
                MLog.Print("��ȡ������Excel�ļ�.", MLogType.Warning);
            }
            else//����·��
            {
                string pathName = EditorSettingsBase.GetPathName(EditorPathName.ExcelGenerationPath);
                bool flag = EditorSettingsConfigurator.ChangePath(pathName);
                if (flag) MLog.Print($"�Ѹ���{pathName}·��.");

                AssetDatabase.Refresh();
            }
        }

        private void XLSX2PersistentData()
        {
            string BINFolder = EditorSettings.excelBINGenerationPath;//Ĭ��.byte�ļ����λ��---Resources�ļ����ڲ�
            string CSFolder = EditorSettings.excelCSGenerationPath;
            List<string> fileList = MPathUtility.GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//��ȡ�����ļ���

            bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.CSAndBIN);
            if (!haveFile) return;

            CreateAllCS(BINFolder, CSFolder, fileList);

            //�˴���Ҫ���������¼��أ���ɺ��������BIN�ļ�  Tip:������Refresh()��Reload()�ſ���
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            //"�ӳ�ִ��"����InitializeScript��---InitializeAfterAssemblyReload()
            //CreateAllBIN(BINFolder, fileList);
        }

        private bool CreateAllCS(string BINFolder, string CSFolder, List<string> fileList)
        {
            bool flag = true;
            //���������ļ��������ű�
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string CSPath = $@"{CSFolder}\{fileName}.cs";
                CSPath = CSPath.Replace("/", "\\");
                string BINPath = $@"{BINFolder}\{fileName}.byte";
                BINPath = BINPath.Replace("/", "\\");

                //��ȡdataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                if (!CheckTable(dataset))
                {
                    MLog.Print($"{fileName}��������⣬����.", MLogType.Warning);
                    continue;
                }
                GetDataFromTable(dataset.Tables[0],
                    out string[] names, out string[] types, out object[][] data);

                bool isSucceed = CreateCS(CSPath, BINPath, names, types);
                if (!isSucceed) flag = false;
            }
            if (flag)
            {
                //TODO:��֪��Ϊʲô�ڼ��д���.cs�ļ��е�Log�������
                MLog.Print("����������CS�ļ�");
                return true;
            }
            else
            {
                MLog.Print("��CS�ļ�δ�ɹ����ɣ�����", MLogType.Warning);
                return false;
            }
        }

        private bool CreateCS(string CSPath, string BINPath, string[] names, string[] types)
        {
            string code = CSBASECODE;

            string className = Path.GetFileNameWithoutExtension(CSPath);
            string collectionClassName = $"{className}s";

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{CollectionClassName}", collectionClassName);

            //string fieldsDefine = GenerateFieldsDefine(names, types);
            string propertiesDefine = GeneratePropertiesDefine(names, types);
            string constructorDefine = GenerateConstructorDefine(className, names, types);

            //code = code.Replace("{FieldsDefine}", fieldsDefine);
            code = code.Replace("{PropertiesDefine}", propertiesDefine);
            code = code.Replace("{ConstructorDefine}", constructorDefine);

            code = code.Replace("{BINPath}", BINPath);

            //д���ļ�
            MSerializationUtility.SaveFile(CSPath, code);
            //using (FileStream fileStream = new FileStream(CSPath, FileMode.Create, FileAccess.Write))
            //{
            //    //ע�⣺ָ����UTF8��ʽ(��Excel��.csv�ļ�ʱ����������ʾ����ָ����������)
            //    using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
            //    {
            //        textWriter.Write(code);
            //    }
            //}

            return true;
        }

        public static bool CreateAllBIN(string BINFolder, List<string> fileList)
        {
            bool flag = true;
            //���������ļ��������������ļ�
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string BINPath = $@"{BINFolder}\{fileName}.byte";
                BINPath = BINPath.Replace("/", "\\");

                //��ȡdataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                if (!CheckTable(dataset))
                {
                    MLog.Print($"{fileName}��������⣬����.", MLogType.Warning);
                    continue;
                }
                GetDataFromTable(dataset.Tables[0],
                    out string[] names, out string[] types, out object[][] data);

                bool isSucceed = CreateBIN(BINPath, fileName, data);
                if (!isSucceed) flag = false;
            }
            if (flag)
            {
                MLog.Print("����������BIN�ļ�");
                return true;
            }
            else
            {
                MLog.Print("��BIN�ļ�δ�ɹ����ɣ�����", MLogType.Warning);
                return false;
            }
        }

        public static bool CreateBIN(string BINPath, string className, object[][] data)
        {
            string CSAssemblyPath = $"{Application.dataPath}/../Library/ScriptAssemblies/Assembly-CSharp.dll";
            Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

            int rowLength = data.Length;
            int colLength = data[0].Length;
            Array instances = null;
            object resInstance = null;

            Type[] types = assembly.GetTypes();
            foreach (var type in types)//1.��������Сʵ��
            {
                if (type.Name == className)
                {
                    instances = Array.CreateInstance(type, rowLength);

                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = ctors[0];//ֻ��˽�й��캯��

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
            foreach (var type in types)//2.ͨ��Сʵ����ɴ�ʵ��
            {
                if (type.Name == $"{className}s")
                {
                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = ctors[0];//ֻ��˽�й��캯��

                    object[] parameter = new object[1] { instances };
                    resInstance = ctor.Invoke(parameter);
                }
            }

            MSerializationUtility.SaveToByte(resInstance, BINPath);
            //using (FileStream fileStream = new FileStream(BINPath, FileMode.Create))
            //{
            //    BinaryFormatter binaryFormatter = new BinaryFormatter();
            //    binaryFormatter.Serialize(fileStream, resInstance);
            //}

            return true;
        }

        /// <summary>
        /// ע�⣺û���õ�����������
        /// </summary>
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

                if (i != n - 1) res.Append(tempLine + "\n\t");
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

                if (i != n - 1) res.Append(tempLine + "\n\t");
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
                    sb.Append($"{names[i].ToUpper()} = {names[i].ToLower()};\n\t\t");
                }
                sb.Append($"{names[n - 1].ToUpper()} = {names[n - 1].ToLower()};");

                return sb.ToString();
            }
        }

        public static void GetDataFromTable(DataTable sheet, out string[] names, out string[] types, out object[][] data)
        {
            int rowCount = sheet.Rows.Count;
            int colCount = sheet.Columns.Count;

            //����ʵ������(ȥ������Ϊnone����)
            int typeCount = 0;
            for (int i = 0; i < colCount; i++)
            {
                if (sheet.Rows[2][i].ToString() == "none") continue;
                typeCount++;
            }

            //��ʼ������
            names = new string[typeCount];
            types = new string[typeCount];
            data = new object[rowCount - 3][];
            for (int i = 0; i < data.Length; i++) data[i] = new object[typeCount];
            //��ʼ������
            //names/types
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
                        MLog.Print("���ݱ��д���δ֪���ͣ�����.", MLogType.Warning);
                    }
                }
            }
        }

        private DataTable GetDefaultTable()
        {
            DataTable table = new DataTable();

            //�����ȴ��У����������
            table.Columns.Add("���");
            table.Columns.Add("����");
            table.Columns.Add("����");
            //table.Rows.Add(new object[] { "���", "����", "����" });
            table.Rows.Add(new object[] { "ID", "NAME", "DESC" });
            table.Rows.Add(new object[] { "int", "string", "string[]" });
            table.Rows.Add(new object[] { 1, "ƻ��", "��ɫ#��" });
            table.Rows.Add(new object[] { 2, "�㽶", "��ɫ#��" });
            table.Rows.Add(new object[] { 3, "����", "��ɫ#��" });

            return table;
        }

        private bool CheckAllFolder(string BINFolder, string CSFolder, List<string> fileList, CreateMode mode)
        {
            //����ļ����Ƿ���ڣ���������ھʹ���
            MPathUtility.CreateFolderIfNotExist(BINFolder);
            MPathUtility.CreateFolderIfNotExist(CSFolder);
            //ȷ���Ƿ��Ѿ����ļ��У�������ھ�ȫ����������
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
                MLog.Print($"{EditorSettings.excelGenerationPath}�з���0��Excel�ļ�������·���Ƿ���ȷ.", MLogType.Warning);
                return false;
            }
            return true;
        }

        public static bool CheckTable(DataSet dataSet)
        {
            if (dataSet.Tables.Count < 1)//�Ƿ���ڱ�
            {
                return false;
            }
            DataTable sheet = dataSet.Tables[0];//ȡ�ױ�
            //�ж����ݱ����Ƿ��������
            if (sheet.Rows.Count < 1)
            {
                return false;
            }
            return true;
        }

        #region ��������
        /// <summary>
        /// <para>.xlsxת.csv</para>
        /// ���ʧ��Ӧ��ֱ��ʹ��.xlsxת�������ļ�����
        /// </summary>
        private void XLSX2CSV()
        {
            string CSVFolder = $@"{EditorSettings.excelGenerationPath}/CSVTemp";//CSV��ʱ�ļ����λ��
            if (!Directory.Exists(CSVFolder))
            {
                Directory.CreateDirectory(CSVFolder);
            }
            //ȷ���Ƿ��Ѿ���CSV��ʱ�ļ���������ھ�ȫ����������
            bool isDelete = false;
            if (Directory.GetFiles(CSVFolder).Length != 0)
            {
                isDelete = true;
                MPathUtility.DeleteFolderFilesSurface(CSVFolder);
                Directory.CreateDirectory(CSVFolder);
            }

            List<string> fileList = MPathUtility.GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//��ȡ�����ļ���
            if (fileList.Count == 0)
            {
                MLog.Print($"{EditorSettings.excelGenerationPath}�з���0��Excel�ļ�������·���Ƿ���ȷ.", MLogType.Warning);
                return;
            }

            bool flag = true;
            //���������ļ�����CSVFolder�д�����Ӧ����ʱ�ļ�
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string CSVPath = $@"{CSVFolder}\{fileName}_Temp.csv";

                //��ȡdataset
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

                if (isDelete) MLog.Print("��������������CSV�ļ�");
                else MLog.Print("����������CSV�ļ�");
            }
        }
        private bool CreateCSV(DataSet dataSet, string CSVPath, string fileName = "")
        {
            if (dataSet.Tables.Count < 1)
            {
                MLog.Print($"CrateCSV��{fileName}û�б���ڣ�����.", MLogType.Warning);
                return false;
            }
            DataTable mSheet = dataSet.Tables[0];//ȡ�ױ�
            //�ж����ݱ����Ƿ��������
            if (mSheet.Rows.Count < 1)
            {
                MLog.Print($"CrateCSV��{fileName}�в��������ݣ�����.", MLogType.Warning);
                return false;
            }

            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;
            StringBuilder sb = new StringBuilder();
            //��ȡ����
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    //ʹ��","�ָ�ÿһ����ֵ
                    sb.Append(mSheet.Rows[i][j] + ",");
                }
                sb.Append("\r\n");//����
            }

            //д���ļ�
            using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write))
            {
                //ע�⣺ָ����UTF8��ʽ(��Excel��.csv�ļ�ʱ����������ʾ����ָ����������)
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(sb.ToString());
                }
            }

            return true;
        }
        #endregion

        #region Ԥ���ʼ����
        private const string CSBASECODE =
      @"using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class {ClassName}
{
    {PropertiesDefine}

    {ConstructorDefine}

    public static {ClassName}[] LoadBytes()
    {
        string path = @""{BINPath}"";
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
