#if UNITY_EDITOR
using Excel;
using MFramework;
using OfficeOpenXml;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Test_Excel : MonoBehaviour
{
    private static string ms_FileName = "TestTable.xlsx";
    private static string ms_SheetName = "TestSheet";

    private void Start()
    {
        //�������·��
        string path = GetCurrentScriptPath();
        path = Path.Combine(path, ms_FileName);

        //����Excel
        CreateExcel(path);

        //��ȡExcel
        DataRowCollection data = ReadExcel(path, ms_SheetName);
        for (int i = 1; i < data.Count; i++)
        {
           MLog.Print($"��{i}��: {data[i][0]} {data[i][1]} {data[i][2]}");
        }

        //��дExcel
        UpdateExcel(path);

        System.Diagnostics.Process.Start(path);//ֱ�Ӵ�.xlsx�ļ�
    }

    public void CreateExcel(string path)
    {
        //����fileInfo����֤��ȫ�µ�
        //Tip��FileInfo��ʵ���������ᴴ�����ļ���ֻ�е�д�����ݲ�����ʱ�Żᴴ��
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            MLog.Print($"{ms_FileName}�ļ��Ѵ��ڣ�������ɾ�������´�������.");
            file.Delete();
            file = new FileInfo(path);
        }

        //�������
        using (ExcelPackage package = new ExcelPackage(file))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(ms_SheetName);

            worksheet.Cells[1, 1].Value = "����1";
            worksheet.Cells[1, 2].Value = "����2";
            worksheet.Cells[1, 3].Value = "����3";

            worksheet.Cells[2, 1].Value = "����1(1)";
            worksheet.Cells[2, 2].Value = "����2(1)";
            worksheet.Cells[2, 3].Value = "����3(1)";
            worksheet.Cells[3, 1].Value = "����1(2)";
            worksheet.Cells[3, 2].Value = "����2(2)";
            worksheet.Cells[3, 3].Value = "����3(2)";

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
        if (!file.Exists) MLog.Print("UpdateExcel��δ�ҵ�excel�ļ�");

        //�������
        using (ExcelPackage package = new ExcelPackage(file))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[ms_SheetName];
            worksheet.Cells[4, 1].Value = "����1(3����)";
            worksheet.Cells[4, 2].Value = "����2(3����)";
            worksheet.Cells[4, 3].Value = "����3(3����)";

            package.Save();
        }
    }

    /// <summary>
    /// ��ȡ��ǰ�ű������ļ���·��
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
#endif