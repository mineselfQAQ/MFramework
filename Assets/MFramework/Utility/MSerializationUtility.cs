using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MFramework
{
    public static class MSerializationUtility
    {
        //=====Json序列化操作====
        private static readonly string JSONPATH = @$"{Environment.CurrentDirectory}\JsonSettings";

        public static void SaveToJson<T>(string filePath, T obj, bool isPrettyPrint = false)
        {
            string text = JsonUtility.ToJson(obj, isPrettyPrint);
            string fullPath = GetFullPath(filePath);

            bool isOverwrite = false;
            if (File.Exists(fullPath)) isOverwrite = true;

            File.WriteAllText(fullPath, text);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite) MLog.Print($"{fileName}.json已成功覆盖.");
            else MLog.Print($"{fileName}.json已成功写入.");
        }

        public static T ReceiveFromJson<T>(string filePath)
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

        private static string GetFullPath(string filePath)
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

        //=====二进制序列化操作====
        //---文件流系---
        public static void SaveToByte(object instance, string filePath, FileMode fileMode = FileMode.Create)
        {
            using (FileStream fileStream = new FileStream(filePath, fileMode))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);//写入
            }
        }
        public static object ReceiveFromByte(string filePath, FileMode fileMode = FileMode.Open)
        {
            using (FileStream fileStream = new FileStream(filePath, fileMode))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStream);
            }
        }
        public static T ReceiveFromByte<T>(string filePath, FileMode fileMode = FileMode.Open)
        {
            using (FileStream fileStream = new FileStream(filePath, fileMode))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(fileStream);
            }
        }

        //---内存流系---
        public static byte[] SaveToByte(object instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, instance);//转换
                return memoryStream.ToArray();
            }
        }
        public static object ReceiveFromByte(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
        public static T ReceiveFromByte<T>(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}