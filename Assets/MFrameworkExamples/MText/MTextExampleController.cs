using MFramework.Text;
using UnityEngine;

namespace MFramework.Examples.MText
{
    public sealed class MTextExampleController : MonoBehaviour
    {
        [SerializeField] private string firstKey = "dialog.hello";
        [SerializeField] private string secondKey = "dialog.warning";
        [SerializeField] private MFramework.Text.MText target;

        private bool _toggle;

        public void ToggleLanguage()
        {
            MLocalizationManager manager = MLocalizationManager.Active;
            if (manager == null) return;

            string language = manager.CurrentLanguage == "zh" ? "en" : "zh";
            manager.SetLanguage(language);
        }

        public void ToggleLine()
        {
            if (target == null) return;

            _toggle = !_toggle;
            target.SetLocalizationKey(_toggle ? secondKey : firstKey);
        }

        public void FinishImmediately()
        {
            target?.FinishTextImmediately();
        }
    }
}
