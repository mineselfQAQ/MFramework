namespace MFramework
{
    //=====UIPanel=====
    public enum UIPanelShowState
    {
        Off,
        On
    }

    public enum UIPanelAnimState
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

    public enum UIPanelAnimSwitch
    {
        Disabled,
        Enabled
    }

    public enum UIPanelOpenAnimMode
    {
        AutoPlay,
        SelfControl
    }
    public enum UIPanelCloseAnimMode
    {
        AutoPlay,
        SelfControl
    }
}

