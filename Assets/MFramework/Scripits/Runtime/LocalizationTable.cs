using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MFramework
{
    [Serializable]
    public class LocalizationTable
    {
        public int ID { get; private set; }
		public string CHINESE { get; private set; }
		public string ENGLISH { get; private set; }

        private LocalizationTable(int id, string chinese, string english)
        {
            ID = id;
			CHINESE = chinese;
			ENGLISH = english;
        }

        public static LocalizationTable[] LoadBytes()
        {
            string path = $"{Application.streamingAssetsPath}/LocalizationTable.byte";

#if UNITY_ANDROID
            //TODO:安卓需要使用UnityWebRequest
#else
            if (!File.Exists(path)) return null;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                LocalizationTables table = binaryFormatter.Deserialize(stream) as LocalizationTables;
                LocalizationTable[] res = table.items;
                return res;
            }
#endif
        }
    }

    [Serializable]
    internal class LocalizationTables
    {
        public LocalizationTable[] items;

        private LocalizationTables(LocalizationTable[] items)
        {
            this.items = items;
        }
    }
}