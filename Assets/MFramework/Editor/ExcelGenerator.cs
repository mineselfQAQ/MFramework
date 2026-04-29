using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UnityEditor;
using UnityEngine;

namespace MFramework.Editor.Excel
{
    public sealed class ExcelGenerator : EditorWindow
    {
        public const string GeneratedNamespace = "MFramework.Text.Generated";

        private const string DelayPrefix = "MFramework.ExcelGenerator.Delay.";
        private const string DelayPendingKey = DelayPrefix + "Pending";
        private const string DelayInputFolderKey = DelayPrefix + "InputFolder";
        private const string DelayByteFolderKey = DelayPrefix + "ByteFolder";

        private const string FixedExcelFolderRelative = "Core/Excel";
        private const string FixedCsOutputFolderRelative = "Assets/MFramework/Module/MText/Generated";
        private const string FixedByteOutputFolderRelative = "Assets/StreamingAssets";

        private string _templateOutputName = "ExcelTemplate.xlsx";

        [InitializeOnLoadMethod]
        private static void InitializeDelayGenerateByte()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static void OnAfterAssemblyReload()
        {
            EditorApplication.delayCall -= RunDelayedByteGeneration;
            EditorApplication.delayCall += RunDelayedByteGeneration;
        }

        [MenuItem("MFramework/Excel Generator", priority = 201)]
        public static void Open()
        {
            var window = GetWindow<ExcelGenerator>(true, "Excel Generator", true);
            window.minSize = new Vector2(560f, 420f);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Excel Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(6f);

            DrawGenerateExcelPart();
            EditorGUILayout.Space(8f);
            DrawPersistentDataPart();
        }

        public static void WriteXlsx(string path, IList<string[]> rows)
        {
            string fullPath = ToFullPath(path);
            EnsureDirectory(fullPath);

            FileInfo file = new FileInfo(fullPath);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(fullPath);
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                for (int row = 0; row < rows.Count; row++)
                {
                    string[] values = rows[row];
                    for (int column = 0; column < values.Length; column++)
                    {
                        worksheet.Cells[row + 1, column + 1].Value = values[column];
                    }
                }

                if (rows.Count > 0)
                {
                    int maxColumns = rows.Max(r => r.Length);
                    worksheet.Cells[1, 1, rows.Count, maxColumns].AutoFitColumns();
                    worksheet.Cells[1, 1, rows.Count, maxColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1, 1, maxColumns].Style.Font.Bold = true;
                }

                package.Save();
            }
        }

        public static List<string[]> CreateDefaultRows()
        {
            return new List<string[]>
            {
                new[] { "ID", "Name", "Description" },
                new[] { "ID", "NAME", "DESC" },
                new[] { "int", "string", "string[]" },
                new[] { "1", "Apple", "red#sweet" },
                new[] { "2", "Banana", "yellow#sweet" },
                new[] { "3", "Orange", "orange#sour" },
            };
        }

        public static bool CreateSingleCS(string excelPath, string csPath, string binLoadPath, string namespaceName = "")
        {
            if (!TryReadPersistentData(excelPath, out string[] names, out string[] types, out _))
            {
                return false;
            }

            return CreateCS(csPath, binLoadPath, names, types, namespaceName);
        }

        public static bool CreateSingleBIN(string excelPath, string binPath, string className = null, string namespaceName = "")
        {
            if (!TryReadPersistentData(excelPath, out _, out _, out object[][] data))
            {
                return false;
            }

            string resolvedClassName = string.IsNullOrWhiteSpace(className)
                ? Path.GetFileNameWithoutExtension(excelPath)
                : className;
            return CreateBIN(binPath, resolvedClassName, data, namespaceName);
        }

        public static bool CreateAllCS(string inputFolder, string csOutputFolder, string byteOutputFolder)
        {
            List<string> excelPaths = GetExcelFiles(inputFolder);
            if (excelPaths.Count == 0)
            {
                Debug.LogWarning($"No xlsx files found in folder: {inputFolder}");
                return false;
            }

            bool success = true;
            foreach (string excelPath in excelPaths)
            {
                string className = Path.GetFileNameWithoutExtension(excelPath);
                string csPath = CombineProjectPath(csOutputFolder, $"{className}.cs");
                string bytePath = CombineProjectPath(byteOutputFolder, $"{className}.byte");
                string runtimeBytePath = BuildRuntimeBytePath(bytePath);
                success &= CreateSingleCS(excelPath, csPath, runtimeBytePath, GeneratedNamespace);
            }

            return success;
        }

