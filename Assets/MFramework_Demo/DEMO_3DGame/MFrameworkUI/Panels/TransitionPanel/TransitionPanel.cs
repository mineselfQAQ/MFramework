using System;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanel : TransitionPanelBase
{
    public override void Init()
    {
        CreateWidget<TakeBloodRestartWidget>(m_TakeBloodRestartWidget_UIWidgetBehaviour);
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadSync(prefabPath);
    }

    public void OpenTakeBloodRestartWidget(Action onFinish = null)
    {
        GetWidget<TakeBloodRestartWidget>().Refresh();
        OpenWidget<TakeBloodRestartWidget>(onFinish);
    }
    public void CloseTakeBloodRestartWidget(Action onFinish = null)
    {
        CloseWidget<TakeBloodRestartWidget>(onFinish);
    }
}