using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MFramework.Text.Generated
{
    [Serializable]
    public class MTextLocalization
    {
        public string KEY { get; private set; }
        public string CHINESE { get; private set; }
        public string ENGLISH { get; private set; }

        private MTextLocalization(string key, string chinese, string english)
        {
            KEY = key;
            CHINESE = chinese;
            ENGLISH = english;
        }

        public static MTextLocalization[] LoadBytes()
        {
            string path = $"{Application.streamingAssetsPath}/MTextLocalization.byte";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MTextLocalizations table = binaryFormatter.Deserialize(stream) as MTextLocalizations;
                return table?.items;
            }
        }
    }

    [Serializable]
    internal class MTextLocalizations
    {
        public MTextLocalization[] items;

        private MTextLocalizations(MTextLocalization[] items)
        {
            this.items = items;
        }
    }
}