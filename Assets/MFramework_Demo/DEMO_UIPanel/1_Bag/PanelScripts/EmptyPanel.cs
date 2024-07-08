using MFramework;

public class EmptyPanel : EmptyPanelBase
{
    protected override void OnFocusChanged(bool focus)
    {
        MLog.Print($"{focus}");
    }
}
