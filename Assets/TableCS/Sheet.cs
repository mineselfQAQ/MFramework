using System;
using System.Collections.Generic;

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
    }

    [Serializable]
    public class Sheets
    {
        public List<Sheet> items;

        private Sheets(List<Sheet> items)
        {
            this.items = items;
        }
    }
}