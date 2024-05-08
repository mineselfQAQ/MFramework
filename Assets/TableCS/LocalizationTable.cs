using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Table
{
    [Serializable]
    public class LocalizationTable
    {
        private string key;
		private int id;
		private string chinese;
		private string english;
        
        public string KEY { get; private set; }
		public int ID { get; private set; }
		public string CHINESE { get; private set; }
		public string ENGLISH { get; private set; }

        private LocalizationTable(string key, int id, string chinese, string english)
        {
            KEY = key;
			ID = id;
			CHINESE = chinese;
			ENGLISH = english;
        }

        public static LocalizationTable[] LoadBytes()
        {
            string path = @"F:\MineselfDemo\MFramework\Assets\StreamingAssets\ExcelBIN\LocalizationTable.byte";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                LocalizationTables table = binaryFormatter.Deserialize(stream) as LocalizationTables;
                LocalizationTable[] res = table.items;
                return res;
            }
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