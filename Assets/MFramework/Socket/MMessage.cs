using System.Collections.Generic;
using System.Text;

namespace MFramework
{
    public class MMessage
    {
        //TODO:支持自定义数据结构
        public string topic;

        public List<object> infos;

        private List<byte[]> bytesInfos;

        private MMessage() {  }

        public byte[] Msg2Bytes()
        {
            if (bytesInfos == null) return Encoding.UTF8.GetBytes(topic);

            StringBuilder sb = new StringBuilder();
            sb.Append($"{topic}:");

            foreach (var info in bytesInfos)
            {
                sb.Append($"{Encoding.ASCII.GetString(info)}|");
            }
            sb.Remove(sb.Length - 1, 1);

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static MMessage CreateMessage(string topic)
        {
            MMessage msg = new MMessage();
            msg.topic = topic;
            msg.bytesInfos = new List<byte[]>();

            return msg;
        }

        public static MMessage Bytes2Msg(byte[] bytes)
        {
            MMessage msg = new MMessage();
            string str = Encoding.UTF8.GetString(bytes);

            string[] strs = str.Split(":");

            //---Topic---
            msg.topic = strs[0];
            //---Infos---
            string[] infoStrs = strs[1].Split("|");
            if (infoStrs.Length == 0) msg.bytesInfos = null;
            msg.infos = new List<object>();
            for (int i = 0; i < strs.Length; i++)
            {
                byte[] byteInfo = Encoding.ASCII.GetBytes(infoStrs[i]);

                object info = MSerializationUtility.ByteDeserialize(byteInfo);
                msg.infos.Add(info);
            }

            return msg;
        }

        public void AddInfo<T>(params T[] info)
        {
            foreach (var o in info)
            {
                byte[] bytes = MSerializationUtility.ByteSerialize(o);
                bytesInfos.Add(bytes);
            }
        }

        public void AddInfo<T1, T2>(T1 info1, T2 info2)
        {
            byte[] bytes = MSerializationUtility.ByteSerialize(info1);
            bytesInfos.Add(bytes);

            bytes = MSerializationUtility.ByteSerialize(info2);
            bytesInfos.Add(bytes);
        }

        public void AddInfo<T1, T2, T3>(T1 info1, T2 info2, T3 info3)
        {
            byte[] bytes = MSerializationUtility.ByteSerialize(info1);
            bytesInfos.Add(bytes);

            bytes = MSerializationUtility.ByteSerialize(info2);
            bytesInfos.Add(bytes);

            bytes = MSerializationUtility.ByteSerialize(info3);
            bytesInfos.Add(bytes);
        }

        public void AddInfo<T1, T2, T3, T4>(T1 info1, T2 info2, T3 info3, T4 info4)
        {
            byte[] bytes = MSerializationUtility.ByteSerialize(info1);
            bytesInfos.Add(bytes);

            bytes = MSerializationUtility.ByteSerialize(info2);
            bytesInfos.Add(bytes);

            bytes = MSerializationUtility.ByteSerialize(info3);
            bytesInfos.Add(bytes);

            bytes = MSerializationUtility.ByteSerialize(info4);
            bytesInfos.Add(bytes);
        }
    }
}