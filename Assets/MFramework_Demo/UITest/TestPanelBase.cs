using MFramework;
using UnityEngine.UI;

public class TestPanelBase : UIPanel
{
    protected Button m_Button1_Button;
    protected Button m_Button2_Button;

    protected override void OnBindCompsAndEvents()
    {
        m_Button1_Button = (Button)viewBehaviour.GetComp(0, 0);
        m_Button2_Button = (Button)viewBehaviour.GetComp(1, 0);

        BindEvent(m_Button1_Button);
        BindEvent(m_Button2_Button);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_Button1_Button);
        UnbindEvent(m_Button2_Button);

        m_Button1_Button = null;
        m_Button2_Button = null;
    }
}
