using UnityEngine;

namespace MFramework.UI
{
    public class UIWidgetBehaviour : UIViewBehaviour
    {
        [SerializeField] private UIWidgetMode widgetMode = UIWidgetMode.Simple;

        public UIWidgetMode WidgetMode => widgetMode;
    }
}
