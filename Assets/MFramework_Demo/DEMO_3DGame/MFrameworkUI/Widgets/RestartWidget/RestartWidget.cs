using UnityEngine;
using UnityEngine.UI;

public class RestartWidget : RestartWidgetBase
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
        m_RetriesNum_MText.text = m_game.retries.ToString(m_controller.HUDRetriesFormat);
    }
}