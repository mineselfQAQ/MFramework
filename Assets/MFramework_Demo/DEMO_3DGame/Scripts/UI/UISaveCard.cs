using MFramework;
using UnityEngine;

/// <summary>
/// UI���---���濨�����ڶ�ȡ�浵
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
