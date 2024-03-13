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

        Weapon[] weapons = Weapon.LoadBytes();
        foreach (var weapon in weapons)
        {
            MLog.Print($"ID:{weapon.ID[0]} {weapon.ID[1]} {weapon.ID[2]} NAME:{weapon.NAME} DESC:{weapon.DESC[0]} {weapon.DESC[1]}");
        }
    }
}
