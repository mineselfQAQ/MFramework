using UnityEngine;

namespace MFramework
{
    public class UIWidgetBehaviour : UIViewBehaviour
    {
        [SerializeField] private UIWidgetMode widgetMode = UIWidgetMode.Simple;
        public UIWidgetMode WidgetMode { get { return widgetMode; } }
    }
}