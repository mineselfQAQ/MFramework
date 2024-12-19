using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace MFramework
{
    public static class MSerializationUtility
    {
        //TIP:
        //所有文件默认路径：
        //根目录下文件类型文件夹---如"项目名/XmlSettings/..."

        //=====Xml序列化操作====
        public static UTF8Encoding UTF8 = new UTF8Encoding(false);

        public static bool SaveToXml<T>(string filePath, T instance)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");

            bool flag = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (!flag) return false;

            try
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                using (XmlTextWriter writer = new XmlTextWriter(fileStream, UTF8))//xmlWriter---XML数据写入流
                {
                    writer.Formatting = Formatting.None;//单行模式

                    //namesapces---需要将其隐藏，否则会在根节点出现两个很长的命名空间
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, instance, ns);
                }
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}序列化失败，请检查", MLogType.Warning);
            }

            return true;
        }
        public static bool SaveToXml<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");

            bool flag = CheckOverwrite(fullPath, mode);
            if (!flag) return false;

            try
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                using (XmlTextWriter writer = new XmlTextWriter(fileStream, UTF8))//xmlWriter---XML数据写入流
                {
                    if (isPrettyPrint) writer.Formatting = Formatting.Indented;//优秀格式(会换行)
                    else writer.Formatting = Formatting.None;//单行模式

                    //namesapces---需要将其隐藏，否则会在根节点出现两个很长的命名空间
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, instance, ns);
                }
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}序列化失败，请检查", MLogType.Warning);
            }

            return true;
        }

        public static object ReadFromXml(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            FileStream fileStream = null;
            try
            {
                using (fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                using (XmlReader reader = XmlReader.Create(fileStream))//xmlReader---XML数据读取流
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    object instance = serializer.Deserialize(reader);

                    return instance;
                }
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}反序列化失败，请检查", MLogType.Warning);
                return null;
            }
        }
        public static T ReadFromXml<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            FileStream fileStream = null;
            try
            {
                using (fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                using (XmlReader reader = XmlReader.Create(fileStream))//xmlReader---XML数据读取流
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    T instance = (T)serializer.Deserialize(reader);

                    return instance;
                }
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}反序列化失败，请检查", MLogType.Warning);
                return default(T);
            }
        }



        //=====Json序列化操作====
        public static bool SaveToJson<T>(string filePath, T instance)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");

            bool flag = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (!flag) return false;

            string text = JsonUtility.ToJson(instance, false);

            File.WriteAllText(fullPath, text);

            return true;
        }
        public static bool SaveToJson<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");

            bool flag = CheckOverwrite(fullPath, mode);
            if (!flag) return false;

            string text = JsonUtility.ToJson(instance, isPrettyPrint);

            File.WriteAllText(fullPath, text);

            return true;
        }

        public static object ReadFromJson(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            string text = File.ReadAllText(fullPath);
            if (text.Length > 0)
            {
                T result = JsonUtility.FromJson<T>(text);
                return result;
            }
            else
            {
                MLog.Print($"{typeof(MSerializationUtility)}：{fileName}.json不存在内容，请检查", MLogType.Warning);
                return default(T);
            }
        }
        public static T ReadFromJson<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");
            //检测文件是否存在
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件{fullPath}不存在，请检查", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            string text = File.ReadAllText(fullPath);
            if (text.Length > 0)
            {
                T result = JsonUtility.FromJson<T>(text);
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
        public static bool SaveToByte(object instance, string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");

            bool flag = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (!flag) return false;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            return true;
        }
        public static bool SaveToByte(object instance, string filePath, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");

            bool flag = CheckOverwrite(fullPath, mode);
            if (!flag) return false;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            return true;
        }

        public static object ReadFromByte(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");
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
                return instance;
            }
        }
        public static T ReadFromByte<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");
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
                return instance;
            }
        }



        //---文件系---
        public static void SaveToFile(string filePath, string code)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(code);
                }
            }
        }

        public static string ReadFromFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (TextReader textReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    return textReader.ReadToEnd();
                }
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



        /// <summary>
        /// 获取完整路径，如果不提供完整路径，会在项目根目录下创建文件
        /// </summary>
        private static string GetFullPath(string filePath, string defaultPath, string suffix)
        {
            string fileName = Path.GetFileName(filePath);
            //文件名要么是".后缀"形式，要么是不带后缀形式
            if (!fileName.Contains($".{suffix}") && fileName.Contains('.'))
            {
                MLog.Print($"{typeof(MSerializationUtility)}：文件名{fileName}不符合要求，请检查", MLogType.Warning);
                return null;
            }
            fileName = $"{Path.GetFileNameWithoutExtension(filePath)}.{suffix}";
            string directoryPath = filePath.CD();

            string fullPath = null;
            if (Path.IsPathRooted(filePath))//绝对路径形式
            {
                MPathUtility.CreateFolderIfNotExist(directoryPath);
                fullPath = $"{directoryPath}/{fileName}";
            }
            else//相对路径形式
            {
                string fullDirectoryPath = $"{defaultPath}/{directoryPath}";
                MPathUtility.CreateFolderIfNotExist(fullDirectoryPath);
                fullPath = $"{fullDirectoryPath}/{fileName}";
            }

            return fullPath;
        }

        private static bool CheckOverwrite(string filePath, SaveMode mode)
        {
            if (mode == SaveMode.Overwrite)
            {
                MPathUtility.DeleteFileIfExist(filePath);
                return true;//无论删不删，都说明可以进行下一步
            }
            else if (mode == SaveMode.DontOverwrite)
            {
                //不想覆盖，但是发生文件已存在
                if (File.Exists(filePath))
                {
                    MLog.Print($"{typeof(MSerializationUtility)}：{filePath}已存在但禁止覆盖，请检查", MLogType.Warning);
                    return false;
                }
            }
            return false;
        }

    }

    public enum SaveMode
    {
        Overwrite,
        DontOverwrite
    }
}