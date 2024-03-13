using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Table
{
    [Serializable]
    public class Weapon
    {
        private byte[] id;
		private string name;
		private string[] desc;
        
        public byte[] ID { get; private set; }
		public string NAME { get; private set; }
		public string[] DESC { get; private set; }

        private Weapon(byte[] id, string name, string[] desc)
        {
            ID = id;
			NAME = name;
			DESC = desc;
        }

        public static Weapon[] LoadBytes()
        {
            string path = @"F:\MineselfDemo\MFramework\Assets\Resources\ExcelBIN\Weapon.byte";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Weapons table = binaryFormatter.Deserialize(stream) as Weapons;
                Weapon[] res = table.items;
                return res;
            }
        }
    }

    [Serializable]
    internal class Weapons
    {
        public Weapon[] items;

        private Weapons(Weapon[] items)
        {
            this.items = items;
        }
    }
}