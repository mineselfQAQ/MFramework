namespace MFramework.Socket
{
    public enum SocketEvent : ushort
    {
        Empty = 0x0000,

        C2SHead = 0x0001,
        C2SDisconnectRequest = 0x0002,
        C2SDisconnectReply = 0x0003,

        S2CKickOut = 0x0010,
        S2CDisconnectReply = 0x0011,
        S2CDisconnectRequest = 0x0012,
    }
}
