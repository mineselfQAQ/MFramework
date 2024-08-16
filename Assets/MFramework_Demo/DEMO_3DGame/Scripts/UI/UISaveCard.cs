using MFramework;
using UnityEngine;

/// <summary>
/// UI组件---保存卡，用于读取存档
/// </summary>
public class UISaveCard : ComponentSingleton<UISaveCard>
{
    [Header("List")]
    public bool focusFirstElement = true;

    [Header("Text Formatting")]
    public string retriesFormat = "00";
    public string starsFormat = "00";
    public string coinsFormat = "000";
    public string dateFormat = "MM/dd/y hh:mm";
}
