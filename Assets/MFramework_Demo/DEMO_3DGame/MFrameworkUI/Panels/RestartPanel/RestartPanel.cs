using UnityEngine;

public class RestartPanel : RestartPanelBase
{
    protected Game m_game => Game.Instance;
    protected UIController m_controller => UIController.Instance;

    public override void Init()
    {
        m_OldRetriesNum_MText.text = m_game.retries.ToString(m_controller.HUDRetriesFormat);
        m_NewRetriesNum_MText.text = (m_game.retries - 1).ToString(m_controller.HUDRetriesFormat);
    }

    public void Refresh()
    {
        m_OldRetriesNum_MText.text = m_game.retries.ToString(m_controller.HUDRetriesFormat);
        m_NewRetriesNum_MText.text = (m_game.retries - 1).ToString(m_controller.HUDRetriesFormat);
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadSync(prefabPath);
    }
}