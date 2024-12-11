using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MFramework
{
    public class UDPDataPack
    {
        /// <summary>
        /// 小包装配器
        /// </summary>
        private class PacketAssembler
        {
            private readonly UInt16 _totalFrag;
            private readonly byte[][] _fragments;

            public bool isComplete { get; private set; }

            public PacketAssembler(UInt16 totalFrag)
            {
                _totalFrag = totalFrag;
                _fragments = new byte[totalFrag][];
            }

            public void AddFragment(UInt16 index, byte[] data)
            {
                if (_fragments[index] == null)
                {
                    _fragments[index] = data;
                }

                //检查是否所有小包都到齐
                //在此处只需检查一次
                isComplete = _fragments.All(fragment => fragment != null);
            }

            public byte[] Assemble()
            {
                //只有所有包到齐才能装配
                if (!isComplete)
                {
                    return null;
                }

                //拼接所有分片(使用内存流)
                using (var ms = new MemoryStream())
                {
                    foreach (var fragment in _fragments)
                    {
                        ms.Write(fragment, 0, fragment.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static Dictionary<UInt16, PacketAssembler> PacketPool = new Dictionary<UInt16, PacketAssembler>();

        public static int MAX_SEG_LEN = 1024;//缓冲区总长
        public static int HEAD_PACKETID_LEN = 2;//包ID
        public static int HEAD_FRAGINDEX_LEN = 2;//当前分片索引
        public static int HEAD_TOTALFRAG_LEN = 2;//总分片数
        public static int HEAD_DATA_LEN = 2;//数据长度(UDP数据不长，这里2bytes足够)
        public static int HEAD_TYPE_LEN = 2;//报文类型
        /// <summary>
        /// 包头总长
        /// </summary>
        public static int HEAD_LEN
        {
            get { return HEAD_PACKETID_LEN + HEAD_FRAGINDEX_LEN + 
                    HEAD_TOTALFRAG_LEN + HEAD_DATA_LEN + HEAD_TYPE_LEN; }
        }
        /// <summary>
        /// 数据最大总长
        /// </summary>
        public static int MAX_DATA_LEN
        {
            get { return MAX_SEG_LEN - HEAD_LEN; }
        }

        public UInt16 ID;
        public UInt16 TotalFrag;
        public UInt16 Type;
        public byte[] Data;//总数据

        public int DataLength
        {
            get { return Data.Length; }
        }

        public List<byte[]> Packets;//拆包后包数据(包括包头)

        public UDPDataPack() { }
        public UDPDataPack(UInt16 type, byte[] data)
        {
            Type = type;
            Data = data;

            CreatePackets(Type, Data);
        }

        private void CreatePackets(UInt16 type, byte[] data)
        {
            if (Packets != null) return;
            Packets = new List<byte[]>();

            int maxDataSize = MAX_DATA_LEN;//最大负载
            if (data.Length == 0) TotalFrag = 1;//分片数---比如Event情况，只有1个小包
            else TotalFrag = (UInt16)((data.Length - 1 + maxDataSize) / maxDataSize);//分片数---一般情况
            ID = GenerateID();

            //创建每一个小包
            for (int i = 0; i < TotalFrag; i++)
            {
                int offset = i * maxDataSize;
                //小包长(前几个包为maxDataSize，最后一个包为data.Length - offset，即剩余长度)
                int length = Math.Min(maxDataSize, data.Length - offset);

                byte[] packetData = new byte[length];
                Array.Copy(data, offset, packetData, 0, length);

                var packet = CreatePacket(packetData, ID, (UInt16)i, TotalFrag, type);
                Packets.Add(packet);
            }
        }

        private byte[] CreatePacket(byte[] packetData, UInt16 id, UInt16 index, UInt16 totalFrag, UInt16 type)
        {
            //包头顺序如下：
            //0-2   2-4       4-6     6-8     8-10      10-N
            //ID    小包索引   总包数   小包长   报文类型   数据               
            byte[] packet = new byte[packetData.Length + HEAD_LEN];

            byte[] temp;
            temp = BitConverter.GetBytes(id);
            Array.Copy(temp, 0, packet, 0, 2);
            temp = BitConverter.GetBytes(index);
            Array.Copy(temp, 0, packet, 2, 2);
            temp = BitConverter.GetBytes(totalFrag);
            Array.Copy(temp, 0, packet, 4, 2);
            temp = BitConverter.GetBytes((UInt16)packetData.Length);
            Array.Copy(temp, 0, packet, 6, 2);
            temp = BitConverter.GetBytes(type);
            Array.Copy(temp, 0, packet, 8, 2);

            //Data
            Array.Copy(packetData, 0, packet, 10, packetData.Length);

            return packet;
        }

        public static UDPDataPack Unpack(byte[] buff, out int packetLength)
        {
            packetLength = -1;
            try
            {
                //暂未获得完整数据
                if (buff.Length < HEAD_LEN) return null;

                UInt16 id = BitConverter.ToUInt16(buff, 0);
                UInt16 index = BitConverter.ToUInt16(buff, 2);
                UInt16 totalFrag = BitConverter.ToUInt16(buff, 4);
                UInt16 dataLength = BitConverter.ToUInt16(buff, 6);
                UInt16 type = BitConverter.ToUInt16(buff, 8);

                packetLength = dataLength + HEAD_LEN;
                if (buff.Length < packetLength) return null;//数据量不足

                //获取包
                byte[] packetData = new byte[dataLength];
                Array.Copy(buff, HEAD_LEN, packetData, 0, dataLength);

                //将小包转入装配器
                if (!PacketPool.ContainsKey(id))
                {
                    PacketPool.Add(id, new PacketAssembler(totalFrag));
                }
                var assembler = PacketPool[id];
                assembler.AddFragment(index, packetData);
                //已收集全部小包，组成完整数据
                if (assembler.isComplete)
                {
                    var data = assembler.Assemble();
                    var dataPack = new UDPDataPack(type, data);

                    PacketPool.Remove(id);
                    IDTable.Remove(id);

                    return dataPack;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static HashSet<int> IDTable = new HashSet<int>();
        private static Random random = new Random();
        private static UInt16 GenerateID()
        {
            UInt16 value = (UInt16)random.Next(0, UInt16.MaxValue);
            if (IDTable.Contains(value))
            {
                value = GenerateID();
            }
            else
            {
                IDTable.Add(value);
            }

            return value;
        }
    }
}
