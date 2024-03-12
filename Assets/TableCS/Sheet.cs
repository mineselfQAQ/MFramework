using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Table
{
    [Serializable]
    public class Sheet
    {
        private int id;
		private string name;
		private string[] desc;
        
        public int ID { get; private set; }
		public string NAME { get; private set; }
		public string[] DESC { get; private set; }

        private Sheet(int id, string name, string[] desc)
        {
            ID = id;
			NAME = name;
			DESC = desc;
        }

        public static Sheet[] LoadBytes()
        {
            string path = @"F:\UnityProject\MFramework\Assets\Resources\ExcelBIN\Sheet.byte";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Sheets table = binaryFormatter.Deserialize(stream) as Sheets;
                Sheet[] res = table.items;
                return res;
            }
        }
    }

    [Serializable]
    internal class Sheets
    {
        public Sheet[] items;

        private Sheets(Sheet[] items)
        {
            this.items = items;
        }
    }
}