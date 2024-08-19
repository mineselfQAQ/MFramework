using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Sheet
{
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
        string path = $"D:/___UnityProject___/MFramework/Assets/StreamingAssets/ExcelBIN/Sheet.byte";
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