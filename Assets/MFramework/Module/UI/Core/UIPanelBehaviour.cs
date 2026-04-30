using UnityEngine;

namespace MFramework.UI
{
    public class UIPanelBehaviour : UIViewBehaviour
    {
        [SerializeField] private int thickness = 10;
        [SerializeField] private UIPanelFocusMode focusMode = UIPanelFocusMode.Disabled;

        public int Thickness => Mathf.Max(1, thickness);
        public UIPanelFocusMode FocusMode => focusMode;
    }
}
