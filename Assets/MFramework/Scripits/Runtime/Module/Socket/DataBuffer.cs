using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Socket缓冲区
    /// </summary>
    public class DataBuffer
    {
        private const int MIN_BUFF_LEN = 1024;//标准缓存区长度(一般来说传输的是文字，不会太大，同时小于链路层的1472)

        private byte[] _buff;
        private int _buffLength = 0;

        internal bool haveBuff => _buffLength > 0;

        public DataBuffer(int minBuffLen = MIN_BUFF_LEN)
        {
            if (minBuffLen <= 0)
            {
                minBuffLen = MIN_BUFF_LEN;
            }
            _buff = new byte[minBuffLen];
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        public void AddBuffer(byte[] data, int len)
        {
            byte[] buff = new byte[len];
            Array.Copy(data, buff, len);
            if (len > _buff.Length - _buffLength)//超过当前缓存
            {
                byte[] temp = new byte[_buffLength + len];
                Array.Copy(_buff, 0, temp, 0, _buffLength);
                Array.Copy(buff, 0, temp, _buffLength, len);
                _buff = temp;//将_buff扩容且数据转移并附加上新数据
            }
            else
            {
                Array.Copy(data, 0, _buff, _buffLength, len);//直接附加新数据
            }
            _buffLength += len;
        }

        public bool TryUnpack(out TCPDataPack dataPack)
        {
            dataPack = TCPDataPack.Unpack(_buff);

            if (dataPack == null) return false;
            //清理已取数据
            _buffLength -= dataPack.BuffLength;
            byte[] temp = new byte[_buffLength < MIN_BUFF_LEN ? MIN_BUFF_LEN : _buffLength];
            Array.Copy(_buff, dataPack.BuffLength, temp, 0, _buffLength);
            _buff = temp;

            return true;
        }
        public bool TryUnpack(out UDPDataPack dataPack)
        {
            dataPack = UDPDataPack.Unpack(_buff, out int packetLength);

            //接收过数据，可以从缓存中移除
            if (packetLength != -1)
            {
                _buffLength -= packetLength;
                byte[] temp = new byte[_buffLength < MIN_BUFF_LEN ? MIN_BUFF_LEN : _buffLength];
                Array.Copy(_buff, packetLength, temp, 0, _buffLength);
                _buff = temp;
            }
            if (dataPack == null) return false;

            return true;
        }
    }
}