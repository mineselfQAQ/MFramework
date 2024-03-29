using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MFramework
{
    public static class MSerializationUtility
    {
        public static byte[] ByteSerialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static object ByteDeserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(memoryStream);
            }
        }

        public static T ByteDeserialize<T>(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(memoryStream);
            }
        }


    }
}