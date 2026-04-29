using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MFramework.Core;
using UnityEngine;

namespace MFramework.Util
{
    public static class MSerializationUtil
    {
        private static readonly ILog _log = new InternalLog(nameof(MSerializationUtil));

        public static readonly UTF8Encoding UTF8 = new UTF8Encoding(false);

        public static bool SaveToXml<T>(string filePath, T instance)
        {
            return SaveToXml(filePath, instance, false, SaveMode.Overwrite);
        }

        public static bool SaveToXml<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_XML_PATH, "xml");
            if (string.IsNullOrEmpty(fullPath) || !CheckOverwrite(fullPath, mode))
            {
                return false;
            }

            try
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(fileStream, UTF8))
                    {
                        writer.Formatting = isPrettyPrint ? Formatting.Indented : Formatting.None;

                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add(string.Empty, string.Empty);

                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(writer, instance, ns);
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                _log.W($"Xml serialization failed: {fullPath}\n{exception}");
                return false;
            }
        }

        public static object ReadFromXml(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_XML_PATH, "xml");
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                _log.W($"Xml file does not exist: {fullPath}");
                return null;
            }

            try
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    using (XmlReader reader = XmlReader.Create(fileStream))
                    {
                        XmlSerializer serializer = new XmlSerializer(type);
                        return serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                _log.W($"Xml deserialization failed: {fullPath}\n{exception}");
                return null;
            }
        }

        public static T ReadFromXml<T>(string filePath)
        {
            object instance = ReadFromXml(filePath, typeof(T));
            return instance is T result ? result : default;
        }

        public static bool SaveToJson<T>(string filePath, T instance)
        {
            return SaveToJson(filePath, instance, false, SaveMode.Overwrite);
        }

        public static bool SaveToJson<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_JSON_PATH, "json");
            if (string.IsNullOrEmpty(fullPath) || !CheckOverwrite(fullPath, mode))
            {
                return false;
            }

            try
            {
                string text = JsonUtility.ToJson(instance, isPrettyPrint);
                File.WriteAllText(fullPath, text, UTF8);
                return true;
            }
            catch (Exception exception)
            {
                _log.W($"Json serialization failed: {fullPath}\n{exception}");
                return false;
            }
        }

        public static object ReadFromJson(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_JSON_PATH, "json");
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                _log.W($"Json file does not exist: {fullPath}");
                return null;
            }

            string text = File.ReadAllText(fullPath, UTF8);
            if (string.IsNullOrEmpty(text))
            {
                _log.W($"Json file is empty: {fullPath}");
                return null;
            }

            return JsonUtility.FromJson(text, type);
        }

        public static T ReadFromJson<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_JSON_PATH, "json");
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                _log.W($"Json file does not exist: {fullPath}");
                return default;
            }

            string text = File.ReadAllText(fullPath, UTF8);
            if (string.IsNullOrEmpty(text))
            {
                _log.W($"Json file is empty: {fullPath}");
                return default;
            }

            return JsonUtility.FromJson<T>(text);
        }

        public static bool SaveToByte(object instance, string filePath)
        {
            return SaveToByte(instance, filePath, SaveMode.Overwrite);
        }

        public static bool SaveToByte(object instance, string filePath, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_BYTE_PATH, "byte");
            if (string.IsNullOrEmpty(fullPath) || !CheckOverwrite(fullPath, mode))
            {
                return false;
            }

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            return true;
        }

        public static object ReadFromByte(string filePath)
        {
            string fullPath = GetFullPath(filePath, MPathCache.DEFAULT_BYTE_PATH, "byte");
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                _log.W($"Byte file does not exist: {fullPath}");
                return null;
            }

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStream);
            }
        }

        public static T ReadFromByte<T>(string filePath)
        {
            object instance = ReadFromByte(filePath);
            return instance is T result ? result : default;
        }

        public static void SaveToFile(string filePath, string code)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, UTF8))
                {
                    textWriter.Write(code);
                }
            }
        }

        public static string ReadFromFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (TextReader textReader = new StreamReader(fileStream, UTF8))
                {
                    return textReader.ReadToEnd();
                }
            }
        }

        public static byte[] SaveToByte<T>(T instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, instance);
                return memoryStream.ToArray();
            }
        }

        public static object ReadFromByte(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static T ReadFromByte<T>(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        private static string GetFullPath(string filePath, string defaultPath, string suffix)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _log.W("File path is empty.");
                return null;
            }

            string fileName = Path.GetFileName(filePath);
            if (fileName.Contains(".") && !fileName.EndsWith($".{suffix}", StringComparison.OrdinalIgnoreCase))
            {
                _log.W($"File extension must be .{suffix}: {fileName}");
                return null;
            }

            fileName = $"{Path.GetFileNameWithoutExtension(filePath)}.{suffix}";
            string directoryPath = Path.GetDirectoryName(filePath);

            string fullDirectoryPath;
            if (Path.IsPathRooted(filePath))
            {
                fullDirectoryPath = string.IsNullOrEmpty(directoryPath) ? MPathCache.PC_ROOT_PATH : directoryPath;
            }
            else
            {
                string relativeDirectory = string.IsNullOrEmpty(directoryPath) ? string.Empty : directoryPath;
                fullDirectoryPath = Path.Combine(defaultPath, relativeDirectory);
            }

            Directory.CreateDirectory(fullDirectoryPath);
            return Path.Combine(fullDirectoryPath, fileName);
        }

        private static bool CheckOverwrite(string filePath, SaveMode mode)
        {
            if (mode == SaveMode.Overwrite)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return true;
            }

            if (mode == SaveMode.DontOverwrite && File.Exists(filePath))
            {
                _log.W($"File already exists and overwrite is disabled: {filePath}");
                return false;
            }

            return true;
        }
    }

    public enum SaveMode
    {
        Overwrite,
        DontOverwrite
    }
}
