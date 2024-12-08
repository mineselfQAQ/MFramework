using UnityEngine;

public class TakeBloodRestartWidget : TakeBloodRestartWidgetBase
{
    protected Game m_game => Game.Instance;
    protected UIController m_controller => UIController.Instance;

    public override void Init()
    {
        
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }

    public void Refresh()
    {
        m_OldRetriesNum_MText.text = m_game.retries.ToString(m_controller.HUDRetriesFormat);
        m_NewRetriesNum_MText.text = (m_game.retries - 1).ToString(m_controller.HUDRetriesFormat);
    }
}