using UnityEngine;

namespace MFramework
{
    public class UIWidget : UIView
    {
        protected string widgetID { get { return viewID; } }

        protected UIView parentView;

        public UIWidgetBehaviour widgetBehaviour { get { return (UIWidgetBehaviour)viewBehaviour; } }

        protected internal void Create(string id, Transform parentTrans, string prefabPath, UIView parent)
        {
            parentView = parent;
            base.Create(id, parentTrans, prefabPath);
        }

        protected void DestroySelf()
        {
            parentView.DestroyWidget(widgetID);
        }

        protected internal override void DestroyingInternal()
        {
            parentView = null;

            base.DestroyingInternal();
        }
    }
}