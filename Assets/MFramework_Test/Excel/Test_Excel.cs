using Excel;
using MFramework;
using OfficeOpenXml;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Test_Excel : MonoBehaviour
{
    private static string ms_FileName = "TestTable.xlsx";
    private static string ms_SheetName = "TestSheet";

    private void Start()
    {
        //组合完整路径
        string path = GetCurrentScriptPath();
        path = Path.Combine(path, ms_FileName);

        //创建Excel
        CreateExcel(path);

        //读取Excel
        DataRowCollection data = ReadExcel(path, ms_SheetName);
        for (int i = 1; i < data.Count; i++)
        {
           MLog.Print($"第{i}行: {data[i][0]} {data[i][1]} {data[i][2]}");
        }

        //改写Excel
        UpdateExcel(path);

        System.Diagnostics.Process.Start(path);//直接打开.xlsx文件
    }

    public void CreateExcel(string path)
    {
        //创建fileInfo并保证是全新的
        //Tip：FileInfo的实例化并不会创建出文件，只有当写入数据并保存时才会创建
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            MLog.Print($"{ms_FileName}文件已存在，将进行删除后重新创建操作.");
            file.Delete();
            file = new FileInfo(path);
        }

        //填充数据
        using (ExcelPackage package = new ExcelPackage(file))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(ms_SheetName);

            worksheet.Cells[1, 1].Value = "参数1";
            worksheet.Cells[1, 2].Value = "参数2";
            worksheet.Cells[1, 3].Value = "参数3";

            worksheet.Cells[2, 1].Value = "参数1(1)";
            worksheet.Cells[2, 2].Value = "参数2(1)";
            worksheet.Cells[2, 3].Value = "参数3(1)";
            worksheet.Cells[3, 1].Value = "参数1(2)";
            worksheet.Cells[3, 2].Value = "参数2(2)";
            worksheet.Cells[3, 3].Value = "参数3(2)";

            package.Save();
        }

        AssetDatabase.Refresh();
    }

    public DataRowCollection ReadExcel(string path, int sheetIndex)
    {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result.Tables[sheetIndex].Rows;
    }
    public DataRowCollection ReadExcel(string path, string sheetName)
    {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result.Tables[sheetName].Rows;
    }

    public void UpdateExcel(string path)
    {
        FileInfo file = new FileInfo(path);
        if (!file.Exists) MLog.Print("UpdateExcel---未找到excel文件");

        //填充数据
        using (ExcelPackage package = new ExcelPackage(file))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[ms_SheetName];
            worksheet.Cells[4, 1].Value = "参数1(3新增)";
            worksheet.Cells[4, 2].Value = "参数2(3新增)";
            worksheet.Cells[4, 3].Value = "参数3(3新增)";

            package.Save();
        }
    }

    /// <summary>
    /// 获取当前脚本所在文件夹路径
    /// </summary>
    private string GetCurrentScriptPath()
    {
        string[] guids = AssetDatabase.FindAssets(this.name);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null)
            {
                string resPath = AssetDatabase.GetAssetPath(script);
                resPath = Path.GetDirectoryName(resPath);
                resPath = Path.GetFullPath(resPath);
                return resPath;
            }
        }
        return null;
    }
}
