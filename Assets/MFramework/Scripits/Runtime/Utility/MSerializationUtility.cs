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
        //=====Xml���л�����====
        //Ĭ���ڸ�Ŀ¼�´洢(�༭���뷢������)
        public static UTF8Encoding UTF8 = new UTF8Encoding(false);

        public static void SaveToXml<T>(string filePath, T instance)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            //xmlWriter---XML����д����
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(stream, UTF8);
            writer.Formatting = Formatting.None;//����ģʽ

            //namesapces---��Ҫ�������أ�������ڸ��ڵ���������ܳ��������ռ�
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, instance, ns);

            writer.Close();
            stream.Close();

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.xml�ѳɹ����ǣ�·����{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.xml�ѳɹ�д�룬·����{fullPath}");
        }
        public static void SaveToXml<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");

            int isOverwrite = CheckOverwrite(fullPath, mode);
            if (isOverwrite == -1) return;

            //xmlWriter---XML����д����
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(stream, UTF8);
            if (isPrettyPrint) writer.Formatting = Formatting.Indented;//�����ʽ(�ỻ��)
            else writer.Formatting = Formatting.None;//����ģʽ

            //namesapces---��Ҫ�������أ�������ڸ��ڵ���������ܳ��������ռ�
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, instance, ns);

            writer.Close();
            stream.Close();

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.xml�ѳɹ����ǣ�·����{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.xml�ѳɹ�д�룬·����{fullPath}");
        }

        public static object ReadFromXml(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            FileStream stream = null;
            try
            {
                //xmlReader---XML���ݶ�ȡ��
                stream = File.OpenRead(fullPath);
                XmlReader reader = XmlReader.Create(stream);

                XmlSerializer serializer = new XmlSerializer(type);
                object instance = serializer.Deserialize(reader);
                MLog.Print($"{fileName}.xml�ѻ�ȡ�ɹ�");

                stream.Close();
                return instance;
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}���л�ʧ�ܣ�����", MLogType.Warning);
                if (stream != null) stream.Close();
                return null;
            }
        }
        public static T ReadFromXml<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultXMLPath, "xml");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            FileStream stream = null;
            try
            {
                //xmlReader---XML���ݶ�ȡ��
                stream = File.OpenRead(fullPath);
                XmlReader reader = XmlReader.Create(stream);

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T instance = (T)serializer.Deserialize(reader);
                MLog.Print($"{fileName}.xml�ѻ�ȡ�ɹ�");

                stream.Close();
                return instance;
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}���л�ʧ�ܣ�����", MLogType.Warning);
                if (stream != null) stream.Close();
                return default(T);
            }
        }



        //=====Json���л�����====
        //Ĭ���ڸ�Ŀ¼�´洢(�༭���뷢������)

        public static void SaveToJson<T>(string filePath, T instance)
        {
            string text = JsonUtility.ToJson(instance, false);

            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            File.WriteAllText(fullPath, text);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.json�ѳɹ����ǣ�·����{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.json�ѳɹ�д�룬·����{fullPath}");
        }
        public static void SaveToJson<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string text = JsonUtility.ToJson(instance, false);

            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");

            int isOverwrite = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (isOverwrite == -1) return;

            File.WriteAllText(fullPath, text);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (isOverwrite == 1) MLog.Print($"{fileName}.json�ѳɹ����ǣ�·����{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.json�ѳɹ�д�룬·����{fullPath}");
        }

        public static object ReadFromJson(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            StreamReader sr = new StreamReader(fullPath);
            string text = sr.ReadToEnd();
            if (text.Length > 0)
            {
                object result = JsonUtility.FromJson(text, type);
                MLog.Print($"{fileName}.json�ѻ�ȡ�ɹ�");
                return result;
            }
            else
            {
                MLog.Print($"{typeof(MSerializationUtility)}��{fileName}.json���������ݣ�����", MLogType.Warning);
                return null;
            }
        }
        public static T ReadFromJson<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultJSONPath, "json");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            StreamReader sr = new StreamReader(fullPath);
            string text = sr.ReadToEnd();
            if (text.Length > 0)
            {
                T result = JsonUtility.FromJson<T>(text);
                MLog.Print($"{fileName}.json�ѻ�ȡ�ɹ�");
                return result;
            }
            else
            {
                MLog.Print($"{typeof(MSerializationUtility)}��{fileName}.json���������ݣ�����", MLogType.Warning);
                return default(T);
            }
        }



        //=====���������л�����====
        //---�ļ���ϵ---
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
            if (isOverwrite == 1) MLog.Print($"{fileName}.byte�ѳɹ����ǣ�·����{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.byte�ѳɹ�д�룬·����{fullPath}");
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
            if (isOverwrite == 1) MLog.Print($"{fileName}.byte�ѳɹ����ǣ�·����{fullPath}");
            else if (isOverwrite == 0) MLog.Print($"{fileName}.byte�ѳɹ�д�룬·����{fullPath}");
        }

        public static object ReadFromByte(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultBYTEPath, "byte");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����.", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                object instance = binaryFormatter.Deserialize(fileStream);
                MLog.Print($"{fileName}.byte�ѻ�ȡ�ɹ�");
                return instance;
            }
        }
        public static T ReadFromByte<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MEditorPersistentSettings.DefaultBYTEPath, "byte");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                T instance = (T)binaryFormatter.Deserialize(fileStream);
                MLog.Print($"{fileName}.byte�ѻ�ȡ�ɹ�");
                return instance;
            }
        }

        //---�ڴ���ϵ---
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
            if (Path.IsPathRooted(filePath))//����·����ʽ
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                fullPath = $@"{directoryPath}/{fullFileName}";
            }
            else//���·����ʽ
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
                    MLog.Print($"{typeof(MSerializationUtility)}��{filePath}�Ѵ���Byte�ļ�����ΪDontOverwriteģʽ������", MLogType.Warning);
                    return -1;
                }
            }
            return -1;
        }

        //---�ļ�ϵ---
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