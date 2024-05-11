using MFramework;
using UnityEngine.UI;

public class TestPanel : TestPanelBase
{
    protected override void OnCreating()
    {
        MLog.Print("OnCreating");
    }

    protected override void OnCreated()
    {
        MLog.Print("OnCreated");
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_Button1_Button)
        {
            MLog.Print("Button1");
        }
        else if (button == m_Button2_Button)
        {
            MLog.Print("Button2");
            DestroySelf();
        }
    }
}
