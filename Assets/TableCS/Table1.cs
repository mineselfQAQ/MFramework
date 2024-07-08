using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Table1
{
    public int ID { get; private set; }
	public string NAME { get; private set; }
	public string[] DESC { get; private set; }

    private Table1(int id, string name, string[] desc)
    {
        ID = id;
		NAME = name;
		DESC = desc;
    }

    public static Table1[] LoadBytes()
    {
        string path = $"F:/___MYPROJECT___/UnityProject/MFramework/Assets/StreamingAssets/ExcelBIN/Table1.byte";
        if (!File.Exists(path)) return null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Table1s table = binaryFormatter.Deserialize(stream) as Table1s;
            Table1[] res = table.items;
            return res;
        }
    }
}

[Serializable]
internal class Table1s
{
    public Table1[] items;

    private Table1s(Table1[] items)
    {
        this.items = items;
    }
}