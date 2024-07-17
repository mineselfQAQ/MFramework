using MFramework;
using System.IO;
using UnityEngine;

/// <summary>
/// 提供保存删除(使用JSON)功能类
/// </summary>
public class GameSaver : ComponentSingleton<GameSaver>
{
    public string fileName = "Save";

    protected const int TotalSlots = 5;

    public virtual void Save(GameData data, int index)
    {
        SaveJSON(data, index);
    }
    protected virtual void SaveJSON(GameData data, int index)
    {
        string path = $"{Application.persistentDataPath}/{fileName}_{index}.json";
        MSerializationUtility.SaveToJson<GameData>(path, data);
    }

    /// <summary>
    /// 加载所有GameData(空存档处为null)
    /// </summary>
    /// <returns></returns>
    public virtual GameData[] LoadList()
    {
        var list = new GameData[TotalSlots];

        for (int i = 0; i < TotalSlots; i++)
        {
            var data = Load(i);

            if (data != null)
            {
                list[i] = data;
            }
        }

        return list;
    }
    public virtual GameData Load(int index)
    {
        return LoadJSON(index);
    }
    protected virtual GameData LoadJSON(int index)
    {
        string path = $"{Application.persistentDataPath}/{fileName}_{index}.json";

        if (File.Exists(path))
        {
            return MSerializationUtility.ReadFromJson<GameData>(path);
        }
        return null;
    }

    public virtual void Delete(int index)
    {
        DeleteJson(index);
    }
    protected virtual void DeleteJson(int index)
    {
        string path = $"{Application.persistentDataPath}/{fileName}_{index}.json";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
