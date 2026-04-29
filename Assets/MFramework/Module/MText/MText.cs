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
            Refresh();
        }

        protected override void OnDisable()
        {
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

        public void SetLocalizationManager(MLocalizationManager manager)
        {
            if (ReferenceEquals(_subscribedManager, manager)) return;

            Unsubscribe();
            _subscribedManager = manager;
            if (_subscribedManager != null)
            {
                _subscribedManager.LanguageChanged += Refresh;
            }

            Refresh();
        }

        public void Refresh()
        {
            string source = text;

            if (_localization != null &&
                _localization.Mode == MTextLocalizationMode.On &&
                _subscribedManager != null &&
                _subscribedManager.TryGetText(_localization.Key, out string localized))
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

        private void Unsubscribe()
        {
            if (_subscribedManager == null) return;

            _subscribedManager.LanguageChanged -= Refresh;
            _subscribedManager = null;
        }
    }
}
