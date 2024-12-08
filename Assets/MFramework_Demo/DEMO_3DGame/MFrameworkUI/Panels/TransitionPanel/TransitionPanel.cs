using System;
using UnityEngine;

public class TransitionPanel : TransitionPanelBase
{
    public override void Init()
    {
        CreateWidget<TakeBloodRestartWidget>(m_TakeBloodRestartWidget_UIWidgetBehaviour);
        CreateWidget<RestartWidget>(m_RestartWidget_UIWidgetBehaviour);
        CreateWidget<GameOverWidget>(m_GameOverWidget_UIWidgetBehaviour);
        CreateWidget<LoadingWidget>(m_LoadingWidget_UIWidgetBehaviour);
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
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

    public void OpenRestartWidget(Action onFinish = null)
    {
        GetWidget<RestartWidget>().Refresh();
        OpenWidget<RestartWidget>(onFinish);
    }
    public void CloseRestartWidget(Action onFinish = null)
    {
        CloseWidget<RestartWidget>(onFinish);
    }

    public void OpenGameOverWidget(Action onFinish = null)
    {
        OpenWidget<GameOverWidget>(onFinish);
    }
    public void CloseGameOverWidget(Action onFinish = null)
    {
        CloseWidget<GameOverWidget>(onFinish);
    }

    public void OpenLoadingWidget(Action onFinish = null)
    {
        OpenWidget<LoadingWidget>(onFinish);
    }
    public void CloseLoadingWidget(Action onFinish = null)
    {
        CloseWidget<LoadingWidget>(onFinish);
    }
}