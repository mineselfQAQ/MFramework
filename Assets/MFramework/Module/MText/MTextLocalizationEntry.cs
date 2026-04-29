using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework.Text
{
    [Serializable]
    public sealed class MTextLocalizedValue
    {
        [SerializeField] private string language = "zh";
        [TextArea(2, 6)]
        [SerializeField] private string value;

        public string Language => language;
        public string Value => value;
    }

    [Serializable]
    public sealed class MTextLocalizationEntry
    {
        [SerializeField] private string key;
        [SerializeField] private List<MTextLocalizedValue> values = new List<MTextLocalizedValue>();

        public string Key => key;
        public IReadOnlyList<MTextLocalizedValue> Values => values;
    }
}
