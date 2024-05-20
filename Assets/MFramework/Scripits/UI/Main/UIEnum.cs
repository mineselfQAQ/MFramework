namespace MFramework
{
    //=====UIWidget=====
    public enum UIWidgetMode
    {
        Simple,
        Advanced
    }

    //=====UIPanel=====
    public enum UIShowState
    {
        None,
        Off,
        On
    }

    public enum UIAnimState
    {
        Idle,//³õÊ¼×´Ì¬
        Opening,
        Opened,
        Closing,
        Closed
    }

    //=====UIPanelBehaviour=====
    public enum UIPanelFocusMode
    {
        Disabled,
        Enabled
    }

    public enum UIAnimSwitch
    {
        Off,
        On
    }

    public enum UIOpenAnimMode
    {
        AutoPlay,
        SelfControl
    }
    public enum UICloseAnimMode
    {
        AutoPlay,
        SelfControl
    }
}

