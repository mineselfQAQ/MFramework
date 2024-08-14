using MFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffect : FlashEffectBase
{
    public override void Init()
    {
        
    }

    public override void Update()
    {
        
    }

    protected override void OnClicked(Button button) 
    {
        
    }

    public void Flash(float duration, Action onFinish)
    {
        var origionColor = m_FlashEffect_Image.color;
        origionColor.a = 1;
        m_FlashEffect_Image.color = origionColor;//设置初始alpha为1

        //alpha---[1,0]
        MTween.DoTween01NoRecord((f) =>
        {
            origionColor.a = Mathf.Lerp(1, 0, f);
            m_FlashEffect_Image.color = origionColor;
        }, MCurve.Linear, duration, onFinish);
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}