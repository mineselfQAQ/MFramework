using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework.Examples.MText
{
    public sealed class MEntry : MEntryBase
    {
        [SerializeField] private string startLanguage = "zh";
        [SerializeField] private string firstKey = "dialog.hello";
        [SerializeField] private string secondKey = "dialog.warning";
        [SerializeField] private MFramework.Text.MText target;
        [SerializeField] private Button languageButton;
        [SerializeField] private Button lineButton;
        [SerializeField] private Button finishButton;

        private bool _toggle;
        private MLocalizationManager _localizationManager;

        protected override IModule[] ConfigureModules()
        {
            return new IModule[]
            {
                new MTextModule(),
            };
        }

        protected override void OnInitialized(MFramework.Core.Tracker.TrackerStoppedEvent e)
        {
            _localizationManager = Core.Container.Resolve<MLocalizationManager>();

            EnsureText("dialog.hello", "zh", "Hello, {wave}MText{/wave} now uses timeline driven animation.");
            EnsureText("dialog.hello", "en", "Hello, {wave}MText{/wave} now uses timeline driven animation.");
            EnsureText("dialog.warning", "zh", "{color=#ffcc00}Warning{/color}: finish typewriter instantly or switch language.");
            EnsureText("dialog.warning", "en", "{color=#ffcc00}Warning{/color}: finish typewriter instantly or switch language.");

            _localizationManager.SetLanguage(startLanguage);
            BindButtons();
        }

        public void ToggleLanguage()
        {
            if (_localizationManager == null) return;

            string language = _localizationManager.CurrentLanguage == "zh" ? "en" : "zh";
            _localizationManager.SetLanguage(language);
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

        private void EnsureText(string key, string language, string value)
        {
            if (!_localizationManager.TryGetText(key, language, out _))
            {
                _localizationManager.SetText(key, language, value);
            }
        }

        private void BindButtons()
        {
            languageButton = languageButton != null ? languageButton : FindButton("LanguageButton");
            lineButton = lineButton != null ? lineButton : FindButton("LineButton");
            finishButton = finishButton != null ? finishButton : FindButton("FinishButton");

            languageButton?.onClick.AddListener(ToggleLanguage);
            lineButton?.onClick.AddListener(ToggleLine);
            finishButton?.onClick.AddListener(FinishImmediately);
        }

        private static Button FindButton(string buttonName)
        {
            foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (button.name == buttonName)
                {
                    return button;
                }
            }

            return null;
        }
    }
}
