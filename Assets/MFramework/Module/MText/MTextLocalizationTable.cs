using System.Collections.Generic;
using UnityEngine;

namespace MFramework.Text
{
    [CreateAssetMenu(menuName = "MFramework/MText/Localization Table", fileName = "MTextLocalizationTable")]
    public sealed class MTextLocalizationTable : ScriptableObject
    {
        [SerializeField] private string defaultLanguage = "zh";
        [SerializeField] private List<MTextLocalizationEntry> entries = new List<MTextLocalizationEntry>();

        private Dictionary<string, Dictionary<string, string>> _lookup;

        public string DefaultLanguage => string.IsNullOrWhiteSpace(defaultLanguage) ? "zh" : defaultLanguage;

        public bool TryGetText(string key, string language, out string text)
        {
            BuildLookupIfNeeded();
            text = null;

            if (string.IsNullOrWhiteSpace(key)) return false;
            if (!_lookup.TryGetValue(key, out Dictionary<string, string> languageMap)) return false;

            if (!string.IsNullOrWhiteSpace(language) && languageMap.TryGetValue(language, out text))
            {
                return true;
            }

            return languageMap.TryGetValue(DefaultLanguage, out text);
        }

        public void RebuildLookup()
        {
            _lookup = new Dictionary<string, Dictionary<string, string>>();

            foreach (MTextLocalizationEntry entry in entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key)) continue;
                if (!_lookup.TryGetValue(entry.Key, out Dictionary<string, string> languageMap))
                {
                    languageMap = new Dictionary<string, string>();
                    _lookup.Add(entry.Key, languageMap);
                }

                foreach (MTextLocalizedValue value in entry.Values)
                {
                    if (value == null || string.IsNullOrWhiteSpace(value.Language)) continue;
                    languageMap[value.Language] = value.Value ?? string.Empty;
                }
            }
        }

        private void BuildLookupIfNeeded()
        {
            if (_lookup == null)
            {
                RebuildLookup();
            }
        }
    }
}
