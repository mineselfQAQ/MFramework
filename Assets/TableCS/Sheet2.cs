using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Table
{
    [Serializable]
    public class Sheet2
    {
        private int id;
		private string name;
		private string[] desc;
        
        public int ID { get; private set; }
		public string NAME { get; private set; }
		public string[] DESC { get; private set; }

        private Sheet2(int id, string name, string[] desc)
        {
            ID = id;
			NAME = name;
			DESC = desc;
        }

        public static Sheet2[] LoadBytes()
        {
            string path = @"F:\UnityProject\MFramework\Assets\StreamingAssets\ExcelBIN\Sheet2.byte";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Sheet2s table = binaryFormatter.Deserialize(stream) as Sheet2s;
                Sheet2[] res = table.items;
                return res;
            }
        }
    }

    [Serializable]
    internal class Sheet2s
    {
        public Sheet2[] items;

        private Sheet2s(Sheet2[] items)
        {
            this.items = items;
        }
    }
}