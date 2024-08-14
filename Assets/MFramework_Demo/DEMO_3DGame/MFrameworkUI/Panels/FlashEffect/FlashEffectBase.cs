using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashEffectBase : UIPanel
{
    protected Image m_FlashEffect_Image;

    protected override void OnBindCompsAndEvents()
    {
        m_FlashEffect_Image = (Image)viewBehaviour.GetComp(0, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_FlashEffect_Image = null;
    }
}