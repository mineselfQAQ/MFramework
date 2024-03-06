using Excel;
using OfficeOpenXml;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Test_Excel : MonoBehaviour
{
    private void Start()
    {
        //组合完整路径
        string path = GetCurrentScriptPath();
        string fileName = "TestTable.xlsx";
        path = Path.Combine(path, fileName);

        //创建fileInfo并保证是全新的
        //Tip：FileInfo的实例化并不会创建出文件，只有当写入数据并保存时才会创建
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            file.Delete();
            file = new FileInfo(path);
        }

        using (ExcelPackage package = new ExcelPackage(file))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("TestSheet");

            worksheet.Cells[1, 1].Value = "参数1";
            worksheet.Cells[1, 2].Value = "参数2";
            worksheet.Cells[1, 3].Value = "参数3";

            worksheet.Cells[2, 1].Value = "No.1值";
            worksheet.Cells[2, 2].Value = "No.1值";
            worksheet.Cells[2, 3].Value = "No.1值";
            worksheet.Cells[3, 1].Value = "No.2值";
            worksheet.Cells[3, 2].Value = "No.2值";
            worksheet.Cells[3, 3].Value = "No.2值";

            package.Save();
        }

        AssetDatabase.Refresh();
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
