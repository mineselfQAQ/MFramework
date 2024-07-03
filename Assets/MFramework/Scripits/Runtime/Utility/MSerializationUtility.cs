using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace MFramework
{
    public static class MSerializationUtility
    {
        //=====Xml序列化操作====
        //默认在根目录下存储(编辑器与发布皆是)
        public static UTF8Encoding UTF8 = new UTF8Encoding(false);

        public static void SaveToXml<T>(string filePath, T instance)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            //xmlWriter---XML数据写入流
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(stream, UTF8);
            writer.Formatting = Formatting.None;//单行模式

            //namesapces---需要将其隐藏，否则会在根节点出现两个很长的命名空间
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, instance, ns);

            writer.Close();
            stream.Close();

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.xml已成功覆盖，路径：{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.xml已成功写入，路径：{fullPath}");
        }
        public static void SaveToXml<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");

            int isOverwrite = CheckOverwrite(fullPath, mode);
            if (isOverwrite == -1) return;

            //xmlWriter---XML数据写入流
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(stream, UTF8);
            if (isPrettyPrint) writer.Formatting = Formatting.Indented;//优秀格式(会换行)
            else writer.Formatting = Formatting.None;//单行模式

            //namesapces---需要将其隐藏，否则会在根节点出现两个很长的命名空间
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, instance, ns);

            writer.Close();
            stream.Close();

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.xml已成功覆盖，路径：{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.xml已成功写入，路径：{fullPath}");
        }

        public static object ReadFromXml(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            FileStream stream = null;
            try
            {
                //xmlReader---XML数据读取流
                stream = File.OpenRead(fullPath);
                XmlReader reader = XmlReader.Create(stream);

                XmlSerializer serializer = new XmlSerializer(type);
                object instance = serializer.Deserialize(reader);
                MLog.Print($"{fileName}.xml已获取成功");

                stream.Close();
                return instance;
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}序列化失败，请检查", MLogType.Warning);
                if (stream != null) stream.Close();
                return null;
            }
        }
        public static T ReadFromXml<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            FileStream stream = null;
            try
            {
                //xmlReader---XML数据读取流
                stream = File.OpenRead(fullPath);
                XmlReader reader = XmlReader.Create(stream);

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T instance = (T)serializer.Deserialize(reader);
                MLog.Print($"{fileName}.xml已获取成功");

                stream.Close();
                return instance;
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}序列化失败，请检查", MLogType.Warning);
                if (stream != null) stream.Close();
                return default(T);
            }
        }



        //=====Json序列化操作====
        //默认在根目录下存储(编辑器与发布皆是)

        public static void SaveToJson<T>(string filePath, T instance)
        {
            string text = JsonUtility.ToJson(instance, false);

            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            File.WriteAllText(fullPath, text);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.json已成功覆盖，路径：{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.json已成功写入，路径：{fullPath}");
        }
        public static void SaveToJson<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string text = JsonUtility.ToJson(instance, false);

            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            File.WriteAllText(fullPath, text);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.json已成功覆盖，路径：{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.json已成功写入，路径：{fullPath}");
        }

        public static object ReadFromJson(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            StreamReader sr = new StreamReader(fullPath);
            string text = sr.ReadToEnd();
            if (text.Length > 0)
            {
                object result = JsonUtility.FromJson(text, type);
                MLog.Print($"{fileName}.json已获取成功");
                return result;
            }
            else
            {
                MLog.Print($"{typeof(MSerializationUtility)}：{fileName}.json不存在内容，请检查", MLogType.Warning);
                return null;
            }
        }
        public static T ReadFromJson<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            StreamReader sr = new StreamReader(fullPath);
            string text = sr.ReadToEnd();
            if (text.Length > 0)
            {
                T result = JsonUtility.FromJson<T>(text);
                MLog.Print($"{fileName}.json已获取成功");
                return result;
            }
            else
            {
                MLog.Print($"{typeof(MSerializationUtility)}：{fileName}.json不存在内容，请检查", MLogType.Warning);
                return default(T);
            }
        }



        //=====二进制序列化操作====
        //---文件流系---
        public static void SaveToByte(object instance, string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultBYTEPath, "byte");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.byte已成功覆盖，路径：{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.byte已成功写入，路径：{fullPath}");
        }
        public static void SaveToByte(object instance, string filePath, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultBYTEPath, "byte");

            int isOverwrite = CheckOverwrite(fullPath, mode);
            if (isOverwrite == -1) return;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.byte已成功覆盖，路径：{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.byte已成功写入，路径：{fullPath}");
        }

        public static object ReadFromByte(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultBYTEPath, "byte");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查.", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                object instance = binaryFormatter.Deserialize(fileStream);
                MLog.Print($"{fileName}.byte已获取成功");
                return instance;
            }
        }
        public static T ReadFromByte<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultBYTEPath, "byte");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                T instance = (T)binaryFormatter.Deserialize(fileStream);
                MLog.Print($"{fileName}.byte已获取成功");
                return instance;
            }
        }

        //---内存流系---
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



        private static string GetFullPath(string filePath, string prePath, string suffix)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullFileName = $"{fileName}.{suffix}";
            string directoryPath = Path.GetDirectoryName(filePath);

            string fullPath = null;
            if (Path.IsPathRooted(filePath))//绝对路径形式
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                fullPath = $@"{directoryPath}/{fullFileName}";
            }
            else//相对路径形式
            {
                string fullDirectoryPath = $@"{prePath}/{directoryPath}";
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }
                fullPath = $@"{fullDirectoryPath}/{fullFileName}";
            }

            return fullPath;
        }

        private static int CheckOverwrite(string filePath, SaveMode mode)
        {
            if (mode == SaveMode.Overwrite)
            {
                return MPathUtility.DeleteFileIfExist(filePath) ? 1 : 0;
            }
            else if (mode == SaveMode.DontOverwrite)
            {
                if (File.Exists(filePath))
                {
                    MLog.Print($"{typeof(MSerializationUtility)}：{filePath}已存在Byte文件，但为DontOverwrite模式，请检查", MLogType.Warning);
                    return -1;
                }
            }
            return -1;
        }

        //---文件系---
        public static void SaveFile(string filePath, string code)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(code);
                }
            }
        }

        public static string ReadFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (TextReader textReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    return textReader.ReadToEnd();
                }
            }
        }
    }

    public enum SaveMode
    {
        Overwrite,
        DontOverwrite
    }
}