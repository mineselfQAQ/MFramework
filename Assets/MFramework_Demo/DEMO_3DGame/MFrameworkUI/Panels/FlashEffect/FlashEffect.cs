using MFramework;
using System;
using UnityEngine;

public class FlashEffect : FlashEffectBase
{
    public void Flash(float duration, Action onFinish)
    {
        var origionColor = m_FlashEffect_Image.color;
        origionColor.a = 1;
        m_FlashEffect_Image.color = origionColor;//设置初始alpha为1

        //alpha---[1,0]
        MTween.UnscaledDoTween01NoRecord((f) =>
        {
            origionColor.a = Mathf.Lerp(1, 0, f);
            m_FlashEffect_Image.color = origionColor;
        }, MCurve.Linear, duration, onFinish);
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadPanelSync(prefabPath);
    }
}