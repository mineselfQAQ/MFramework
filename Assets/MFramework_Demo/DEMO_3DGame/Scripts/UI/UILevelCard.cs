using MFramework;
using UnityEngine;

/// <summary>
/// UI组件---关卡卡，用于选择关卡(基于存档)
/// </summary>
public class UILevelCard : ComponentSingleton<UILevelCard>
{
    [Header("List")]
    public bool focusFirstElement = true;
}
