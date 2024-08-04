using System;
using UnityEngine;

namespace MFramework
{
    public class UIPanelBehaviour : UIViewBehaviour
    {
        //必须>=1
        [SerializeField] private int thickness = 10;//Panel的厚度(该Panel与下一Panel之间的sortingOrder距离)
        public int Thickness { get { return thickness; } }

        //集中模式---开启时，点击以及Panel的操作会将其置顶
        [SerializeField] private UIPanelFocusMode focusMode = UIPanelFocusMode.Disabled;
        public UIPanelFocusMode FocusMode { get { return focusMode; } }
    }
}