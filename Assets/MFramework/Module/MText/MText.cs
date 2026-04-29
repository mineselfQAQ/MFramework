using TMPro;
using UnityEngine;

namespace MFramework.Text
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MLocalization))]
    [AddComponentMenu("MFramework/MText/MText")]
    public class MText : TextMeshProUGUI
    {
        private MLocalization _localization;
        private MTextAnimator _animator;
        private MLocalizationManager _subscribedManager;

        protected override void Awake()
        {
            base.Awake();
            _localization = GetComponent<MLocalization>();
            _animator = GetComponent<MTextAnimator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MLocalizationManager.ActiveChanged += OnActiveLocalizationManagerChanged;
            Subscribe();
            Refresh();
        }

        protected override void OnDisable()
        {
            MLocalizationManager.ActiveChanged -= OnActiveLocalizationManagerChanged;
            Unsubscribe();
            base.OnDisable();
        }

        public void SetMText(string value)
        {
            SetRawText(value);
        }

        public void SetLocalizationKey(string key)
        {
            _localization.Key = key;
            Refresh();
        }

        public void Refresh()
        {
            string source = text;
            MLocalizationManager manager = MLocalizationManager.Active;

            if (_localization != null &&
                _localization.Mode == MTextLocalizationMode.On &&
                manager != null &&
                manager.TryGetText(_localization.Key, out string localized))
            {
                source = localized;
            }

            SetRawText(source);
        }

        public void PlayText()
        {
            _animator?.PlayText();
        }

        public void FinishTextImmediately()
        {
            _animator?.FinishTextImmediately();
        }

        private void SetRawText(string value)
        {
            MTextParseResult parseResult = MTextInlineParser.Parse(value ?? string.Empty);
            text = parseResult.Text;
            _animator?.RebuildAndPlay(parseResult.Effects);
        }

        private void Subscribe()
        {
            MLocalizationManager manager = MLocalizationManager.Active;
            if (manager == null) return;

            if (ReferenceEquals(_subscribedManager, manager)) return;

            Unsubscribe();
            _subscribedManager = manager;
            _subscribedManager.LanguageChanged += Refresh;
        }

        private void OnActiveLocalizationManagerChanged()
        {
            Subscribe();
            Refresh();
        }

        private void Unsubscribe()
        {
            if (_subscribedManager == null) return;

            _subscribedManager.LanguageChanged -= Refresh;
            _subscribedManager = null;
        }
    }
}
