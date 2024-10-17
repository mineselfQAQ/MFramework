using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace MFramework
{
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

        public static void LoadBytes(Action<LocalizationTable[]> onFinish)
        {
            string path = $"{Application.streamingAssetsPath}/LocalizationTable.byte";

            UnityWebRequest request = UnityWebRequest.Get(path);
            var op = request.SendWebRequest();

            while (!op.isDone)
            {
                //阻塞，强制等待
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                //ERROR
                onFinish(null);
            }
            else
            {
                // 得到二进制数据
                byte[] fileData = request.downloadHandler.data;

                // 使用 MemoryStream 来反序列化数据
                using (MemoryStream stream = new MemoryStream(fileData))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    LocalizationTables table = binaryFormatter.Deserialize(stream) as LocalizationTables;
                    LocalizationTable[] res = table.items;
                    onFinish(res);  // 使用回调函数传回数据
                }
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
}