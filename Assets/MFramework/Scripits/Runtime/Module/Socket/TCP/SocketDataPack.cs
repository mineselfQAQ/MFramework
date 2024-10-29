using System;

namespace MFramework
{
    /// <summary>
    /// Socket数据包(报文类型+数据)
    /// </summary>
    public class SocketDataPack
    {
        public static int HEAD_DATA_LEN = 4;
        public static int HEAD_TYPE_LEN = 2;
        public static int HEAD_LEN
        {
            get { return HEAD_DATA_LEN + HEAD_TYPE_LEN; }
        }

        public UInt16 Type;
        public byte[] Data;
        public byte[] Buff;//包---4bytes数据长度+2bytes报文类型+Nbytes数据(如：10(6bytes+数据4bytes) + 0x0001(心跳包) + "AB")

        public int BuffLength
        {
            get { return Buff.Length; }
        }
        public int DataLength
        {
            get { return Data.Length; }
        }

        public SocketDataPack() { }
        public SocketDataPack(UInt16 type, byte[] data)
        {
            Type = type;
            Data = data;

            Buff = GetBuff(Type, Data);
        }

        public static byte[] GetBuff(UInt16 type, byte[] data)
        {
            //buff总长度---数据总长度+报文类型(2bytes，即UInt16)+数据(data.Length)
            byte[] buff = new byte[data.Length + HEAD_LEN];
            byte[] temp;

            temp = BitConverter.GetBytes(buff.Length);//int为32位，一定返回byte[4]
            //1-4位存储buff.Length
            Array.Copy(temp, 0, buff, 0, HEAD_DATA_LEN);

            temp = BitConverter.GetBytes(type);//ushort为16为，一定返回byte[2]
            //5-6位存储type
            Array.Copy(temp, 0, buff, HEAD_DATA_LEN, HEAD_TYPE_LEN);

            //接下来都存储data
            Array.Copy(data, 0, buff, HEAD_LEN, data.Length);

            return buff;
        }

        public static SocketDataPack Unpack(byte[] buff)
        {
            try
            {
                //暂未获得完整数据(数据至少7bytes(6+1))
                if (buff.Length < HEAD_LEN) return null;

                byte[] temp = new byte[HEAD_DATA_LEN];
                Array.Copy(buff, 0, temp, 0, HEAD_DATA_LEN);

                //取长度
                int buffLength = BitConverter.ToInt32(temp, 0);
                if (buffLength <= 0) return null;
                if (buffLength > buff.Length) return null;
                int dataLength = buffLength - HEAD_LEN;
                //取报文类型
                temp = new byte[HEAD_TYPE_LEN];
                Array.Copy(buff, HEAD_DATA_LEN, temp, 0, HEAD_TYPE_LEN);
                UInt16 dataType = BitConverter.ToUInt16(temp, 0);
                //取数据
                byte[] data = new byte[dataLength];
                Array.Copy(buff, HEAD_LEN, data, 0, dataLength);

                var dataPack = new SocketDataPack(dataType, data);
                return dataPack;
            }
            catch
            {
                return null;
            }
        }
    }
}