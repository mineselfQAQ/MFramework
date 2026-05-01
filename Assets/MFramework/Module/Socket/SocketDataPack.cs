using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MFramework.Socket
{
    public sealed class DataBuffer
    {
        private const int MinBufferLength = 1024;

        private byte[] _buffer;
        private int _length;

        internal bool HaveBuffer => _length > 0;

        public DataBuffer(int minBufferLength = MinBufferLength)
        {
            if (minBufferLength <= 0)
            {
                minBufferLength = MinBufferLength;
            }

            _buffer = new byte[minBufferLength];
        }

        public void AddBuffer(byte[] data, int length)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (length <= 0) return;
            if (length > data.Length) throw new ArgumentOutOfRangeException(nameof(length));

            EnsureCapacity(_length + length);
            Array.Copy(data, 0, _buffer, _length, length);
            _length += length;
        }

        public bool TryUnpack(out TCPDataPack dataPack)
        {
            dataPack = TCPDataPack.Unpack(_buffer, _length);
            if (dataPack == null) return false;

            RemovePrefix(dataPack.BufferLength);
            return true;
        }

        public bool TryUnpack(UDPDataPackAssembler assembler, out UDPDataPack dataPack)
        {
            if (assembler == null) throw new ArgumentNullException(nameof(assembler));

            dataPack = assembler.Unpack(_buffer, _length, out int packetLength);
            if (packetLength > 0)
            {
                RemovePrefix(packetLength);
            }

            return dataPack != null;
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= _buffer.Length) return;

            int newSize = Math.Max(capacity, _buffer.Length * 2);
            Array.Resize(ref _buffer, newSize);
        }

        private void RemovePrefix(int length)
        {
            _length -= length;
            if (_length > 0)
            {
                Array.Copy(_buffer, length, _buffer, 0, _length);
            }

            if (_buffer.Length > MinBufferLength && _length < MinBufferLength)
            {
                Array.Resize(ref _buffer, MinBufferLength);
            }
        }
    }

    public sealed class TCPDataPack
    {
        public const int HeadDataLength = 4;
        public const int HeadTypeLength = 2;
        public const int HeadLength = HeadDataLength + HeadTypeLength;

        public ushort Type { get; }
        public byte[] Data { get; }
        public byte[] Buffer { get; }
        public int BufferLength => Buffer.Length;
        public int DataLength => Data.Length;

        public TCPDataPack(ushort type, byte[] data)
        {
            Type = type;
            Data = data ?? Array.Empty<byte>();
            Buffer = GetBuffer(Type, Data);
        }

        public static byte[] GetBuffer(ushort type, byte[] data)
        {
            data ??= Array.Empty<byte>();
            byte[] buffer = new byte[data.Length + HeadLength];

            Array.Copy(BitConverter.GetBytes(buffer.Length), 0, buffer, 0, HeadDataLength);
            Array.Copy(BitConverter.GetBytes(type), 0, buffer, HeadDataLength, HeadTypeLength);
            Array.Copy(data, 0, buffer, HeadLength, data.Length);

            return buffer;
        }

        public static TCPDataPack Unpack(byte[] buffer, int availableLength)
        {
            if (buffer == null || availableLength < HeadLength) return null;

            int bufferLength = BitConverter.ToInt32(buffer, 0);
            if (bufferLength < HeadLength || bufferLength > availableLength) return null;

            int dataLength = bufferLength - HeadLength;
            ushort type = BitConverter.ToUInt16(buffer, HeadDataLength);
            byte[] data = new byte[dataLength];
            Array.Copy(buffer, HeadLength, data, 0, dataLength);

            return new TCPDataPack(type, data);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Data);
        }
    }

    public sealed class UDPDataPack
    {
        public const int MaxSegmentLength = 1024;
        public const int HeadPacketIdLength = 2;
        public const int HeadFragmentIndexLength = 2;
        public const int HeadTotalFragmentLength = 2;
        public const int HeadDataLength = 2;
        public const int HeadTypeLength = 2;
        public const int HeadLength = HeadPacketIdLength + HeadFragmentIndexLength + HeadTotalFragmentLength + HeadDataLength + HeadTypeLength;
        public const int MaxDataLength = MaxSegmentLength - HeadLength;

        private static readonly Random Random = new Random();
        private static readonly object RandomLock = new object();

        public ushort Id { get; }
        public ushort TotalFragment { get; }
        public ushort Type { get; }
        public byte[] Data { get; }
        public int DataLength => Data.Length;
        public IReadOnlyList<byte[]> Packets { get; }

        public UDPDataPack(ushort type, byte[] data)
        {
            Type = type;
            Data = data ?? Array.Empty<byte>();
            Id = GenerateId();
            Packets = CreatePackets(Type, Data, Id, out ushort totalFragment);
            TotalFragment = totalFragment;
        }

        internal UDPDataPack(ushort id, ushort totalFragment, ushort type, byte[] data)
        {
            Id = id;
            TotalFragment = totalFragment;
            Type = type;
            Data = data ?? Array.Empty<byte>();
            Packets = Array.Empty<byte[]>();
        }

        private static IReadOnlyList<byte[]> CreatePackets(ushort type, byte[] data, ushort id, out ushort totalFragment)
        {
            int fragmentCount = data.Length == 0 ? 1 : (data.Length - 1 + MaxDataLength) / MaxDataLength;
            if (fragmentCount > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "UDP payload is too large to fragment.");
            }

            totalFragment = (ushort)fragmentCount;
            var packets = new List<byte[]>(fragmentCount);

            for (int i = 0; i < fragmentCount; i++)
            {
                int offset = i * MaxDataLength;
                int length = Math.Min(MaxDataLength, data.Length - offset);
                if (data.Length == 0) length = 0;

                byte[] packetData = new byte[length];
                Array.Copy(data, offset, packetData, 0, length);
                packets.Add(CreatePacket(packetData, id, (ushort)i, totalFragment, type));
            }

            return packets;
        }

        private static byte[] CreatePacket(byte[] packetData, ushort id, ushort index, ushort totalFragment, ushort type)
        {
            byte[] packet = new byte[packetData.Length + HeadLength];

            Array.Copy(BitConverter.GetBytes(id), 0, packet, 0, 2);
            Array.Copy(BitConverter.GetBytes(index), 0, packet, 2, 2);
            Array.Copy(BitConverter.GetBytes(totalFragment), 0, packet, 4, 2);
            Array.Copy(BitConverter.GetBytes((ushort)packetData.Length), 0, packet, 6, 2);
            Array.Copy(BitConverter.GetBytes(type), 0, packet, 8, 2);
            Array.Copy(packetData, 0, packet, HeadLength, packetData.Length);

            return packet;
        }

        private static ushort GenerateId()
        {
            lock (RandomLock)
            {
                return (ushort)Random.Next(0, ushort.MaxValue + 1);
            }
        }
    }

    public sealed class UDPDataPackAssembler
    {
        private sealed class PacketAssembler
        {
            private readonly byte[][] _fragments;

            public PacketAssembler(ushort totalFragment)
            {
                _fragments = new byte[totalFragment][];
            }

            public bool AddFragment(ushort index, byte[] data)
            {
                if (index >= _fragments.Length) return false;

                _fragments[index] ??= data;
                return _fragments.All(fragment => fragment != null);
            }

            public byte[] Assemble()
            {
                using var stream = new MemoryStream();
                foreach (byte[] fragment in _fragments)
                {
                    stream.Write(fragment, 0, fragment.Length);
                }

                return stream.ToArray();
            }
        }

        private readonly Dictionary<ushort, PacketAssembler> _assemblers = new Dictionary<ushort, PacketAssembler>();

        public UDPDataPack Unpack(byte[] buffer, int availableLength, out int packetLength)
        {
            packetLength = -1;
            if (buffer == null || availableLength < UDPDataPack.HeadLength) return null;

            ushort id = BitConverter.ToUInt16(buffer, 0);
            ushort index = BitConverter.ToUInt16(buffer, 2);
            ushort totalFragment = BitConverter.ToUInt16(buffer, 4);
            ushort dataLength = BitConverter.ToUInt16(buffer, 6);
            ushort type = BitConverter.ToUInt16(buffer, 8);

            if (totalFragment == 0) return null;

            packetLength = dataLength + UDPDataPack.HeadLength;
            if (availableLength < packetLength) return null;

            byte[] packetData = new byte[dataLength];
            Array.Copy(buffer, UDPDataPack.HeadLength, packetData, 0, dataLength);

            if (!_assemblers.TryGetValue(id, out PacketAssembler assembler))
            {
                assembler = new PacketAssembler(totalFragment);
                _assemblers.Add(id, assembler);
            }

            bool isComplete = assembler.AddFragment(index, packetData);
            if (!isComplete) return null;

            byte[] data = assembler.Assemble();
            _assemblers.Remove(id);
            return new UDPDataPack(id, totalFragment, type, data);
        }
    }
}
