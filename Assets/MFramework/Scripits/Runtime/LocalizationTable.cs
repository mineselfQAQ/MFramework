using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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
        string path = $"{Application.streamingAssetsPath}/ExcelBIN/LocalizationTable.byte";
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