using UnityEngine;
using MFramework;

public class TestPanel : TestPanelBase
{
    protected override void OnCreating()
    {
        Debug.Log("OnCreating");
    }

    protected override void OnCreated()
    {
        Debug.Log("OnCreated");
    }
}
