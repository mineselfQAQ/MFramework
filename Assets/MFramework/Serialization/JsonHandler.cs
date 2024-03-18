using MFramework;
using System;
using System.IO;
using UnityEngine;

[MonoSingletonSetting(HideFlags.NotEditable, "#JsonHandler#")]
public class JsonHandler : MonoSingleton<JsonHandler>
{
    private static readonly string JSONPATH = @$"{Environment.CurrentDirectory}\JsonSettings";

    public void SaveToJson<T>(string filePath, T obj, bool isPrettyPrint = false)
    {
        string text = JsonUtility.ToJson(obj, isPrettyPrint);
        string fullPath = GetFullPath(filePath);

        bool isOverwrite = false;
        if (File.Exists(fullPath)) isOverwrite = true;

        File.WriteAllText(fullPath, text);

        string fileName = Path.GetFileNameWithoutExtension(filePath);
        if(isOverwrite) MLog.Print($"{fileName}.json已成功覆盖.");
        else MLog.Print($"{fileName}.json已成功写入.");
    }

    public T ReceiveFromJson<T>(string filePath)
    {
        string fullPath = GetFullPath(filePath);
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        if (!File.Exists(fullPath))
        {
            MLog.Print($"路径{fullPath}不存在，请检查.", MLogType.Error);
            return default(T);
        }

        StreamReader sr = new StreamReader(fullPath);
        string text = sr.ReadToEnd();

        if (text.Length > 0)
        {
            T result = JsonUtility.FromJson<T>(text);
            MLog.Print($"{fileName}.json已获取成功.");
            return result;
        }
        else
        {
            MLog.Print($"{fileName}.json不存在内容，请检查.", MLogType.Error);
            return default(T);
        }
    }

    private string GetFullPath(string filePath)
    {
        string directoryPath = Path.GetDirectoryName(filePath);
        string fullDirectoryPath = $@"{JSONPATH}\{directoryPath}";
        if (!Directory.Exists(fullDirectoryPath))
        {
            Directory.CreateDirectory(fullDirectoryPath);
        }

        string fileName = $"{Path.GetFileNameWithoutExtension(filePath)}.json";
        return $@"{fullDirectoryPath}\{fileName}";
    }
}
