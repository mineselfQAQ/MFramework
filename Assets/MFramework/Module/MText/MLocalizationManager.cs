using System;
using System.Collections.Generic;

namespace MFramework.Text
{
    public sealed class MLocalizationManager
    {
        private MTextLocalizationTable _table;
        private readonly Dictionary<string, Dictionary<string, string>> _runtimeTexts = new Dictionary<string, Dictionary<string, string>>();
        private string _currentLanguage = "zh";

        public static MLocalizationManager Active { get; private set; }

        public static event Action ActiveChanged;

        public event Action LanguageChanged;

        public string CurrentLanguage => _currentLanguage;
        public MTextLocalizationTable Table => _table;

        public static void SetActive(MLocalizationManager manager)
        {
            if (ReferenceEquals(Active, manager)) return;

            Active = manager;
            ActiveChanged?.Invoke();
        }

        public void SetTable(MTextLocalizationTable table)
        {
            _table = table;
            _table?.RebuildLookup();
            LanguageChanged?.Invoke();
        }

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

        public void SetLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language)) return;
            if (string.Equals(_currentLanguage, language, StringComparison.Ordinal)) return;

            _currentLanguage = language;
            LanguageChanged?.Invoke();
        }

        public bool TryGetText(string key, out string text)
        {
            text = null;
            if (_runtimeTexts.TryGetValue(key, out Dictionary<string, string> languageMap) &&
                languageMap.TryGetValue(_currentLanguage, out text))
            {
                return true;
            }

            return _table != null && _table.TryGetText(key, _currentLanguage, out text);
        }
    }
}
