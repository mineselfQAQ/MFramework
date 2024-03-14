using MFramework;
using Table;
using UnityEngine;

public class Test_ExcelGenerator : MonoBehaviour
{
    private void Start()
    {
        Sheet[] sheets = Sheet.LoadBytes();
        foreach (var sheet in sheets)
        {
            MLog.Print($"ID:{sheet.ID} NAME:{sheet.NAME} DESC:{sheet.DESC[0]} {sheet.DESC[1]}");
        }
    }
}
