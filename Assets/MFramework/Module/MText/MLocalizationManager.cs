using System;
using System.Collections.Generic;
using MFramework.Text.Generated;

namespace MFramework.Text
{
    public sealed class MLocalizationManager
    {
        private readonly Dictionary<string, Dictionary<string, string>> _runtimeTexts = new Dictionary<string, Dictionary<string, string>>();
        private string _currentLanguage = "zh";

        public event Action LanguageChanged;

        public string CurrentLanguage => _currentLanguage;

        public void SetText(string key, string language, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            if (string.IsNullOrWhiteSpace(language)) return;

            if (!_runtimeTexts.TryGetValue(key, out Dictionary<string, string> languageMap))
            {
                languageMap = new Dictionary<string, string>();
                _runtimeTexts.Add(key, languageMap);
            }

            languageMap[language] = value ?? string.Empty;
            LanguageChanged?.Invoke();
        }

        public bool LoadGeneratedTextTable()
        {
            MTextLocalization[] items = MTextLocalization.LoadBytes();
            if (items == null) return false;

            _runtimeTexts.Clear();
            foreach (MTextLocalization item in items)
            {
                if (item == null) continue;
                SetTextSilently(item.KEY, "zh", item.CHINESE ?? string.Empty);
                SetTextSilently(item.KEY, "en", item.ENGLISH ?? string.Empty);
            }

            LanguageChanged?.Invoke();
            return true;
        }

        public void SetLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language)) return;
            if (string.Equals(_currentLanguage, language, StringComparison.Ordinal)) return;

            _currentLanguage = language;
            LanguageChanged?.Invoke();
        }

        public bool TryGetText(string key, out string text)
        {
            return TryGetText(key, _currentLanguage, out text);
        }

        public bool TryGetText(string key, string language, out string text)
        {
            text = null;
            if (_runtimeTexts.TryGetValue(key, out Dictionary<string, string> languageMap) &&
                languageMap.TryGetValue(language, out text))
            {
                return true;
            }

            return false;
        }

        private void SetTextSilently(string key, string language, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            if (string.IsNullOrWhiteSpace(language)) return;

            if (!_runtimeTexts.TryGetValue(key, out Dictionary<string, string> languageMap))
            {
                languageMap = new Dictionary<string, string>();
                _runtimeTexts.Add(key, languageMap);
            }

            languageMap[language] = value;
        }
    }
}
