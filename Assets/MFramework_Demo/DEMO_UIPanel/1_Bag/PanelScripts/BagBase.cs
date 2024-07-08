using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagBase : UIPanel
{
    protected RectTransform m_Bag_RectTransform;
	protected Image m_Background_Image;

    protected override void OnBindCompsAndEvents()
    {
        m_Bag_RectTransform = (RectTransform)viewBehaviour.GetComp(0, 0);
		m_Background_Image = (Image)viewBehaviour.GetComp(1, 0);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        m_Bag_RectTransform = null;
		m_Background_Image = null;
    }
}