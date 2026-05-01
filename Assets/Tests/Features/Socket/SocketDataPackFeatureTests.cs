using System;
using System.Linq;
using System.Text;
using MFramework.Socket;
using NUnit.Framework;

namespace Tests.Features.Socket
{
    public class SocketDataPackFeatureTests
    {
        [Test]
        public void Feature_TCPDataPack_RestoresBufferedPacketsInOrder()
        {
            byte[] first = TCPDataPack.GetBuffer((ushort)SocketEvent.Empty, Encoding.UTF8.GetBytes("first"));
            byte[] second = TCPDataPack.GetBuffer((ushort)SocketEvent.C2SHead, Encoding.UTF8.GetBytes("second"));
            byte[] combined = first.Concat(second).ToArray();
            var buffer = new DataBuffer();

            buffer.AddBuffer(combined, combined.Length);

            Assert.IsTrue(buffer.TryUnpack(out TCPDataPack firstPack));
            Assert.AreEqual((ushort)SocketEvent.Empty, firstPack.Type);
            Assert.AreEqual("first", firstPack.ToString());

            Assert.IsTrue(buffer.TryUnpack(out TCPDataPack secondPack));
            Assert.AreEqual((ushort)SocketEvent.C2SHead, secondPack.Type);
            Assert.AreEqual("second", secondPack.ToString());
        }

        [Test]
        public void Feature_UDPDataPack_AssemblesFragmentedPayload()
        {
            byte[] payload = Enumerable.Range(0, UDPDataPack.MaxDataLength + 32)
                .Select(i => (byte)(i % byte.MaxValue))
                .ToArray();
            var pack = new UDPDataPack((ushort)SocketEvent.Empty, payload);
            var assembler = new UDPDataPackAssembler();
            var buffer = new DataBuffer();

            UDPDataPack assembled = null;
            foreach (byte[] packet in pack.Packets)
            {
                buffer.AddBuffer(packet, packet.Length);
                if (buffer.TryUnpack(assembler, out UDPDataPack candidate))
                {
                    assembled = candidate;
                }
            }

            Assert.NotNull(assembled);
            Assert.AreEqual((ushort)SocketEvent.Empty, assembled.Type);
            CollectionAssert.AreEqual(payload, assembled.Data);
        }
    }
}