        public static bool CreateAllBIN(string inputFolder, string byteOutputFolder)
        {
            List<string> excelPaths = GetExcelFiles(inputFolder);
            if (excelPaths.Count == 0)
            {
                Debug.LogWarning($"No xlsx files found in folder: {inputFolder}");
                return false;
            }

            bool success = true;
            foreach (string excelPath in excelPaths)
            {
                string className = Path.GetFileNameWithoutExtension(excelPath);
                string bytePath = CombineProjectPath(byteOutputFolder, $"{className}.byte");
                success &= CreateSingleBIN(excelPath, bytePath, className, GeneratedNamespace);
            }

            return success;
        }

        public static bool CreateAllCSAndDelayBIN(string inputFolder, string csOutputFolder, string byteOutputFolder)
        {
            bool success = CreateAllCS(inputFolder, csOutputFolder, byteOutputFolder);
            if (!success) return false;

            EditorPrefs.SetBool(DelayPendingKey, true);
            EditorPrefs.SetString(DelayInputFolderKey, inputFolder ?? string.Empty);
            EditorPrefs.SetString(DelayByteFolderKey, byteOutputFolder ?? string.Empty);

            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            return true;
        }

        private void DrawGenerateExcelPart()
        {
            EditorGUILayout.LabelField("Generate Excel File", EditorStyles.boldLabel);
            _templateOutputName = EditorGUILayout.TextField("Name", _templateOutputName);
            DrawReadOnlyPathField("Output", GetFixedExcelFolder());

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create Default XLSX", GUILayout.Height(30f)))
                {
                    CreateDefaultXlsx();
                }
            }
        }

        private void DrawPersistentDataPart()
        {
            EditorGUILayout.LabelField("Generate Persistent Data", EditorStyles.boldLabel);
            DrawReadOnlyPathField("Input Folder", GetFixedExcelFolder());
            DrawReadOnlyPathField("CS Output Folder", GetFixedCsOutputFolder());
            DrawReadOnlyPathField("BYTE Output Folder", GetFixedByteOutputFolder());

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("1. Create .cs Files", GUILayout.Height(30f)))
                {
                    GenerateAllCSFromWindow();
                }

                if (GUILayout.Button("2. Create .byte Files", GUILayout.Height(30f)))
                {
                    GenerateAllByteFromWindow();
                }
            }

            if (GUILayout.Button("Generate .cs + .byte Files", GUILayout.Height(30f)))
            {
                GenerateAllCSAndByteFromWindow();
            }

            EditorGUILayout.HelpBox("All .xlsx files in Input Folder are generated. Class names and output file names come from each Excel file name. Namespace is fixed to MFramework.Text.Generated.", MessageType.Info);
        }

        private void GenerateAllCSFromWindow()
        {
            string csOutputFolder = GetFixedCsOutputFolder();
            bool success = CreateAllCS(GetFixedExcelFolder(), csOutputFolder, GetFixedByteOutputFolder());
            if (!success)
            {
                EditorUtility.DisplayDialog("Generate CS Failed", "No valid Excel files were generated. Check paths and console logs.", "OK");
                return;
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Generate CS Complete", $"Generated .cs files into:\n{csOutputFolder}", "OK");
        }

        private void GenerateAllByteFromWindow()
        {
            string byteOutputFolder = GetFixedByteOutputFolder();
            bool success = CreateAllBIN(GetFixedExcelFolder(), byteOutputFolder);
            if (!success)
            {
                EditorUtility.DisplayDialog("Generate BYTE Failed", "Generated CS must be compiled before creating BYTE. Check paths and console logs.", "OK");
                return;
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Generate BYTE Complete", $"Generated .byte files into:\n{byteOutputFolder}", "OK");
        }

        private void GenerateAllCSAndByteFromWindow()
        {
            string csOutputFolder = GetFixedCsOutputFolder();
            string byteOutputFolder = GetFixedByteOutputFolder();
            bool success = CreateAllCSAndDelayBIN(GetFixedExcelFolder(), csOutputFolder, byteOutputFolder);
            if (!success)
            {
                EditorUtility.DisplayDialog("Generate Failed", "No valid Excel files were generated. Check paths and console logs.", "OK");
                return;
            }

            EditorUtility.DisplayDialog("Generate Started", $".cs files were generated into:\n{csOutputFolder}\n\n.byte files will be generated after Unity compiles into:\n{byteOutputFolder}", "OK");
        }

        private static void DrawReadOnlyPathField(string label, string pathValue)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField(label, pathValue);
            }
        }

        private static void RunDelayedByteGeneration()
        {
            if (!EditorPrefs.GetBool(DelayPendingKey, false)) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall -= RunDelayedByteGeneration;
                EditorApplication.delayCall += RunDelayedByteGeneration;
                return;
            }

            EditorPrefs.SetBool(DelayPendingKey, false);

            string inputFolder = EditorPrefs.GetString(DelayInputFolderKey, string.Empty);
            string byteFolder = EditorPrefs.GetString(DelayByteFolderKey, string.Empty);

            if (string.IsNullOrWhiteSpace(inputFolder) || string.IsNullOrWhiteSpace(byteFolder))
            {
                Debug.LogError("Excel delayed BYTE generation failed: saved task is incomplete.");
                return;
            }

            bool success = CreateAllBIN(inputFolder, byteFolder);
            if (success)
            {
                AssetDatabase.Refresh();
                Debug.Log($"Excel delayed BYTE generated into: {byteFolder}");
            }
        }

        public static bool TryReadPersistentData(string excelPath, out string[] names, out string[] types, out object[][] data)
        {
            names = Array.Empty<string>();
            types = Array.Empty<string>();
            data = Array.Empty<object[]>();

            string fullPath = ToFullPath(excelPath);
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Excel file does not exist: {excelPath}");
                return false;
            }

            using FileStream stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = excelReader.AsDataSet();
            if (!CheckTable(dataSet))
            {
                Debug.LogError($"Excel table is invalid: {excelPath}");
                return false;
            }

            GetDataFromTable(dataSet.Tables[0], out names, out types, out data);
            return true;
        }

        public static void GetDataFromTable(DataTable sheet, out string[] names, out string[] types, out object[][] data)
        {
            if (sheet.Rows.Count < 3)
            {
                throw new InvalidOperationException("Excel persistent-data format requires at least 3 rows: display names, field names, field types.");
            }

            int rowCount = sheet.Rows.Count;
            int columnCount = sheet.Columns.Count;
            var validColumns = new List<int>();

            for (int column = 0; column < columnCount; column++)
            {
                string type = sheet.Rows[2][column]?.ToString();
                if (string.IsNullOrWhiteSpace(type) || string.Equals(type, "none", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                validColumns.Add(column);
            }

            names = new string[validColumns.Count];
            types = new string[validColumns.Count];
            data = new object[Mathf.Max(0, rowCount - 3)][];

            for (int row = 0; row < data.Length; row++)
            {
                data[row] = new object[validColumns.Count];
            }

            for (int i = 0; i < validColumns.Count; i++)
            {
                int column = validColumns[i];
                names[i] = sheet.Rows[1][column].ToString();
                types[i] = sheet.Rows[2][column].ToString();
            }

            for (int row = 3; row < rowCount; row++)
            {
                for (int i = 0; i < validColumns.Count; i++)
                {
                    int column = validColumns[i];
                    data[row - 3][i] = ConvertCell(sheet.Rows[row][column]?.ToString() ?? string.Empty, types[i]);
                }
            }
        }

        public static bool CheckTable(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count < 1) return false;
            return dataSet.Tables[0].Rows.Count >= 3;
        }

        private void CreateDefaultXlsx()
        {
            WriteXlsx(GetTemplateOutputPath(), CreateDefaultRows());
            AssetDatabase.Refresh();
        }

        private string GetTemplateOutputPath()
        {
            string fileName = string.IsNullOrWhiteSpace(_templateOutputName)
                ? "ExcelTemplate"
                : Path.GetFileNameWithoutExtension(_templateOutputName.Trim());
            return CombineProjectPath(GetFixedExcelFolder(), $"{fileName}.xlsx");
        }

        private static string GetFixedExcelFolder()
        {
            return CombineProjectPath(GetProjectRootPath(), FixedExcelFolderRelative);
        }

        private static string GetFixedCsOutputFolder()
        {
            return CombineProjectPath(GetProjectRootPath(), FixedCsOutputFolderRelative);
        }

        private static string GetFixedByteOutputFolder()
        {
            return CombineProjectPath(GetProjectRootPath(), FixedByteOutputFolderRelative);
        }

        private static string GetProjectRootPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace('\\', '/');
        }

        private static bool CreateCS(string csPath, string binLoadPath, string[] names, string[] types, string namespaceName)
        {
            string fullPath = ToFullPath(csPath);
            EnsureDirectory(fullPath);

            bool hasNamespace = !string.IsNullOrWhiteSpace(namespaceName);
            string code = hasNamespace ? CsBaseCodeWithNamespace : CsBaseCode;
            string className = Path.GetFileNameWithoutExtension(csPath);
            string collectionClassName = $"{className}s";

            code = code.Replace("{NameSpace}", namespaceName ?? string.Empty);
            code = code.Replace("{ClassName}", className);
            code = code.Replace("{CollectionClassName}", collectionClassName);
            code = code.Replace("{PropertiesDefine}", GeneratePropertiesDefine(names, types, hasNamespace));
            code = code.Replace("{ConstructorDefine}", GenerateConstructorDefine(className, names, types, hasNamespace));
            code = code.Replace("{BINPath}", EscapeForGeneratedString(binLoadPath));

            File.WriteAllText(fullPath, code, new UTF8Encoding(false));
            return true;
        }

        private static bool CreateBIN(string binPath, string className, object[][] data, string namespaceName)
        {
            string fullPath = ToFullPath(binPath);
            EnsureDirectory(fullPath);

            Type itemType = FindGeneratedType(className, namespaceName);
            if (itemType == null)
            {
                Debug.LogError($"Generated Excel type not found. Compile generated CS first: {className}");
                return false;
            }

            Type collectionType = FindGeneratedType($"{className}s", namespaceName);
            if (collectionType == null)
            {
                Debug.LogError($"Generated Excel collection type not found. Compile generated CS first: {className}s");
                return false;
            }

            Array instances = Array.CreateInstance(itemType, data.Length);
            ConstructorInfo itemCtor = itemType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
            if (itemCtor == null)
            {
                Debug.LogError($"Generated Excel type has no non-public constructor: {itemType.FullName}");
                return false;
            }

            for (int i = 0; i < data.Length; i++)
            {
                object instance = itemCtor.Invoke(data[i]);
                instances.SetValue(instance, i);
            }

            ConstructorInfo collectionCtor = collectionType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
            if (collectionCtor == null)
            {
                Debug.LogError($"Generated Excel collection type has no non-public constructor: {collectionType.FullName}");
                return false;
            }

            object collection = collectionCtor.Invoke(new object[] { instances });
            using (FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, collection);
            }
            return true;
        }

        private static Type FindGeneratedType(string className, string namespaceName)
        {
            string fullName = string.IsNullOrWhiteSpace(namespaceName) ? className : $"{namespaceName}.{className}";
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);
                if (type != null) return type;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
                if (type != null) return type;
            }

            return null;
        }

        private static object ConvertCell(string value, string type)
        {
            switch (type)
            {
                case "byte": return Convert.ToByte(value);
                case "short": return Convert.ToInt16(value);
                case "int": return Convert.ToInt32(value);
                case "long": return Convert.ToInt64(value);
                case "float": return Convert.ToSingle(value);
                case "double": return Convert.ToDouble(value);
                case "bool": return Convert.ToBoolean(value);
                case "char": return Convert.ToChar(value);
                case "string": return value;
                case "byte[]": return SplitArray(value, Convert.ToByte);
                case "short[]": return SplitArray(value, Convert.ToInt16);
                case "int[]": return SplitArray(value, Convert.ToInt32);
                case "long[]": return SplitArray(value, Convert.ToInt64);
                case "float[]": return SplitArray(value, Convert.ToSingle);
                case "double[]": return SplitArray(value, Convert.ToDouble);
                case "char[]": return SplitArray(value, Convert.ToChar);
                case "string[]": return string.IsNullOrEmpty(value) ? Array.Empty<string>() : value.Split('#');
                default:
                    throw new InvalidOperationException($"Unknown Excel field type: {type}");
            }
        }

        private static T[] SplitArray<T>(string value, Func<string, T> convert)
        {
            if (string.IsNullOrEmpty(value)) return Array.Empty<T>();

            string[] parts = value.Split('#');
            T[] result = new T[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                result[i] = convert(parts[i]);
            }

            return result;
        }

        private static string GeneratePropertiesDefine(string[] names, string[] types, bool hasNamespace)
        {
            var builder = new StringBuilder();
            string indent = hasNamespace ? "        " : "    ";

            for (int i = 0; i < names.Length; i++)
            {
                builder.Append(indent);
                builder.Append("public ");
                builder.Append(types[i]);
                builder.Append(' ');
                builder.Append(names[i].ToUpperInvariant());
                builder.Append(" { get; private set; }");
                if (i < names.Length - 1)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        private static string GenerateConstructorDefine(string className, string[] names, string[] types, bool hasNamespace)
        {
            string indent = hasNamespace ? "        " : "    ";
            string innerIndent = hasNamespace ? "            " : "        ";
            var parameters = new StringBuilder();
            var assignments = new StringBuilder();

            for (int i = 0; i < names.Length; i++)
            {
                string parameterName = names[i].ToLowerInvariant();
                parameters.Append(types[i]).Append(' ').Append(parameterName);
                assignments.Append(innerIndent)
                    .Append(names[i].ToUpperInvariant())
                    .Append(" = ")
                    .Append(parameterName)
                    .Append(';');

                if (i < names.Length - 1)
                {
                    parameters.Append(", ");
                    assignments.AppendLine();
                }
            }

            return $"{indent}private {className}({parameters})\n{indent}{{\n{assignments}\n{indent}}}";
        }

        private static string EscapeForGeneratedString(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string ToProjectPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;
            string fullAssetsPath = Path.GetFullPath(Application.dataPath).Replace('\\', '/');
            string fullPath = Path.GetFullPath(path).Replace('\\', '/');
            if (fullPath.StartsWith(fullAssetsPath, StringComparison.OrdinalIgnoreCase))
            {
                return "Assets" + fullPath.Substring(fullAssetsPath.Length);
            }

            return path;
        }

        public static string ToFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;
            if (Path.IsPathRooted(path)) return path;
            return Path.GetFullPath(path);
        }

        private static void EnsureDirectory(string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static List<string> GetExcelFiles(string inputFolder)
        {
            string fullFolder = ToFullPath(inputFolder);
            if (!Directory.Exists(fullFolder)) return new List<string>();

            return Directory.GetFiles(fullFolder, "*.xlsx", SearchOption.TopDirectoryOnly)
                .Where(path => !Path.GetFileName(path).StartsWith("~$", StringComparison.Ordinal))
                .Select(ToProjectPath)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string CombineProjectPath(string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folder)) return fileName;
            return $"{folder.TrimEnd('/', '\\')}/{fileName}".Replace('\\', '/');
        }

        private static string BuildRuntimeBytePath(string bytePath)
        {
            string normalized = ToProjectPath(bytePath).Replace('\\', '/');
            const string streamingPrefix = "Assets/StreamingAssets";
            if (normalized.StartsWith(streamingPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string relative = normalized.Substring(streamingPrefix.Length).TrimStart('/');
                return string.IsNullOrWhiteSpace(relative)
                    ? "{Application.streamingAssetsPath}"
                    : "{Application.streamingAssetsPath}/" + relative;
            }

            return normalized;
        }

        private const string CsBaseCodeWithNamespace =
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
                return table?.items;
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

        private const string CsBaseCode =
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
            return table?.items;
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
    }

}
